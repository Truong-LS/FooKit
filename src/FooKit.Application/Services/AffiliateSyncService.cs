using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;
using FooKit.Domain.ValueObjects;

namespace FooKit.Application.Services
{
    public class AffiliateSyncService : IAffiliateSyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductSearchApiService _searchService;
        private readonly ILogger<AffiliateSyncService> _logger;

        public AffiliateSyncService(
            IUnitOfWork unitOfWork,
            IProductSearchApiService searchService,
            ILogger<AffiliateSyncService> logger)
        {
            _unitOfWork = unitOfWork;
            _searchService = searchService;
            _logger = logger;
        }

        public async Task ManualSyncAsync(bool forceSyncAll, string targetIngredientId)
        {
            _logger.LogInformation("Starting manual affiliate sync. ForceSyncAll: {ForceSyncAll}, TargetIngredientId: {TargetIngredientId}", forceSyncAll, targetIngredientId);

            var cutoffTime = DateTime.UtcNow.AddHours(-24);
            var maxLinks = 3; // Ideally injected via options

            var ingredientsToProcess = await _unitOfWork.StandardIngredients.GetIngredientsForSyncAsync(maxLinks, cutoffTime, forceSyncAll, targetIngredientId);

            if (!ingredientsToProcess.Any())
            {
                _logger.LogInformation("No ingredients matched the manual sync criteria.");
                return;
            }

            foreach (var ingredient in ingredientsToProcess)
            {
                try
                {
                    var existingUrls = ingredient.AffiliateProducts
                        .Where(ap => ap.IsActive)
                        .Select(ap => ap.ProductUrl)
                        .ToList();

                    var bestProduct = await _searchService.SearchBestProductAsync(ingredient.Name, existingUrls);

                    if (bestProduct == null) continue;

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

                    await _unitOfWork.AffiliateProducts.AddAsync(newProduct);

                    var activeLinks = ingredient.AffiliateProducts
                        .Where(ap => ap.IsActive)
                        .OrderByDescending(ap => ap.LastUpdatedPriceAt)
                        .ToList();

                    // +1 because we are adding a new product that hasn't been saved yet
                    if (activeLinks.Count + 1 > maxLinks)
                    {
                        var linksToDeactivate = activeLinks.Skip(maxLinks - 1).ToList();
                        foreach (var link in linksToDeactivate)
                        {
                            link.IsActive = false;
                            _unitOfWork.AffiliateProducts.Update(link);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing manual sync for ingredient {Name}", ingredient.Name);
                }
            }

            _logger.LogInformation("Manual affiliate sync completed.");
        }
    }
}
