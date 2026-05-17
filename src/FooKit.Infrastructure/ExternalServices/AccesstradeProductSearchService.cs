using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyProject.Application.Configuration;
using MyProject.Application.DTOs.AffiliateProductDtos;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.Infrastructure.ExternalServices;

/// <summary>
/// Calls the real Accesstrade Datafeed API to search for affiliate products on Shopee.
/// Uses IHttpClientFactory-managed HttpClient with Polly retry policies.
/// </summary>
public class AccesstradeProductSearchService : IProductSearchApiService
{
    private readonly HttpClient _httpClient;
    private readonly AffiliateWorkerOptions _options;
    private readonly ILogger<AccesstradeProductSearchService> _logger;

    public AccesstradeProductSearchService(
        HttpClient httpClient,
        IOptions<AffiliateWorkerOptions> options,
        ILogger<AccesstradeProductSearchService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AccesstradeProductDto?> SearchBestProductAsync(string ingredientName, List<string> existingUrls)
    {
        try
        {
            // Build request URL with URL-encoded keyword
            var encodedKeyword = WebUtility.UrlEncode(ingredientName);
            var requestUrl = $"{_options.SearchApiEndpoint}?keyword={encodedKeyword}&merchant=shopee&limit=5";

            // Create request with required Authorization header
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", $"Token {_options.AccessKey}");

            _logger.LogDebug("Calling Accesstrade API for ingredient: {IngredientName}", ingredientName);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AccesstradeResponseDto>(content);

            if (result == null || result.Data.Count == 0)
            {
                _logger.LogWarning("No products returned from Accesstrade for: {IngredientName}", ingredientName);
                return null;
            }

            // Select the best product: Price > 0, has valid affiliate_url, URL not already in DB
            var bestProduct = result.Data.FirstOrDefault(p =>
                p.Price > 0 &&
                !string.IsNullOrWhiteSpace(p.AffiliateUrl) &&
                !existingUrls.Contains(p.AffiliateUrl));

            if (bestProduct == null)
            {
                _logger.LogInformation(
                    "All {Count} products for '{IngredientName}' were filtered out (price=0, empty URL, or duplicate).",
                    result.Data.Count, ingredientName);
            }

            return bestProduct;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while searching Accesstrade for: {IngredientName}", ingredientName);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Accesstrade response for: {IngredientName}", ingredientName);
            return null;
        }
    }
}
