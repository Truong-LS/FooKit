using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using FooKit.Application.DTOs.AdminDtos;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;

namespace FooKit.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly WorkerHealthTracker _tracker;
        private readonly IConfiguration _config;

        public AdminDashboardService(
            IUnitOfWork unitOfWork,
            WorkerHealthTracker tracker,
            IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _tracker = tracker;
            _config = config;
        }

        public async Task<DashboardOverviewDto> GetOverviewAsync()
        {
            var todayUtc = DateTime.UtcNow.Date;

            var totalUsers = (await _unitOfWork.Users.GetAllAsync()).Count();
            var premiumUsers = (await _unitOfWork.UserSubscriptions.FindAsync(us => us.IsActive && us.EndDate > DateTime.UtcNow))
                .Select(us => us.UserId)
                .Distinct()
                .Count();
            var newUsersToday = (await _unitOfWork.Users.FindAsync(u => u.CreatedAt >= todayUtc)).Count();

            var mealsGeneratedToday = (await _unitOfWork.SuggestionRequests.FindAsync(sr => sr.CreatedAt >= todayUtc)).Count();
            var totalActiveAffiliateLinks = (await _unitOfWork.AffiliateProducts.FindAsync(ap => ap.IsActive)).Count();

            return new DashboardOverviewDto
            {
                Timestamp = DateTime.UtcNow,
                UsersMetrics = new UserMetricsDto
                {
                    TotalUsers = totalUsers,
                    PremiumUsers = premiumUsers,
                    NewUsersToday = newUsersToday
                },
                ContentMetrics = new ContentMetricsDto
                {
                    MealsGeneratedToday = mealsGeneratedToday,
                    TotalActiveAffiliateLinks = totalActiveAffiliateLinks
                },
                SystemHealth = new SystemHealthDto
                {
                    IsWorkerRunning = _tracker.IsWorkerRunning,
                    LastAffiliateSync = _tracker.LastAffiliateSyncTime
                }
            };
        }

        public async Task<ApiUsageDto> GetApiUsageAsync(DateTime startDate, DateTime endDate)
        {
            // Ensure end date includes the full day if only date is passed
            if (endDate.TimeOfDay == TimeSpan.Zero)
            {
                endDate = endDate.Date.AddDays(1).AddTicks(-1);
            }

            var logs = (await _unitOfWork.ThirdPartyApiLogs.FindAsync(log => log.CreatedAt >= startDate && log.CreatedAt <= endDate)).ToList();

            // AI Provider calculations (Google Gemini)
            var aiLogs = logs.Where(l => l.ServiceName.Equals("GoogleGemini", StringComparison.OrdinalIgnoreCase)).ToList();
            var aiRequestsCount = aiLogs.Count(l => l.Endpoint.Equals("generateContent", StringComparison.OrdinalIgnoreCase));
            var aiTokensUsed = aiLogs.Sum(l => (long)l.TokensUsed);

            // Fetch pricing config (default $2.50 per 1M tokens)
            if (!decimal.TryParse(_config["AI_COST_PER_MILLION_TOKENS"], out var aiCostPerMillion))
            {
                aiCostPerMillion = 2.50m;
            }
            var estimatedAiCost = (aiTokensUsed / 1000000m) * aiCostPerMillion;

            // Recipe Provider calculations (Spoonacular)
            var recipeLogs = logs.Where(l => l.ServiceName.Equals("Spoonacular", StringComparison.OrdinalIgnoreCase)).ToList();
            var recipeRequestsCount = recipeLogs.Count;

            // Fetch quota config (default 4500 requests per month for free plan)
            if (!double.TryParse(_config["SPOONACULAR_MONTHLY_QUOTA"], out var spoonacularQuota))
            {
                spoonacularQuota = 4500;
            }
            var quotaUsedPercentage = spoonacularQuota > 0 
                ? Math.Round((recipeRequestsCount / spoonacularQuota) * 100, 2) 
                : 0;

            // Fetch cost config (default $0.00468 per request)
            if (!decimal.TryParse(_config["SPOONACULAR_COST_PER_REQUEST"], out var spoonCostPerRequest))
            {
                spoonCostPerRequest = 0.00468m;
            }
            var estimatedRecipeCost = recipeRequestsCount * spoonCostPerRequest;

            // Cache hit rate calculations
            var translationLogs = aiLogs.Where(l => l.Endpoint.Equals("TranslateIngredient", StringComparison.OrdinalIgnoreCase)).ToList();
            var totalTranslations = translationLogs.Count;
            var cacheHitsCount = translationLogs.Count(l => l.WasCacheHit);
            var cacheHitRate = totalTranslations > 0 
                ? Math.Round(((double)cacheHitsCount / totalTranslations) * 100, 2) 
                : 0.0;

            var totalEstimatedCost = estimatedAiCost + estimatedRecipeCost;

            return new ApiUsageDto
            {
                Period = new PeriodDto
                {
                    Start = startDate.ToString("yyyy-MM-dd"),
                    End = endDate.ToString("yyyy-MM-dd")
                },
                TotalEstimatedCostUsd = Math.Round(totalEstimatedCost, 2),
                Details = new ApiUsageDetailsDto
                {
                    AiProvider = new AiProviderUsageDto
                    {
                        Name = "Google Gemini",
                        TotalRequests = aiRequestsCount,
                        TotalTokensUsed = aiTokensUsed,
                        EstimatedCostUsd = Math.Round(estimatedAiCost, 2)
                    },
                    RecipeProvider = new RecipeProviderUsageDto
                    {
                        Name = "Spoonacular",
                        TotalRequests = recipeRequestsCount,
                        QuotaUsedPercentage = quotaUsedPercentage,
                        EstimatedCostUsd = Math.Round(estimatedRecipeCost, 2)
                    }
                },
                CacheEfficiency = new CacheEfficiencyDto
                {
                    AiCacheHitRatePercentage = cacheHitRate
                }
            };
        }
    }
}
