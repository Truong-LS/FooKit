using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyProject.Application.Configuration;
using MyProject.Application.Interfaces.IServices;
using MyProject.Domain.Entities;
using MyProject.Domain.ValueObjects;
using MyProject.Infrastructure.Data.DBContext;

namespace MyProject.API.Workers;

/// <summary>
/// Background service that periodically scans StandardIngredients,
/// fetches real affiliate product data from Accesstrade API,
/// and manages the lifecycle of AffiliateProduct records via soft-delete.
/// </summary>
public class AutoSearchAffiliateWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AffiliateWorkerOptions _options;
    private readonly ILogger<AutoSearchAffiliateWorker> _logger;
    private readonly WorkerHealthTracker _tracker;

    public AutoSearchAffiliateWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<AffiliateWorkerOptions> options,
        ILogger<AutoSearchAffiliateWorker> logger,
        WorkerHealthTracker tracker)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
        _tracker = tracker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "AutoSearchAffiliateWorker started. Interval: {Hours}h, BatchSize: {Batch}, MaxLinks: {Max}",
            _options.IntervalHours, _options.BatchSize, _options.MaxActiveLinksPerIngredient);

        _tracker.IsWorkerRunning = true;

        // Wait a short time before the first run to let the app fully start
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromHours(_options.IntervalHours));

        try
        {
            // Run immediately on startup, then on each timer tick
            do
            {
                try
                {
                    await ProcessBatchAsync(stoppingToken);
                    _tracker.LastAffiliateSyncTime = DateTime.UtcNow;
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("AutoSearchAffiliateWorker is shutting down.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in AutoSearchAffiliateWorker cycle. Will retry next interval.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        finally
        {
            _tracker.IsWorkerRunning = false;
        }
    }

    /// <summary>
    /// Processes one batch of StandardIngredients: fetches affiliate data, inserts new records,
    /// and soft-deletes old ones exceeding the MaxActiveLinksPerIngredient limit.
    /// </summary>
    private async Task ProcessBatchAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FooKitDbContext>();
        var searchService = scope.ServiceProvider.GetRequiredService<IProductSearchApiService>();

        // Step 1: Fetch target ingredients that need refreshing
        var cutoffTime = DateTime.UtcNow.AddHours(-24);
        var maxLinks = _options.MaxActiveLinksPerIngredient;

        var ingredientsToProcess = await dbContext.StandardIngredients
            .Include(si => si.AffiliateProducts)
            .Where(si =>
                // Ingredients with fewer active links than the maximum
                si.AffiliateProducts.Count(ap => ap.IsActive) < maxLinks ||
                // OR ingredients whose newest active link is older than 24 hours
                si.AffiliateProducts
                    .Where(ap => ap.IsActive)
                    .OrderByDescending(ap => ap.LastUpdatedPriceAt)
                    .Select(ap => ap.LastUpdatedPriceAt)
                    .FirstOrDefault() < cutoffTime)
            .Take(_options.BatchSize)
            .ToListAsync(stoppingToken);

        _logger.LogInformation("Found {Count} ingredients to process in this cycle.", ingredientsToProcess.Count);

        if (ingredientsToProcess.Count == 0)
        {
            _logger.LogInformation("No ingredients need updating. Skipping this cycle.");
            return;
        }

        var totalInserted = 0;
        var totalDeactivated = 0;

        // Step 2 & 3: Loop through each ingredient, call API, parse results
        foreach (var ingredient in ingredientsToProcess)
        {
            stoppingToken.ThrowIfCancellationRequested();

            try
            {
                // Collect existing URLs for this ingredient to avoid duplicates
                var existingUrls = ingredient.AffiliateProducts
                    .Where(ap => ap.IsActive)
                    .Select(ap => ap.ProductUrl)
                    .ToList();

                var bestProduct = await searchService.SearchBestProductAsync(ingredient.Name, existingUrls);

                if (bestProduct == null)
                {
                    _logger.LogDebug("No suitable product found for ingredient: {Name}", ingredient.Name);
                    await Task.Delay(_options.DelayBetweenRequestsMs, stoppingToken);
                    continue;
                }

                // Step 4: Insert new AffiliateProduct
                var newProduct = new AffiliateProduct
                {
                    Id = Guid.NewGuid(),
                    StandardIngredientId = ingredient.Id,
                    ProductName = bestProduct.Name,
                    ProductUrl = bestProduct.AffiliateUrl,
                    CurrentPrice = new Money(bestProduct.Price, "VND"),
                    Platform = bestProduct.Merchant,
                    LastUpdatedPriceAt = DateTime.UtcNow,
                    IsActive = true
                };

                dbContext.AffiliateProducts.Add(newProduct);
                totalInserted++;

                _logger.LogInformation(
                    "Added new affiliate link for '{IngredientName}': {ProductName} - {Price:N0} VND",
                    ingredient.Name, bestProduct.Name, bestProduct.Price);

                // Step 4 (Soft Delete): Deactivate old links if exceeding the limit
                var activeLinks = await dbContext.AffiliateProducts
                    .Where(ap => ap.StandardIngredientId == ingredient.Id && ap.IsActive)
                    .OrderByDescending(ap => ap.LastUpdatedPriceAt)
                    .ToListAsync(stoppingToken);

                // +1 because the new product hasn't been saved yet but is tracked
                if (activeLinks.Count + 1 > maxLinks)
                {
                    // Keep the newest (maxLinks - 1) existing + 1 new = maxLinks total
                    var linksToDeactivate = activeLinks.Skip(maxLinks - 1).ToList();

                    foreach (var link in linksToDeactivate)
                    {
                        link.IsActive = false;
                        totalDeactivated++;
                    }

                    _logger.LogInformation(
                        "Deactivated {Count} old link(s) for ingredient '{Name}'.",
                        linksToDeactivate.Count, ingredient.Name);
                }

                // Step 5: Save after each ingredient
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing ingredient '{Name}' (ID: {Id}). Skipping.",
                    ingredient.Name, ingredient.Id);
            }

            // Delay between requests to avoid rate limiting
            await Task.Delay(_options.DelayBetweenRequestsMs, stoppingToken);
        }

        _logger.LogInformation(
            "Cycle complete. Inserted: {Inserted}, Deactivated: {Deactivated}",
            totalInserted, totalDeactivated);
    }
}
