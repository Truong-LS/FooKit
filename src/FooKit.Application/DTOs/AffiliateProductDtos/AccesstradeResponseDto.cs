using System.Text.Json.Serialization;

namespace FooKit.Application.DTOs.AffiliateProductDtos;

/// <summary>
/// Root response from Accesstrade Datafeed API.
/// Maps the top-level JSON object containing total count and data array.
/// </summary>
public class AccesstradeResponseDto
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("data")]
    public List<AccesstradeProductDto> Data { get; set; } = new();
}

/// <summary>
/// Individual product item from Accesstrade Datafeed API.
/// JSON field names follow snake_case convention used by the API.
/// </summary>
public class AccesstradeProductDto
{
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("affiliate_url")]
    public string AffiliateUrl { get; set; } = string.Empty;

    [JsonPropertyName("merchant")]
    public string Merchant { get; set; } = string.Empty;
}
