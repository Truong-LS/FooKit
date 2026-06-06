using FooKit.Application.DTOs.AffiliateProductDtos;

namespace FooKit.Application.Interfaces.IServices;

/// <summary>
/// Abstraction for searching affiliate products from an external provider.
/// </summary>
public interface IProductSearchApiService
{
    /// <summary>
    /// Searches for the best matching affiliate product by ingredient name.
    /// Filters out products whose URLs already exist in the database.
    /// </summary>
    /// <param name="ingredientName">The StandardIngredient name to search for.</param>
    /// <param name="existingUrls">URLs already stored in the database for this ingredient, to avoid duplicates.</param>
    /// <returns>The best matching product DTO, or null if no valid product was found.</returns>
    Task<AccesstradeProductDto?> SearchBestProductAsync(string ingredientName, List<string> existingUrls);
}
