namespace MyProject.Application.Configuration;

/// <summary>
/// Configuration options for the Auto-Search Affiliate background worker.
/// Non-sensitive values are read from appsettings.json; AccessKey comes from environment variables.
/// </summary>
public class AffiliateWorkerOptions
{
    public const string SectionName = "AffiliateWorkerOptions";

    /// <summary>
    /// Accesstrade Datafeed API endpoint.
    /// </summary>
    public string SearchApiEndpoint { get; set; } = "https://api.accesstrade.vn/v1/datafeeds";

    /// <summary>
    /// How often the worker runs, in hours.
    /// </summary>
    public int IntervalHours { get; set; } = 12;

    /// <summary>
    /// Number of StandardIngredients to process per cycle.
    /// </summary>
    public int BatchSize { get; set; } = 20;

    /// <summary>
    /// Delay between consecutive API calls in milliseconds, to avoid rate limiting.
    /// </summary>
    public int DelayBetweenRequestsMs { get; set; } = 2000;

    /// <summary>
    /// Maximum number of active affiliate links allowed per ingredient.
    /// Older links beyond this limit will be soft-deleted (IsActive = false).
    /// </summary>
    public int MaxActiveLinksPerIngredient { get; set; } = 3;

    /// <summary>
    /// Accesstrade API authentication token. Loaded from environment variable ACCESSTRADE_ACCESS_KEY.
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;
}
