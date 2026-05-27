using System.Text.Json.Serialization;

namespace MyProject.Application.DTOs.AdminDashboardDtos
{
    public class ApiUsageDto
    {
        [JsonPropertyName("period")]
        public PeriodDto Period { get; set; } = new();

        [JsonPropertyName("total_estimated_cost_usd")]
        public decimal TotalEstimatedCostUsd { get; set; }

        [JsonPropertyName("details")]
        public ApiUsageDetailsDto Details { get; set; } = new();

        [JsonPropertyName("cache_efficiency")]
        public CacheEfficiencyDto CacheEfficiency { get; set; } = new();
    }

    public class PeriodDto
    {
        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
    }

    public class ApiUsageDetailsDto
    {
        [JsonPropertyName("ai_provider")]
        public AiProviderUsageDto AiProvider { get; set; } = new();

        [JsonPropertyName("recipe_provider")]
        public RecipeProviderUsageDto RecipeProvider { get; set; } = new();
    }

    public class AiProviderUsageDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Google Gemini";

        [JsonPropertyName("total_requests")]
        public int TotalRequests { get; set; }

        [JsonPropertyName("total_tokens_used")]
        public long TotalTokensUsed { get; set; }

        [JsonPropertyName("estimated_cost_usd")]
        public decimal EstimatedCostUsd { get; set; }
    }

    public class RecipeProviderUsageDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Spoonacular";

        [JsonPropertyName("total_requests")]
        public int TotalRequests { get; set; }

        [JsonPropertyName("quota_used_percentage")]
        public double QuotaUsedPercentage { get; set; }

        [JsonPropertyName("estimated_cost_usd")]
        public decimal EstimatedCostUsd { get; set; }
    }

    public class CacheEfficiencyDto
    {
        [JsonPropertyName("ai_cache_hit_rate_percentage")]
        public double AiCacheHitRatePercentage { get; set; }
    }
}
