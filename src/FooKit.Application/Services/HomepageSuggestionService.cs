using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FooKit.Application.DTOs.DishDtos;
using FooKit.Application.DTOs.HomepageDtos;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;
using FooKit.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace FooKit.Application.Services
{
    public class HomepageSuggestionService : IHomepageSuggestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpoonacularService _spoonacularService;
        private readonly IAiMatchingService _aiMatchingService;
        private readonly ILogger<HomepageSuggestionService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHomepageCacheSignal _cacheSignal;

        public HomepageSuggestionService(
            IUnitOfWork unitOfWork,
            ISpoonacularService spoonacularService,
            IAiMatchingService aiMatchingService,
            ILogger<HomepageSuggestionService> logger,
            IMemoryCache memoryCache,
            IHomepageCacheSignal cacheSignal)
        {
            _unitOfWork = unitOfWork;
            _spoonacularService = spoonacularService;
            _aiMatchingService = aiMatchingService;
            _logger = logger;
            _memoryCache = memoryCache;
            _cacheSignal = cacheSignal;
        }

        public async Task<MealSuggestionResponseDto> GetMealSuggestionsAsync(Guid userId, string mealType)
        {
            var response = new MealSuggestionResponseDto();

            // 1. Premium Validation
            var activeSub = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionAsync(userId);
            var isPremium = activeSub != null && activeSub.IsActive;
            if (!isPremium)
            {
                response.IsPremiumExpired = true;
            }

            // 2. Cache Checking (Step 5A)
            var cacheKey = $"HomepageCache:User_{userId}_{mealType}";
            if (_memoryCache.TryGetValue(cacheKey, out string cachedData))
            {
                _logger.LogInformation("Cache hit for user {UserId} and meal {MealType}. Returning serialized menu data.", userId, mealType);
                return JsonSerializer.Deserialize<MealSuggestionResponseDto>(cachedData) ?? response;
            }

            // 3. User Profile Fetching
            var userTools = (await _unitOfWork.Users.GetByIdAsync(userId))?.Tools?.Select(t => t.ToolName).ToList() ?? new List<string>();
            var allergies = (await _unitOfWork.UserAllergies.FindAsync(a => a.UserId == userId)).Select(a => a.AllergenName).ToList();
            var cuisines = (await _unitOfWork.UserFavoriteCuisines.FindAsync(c => c.UserId == userId)).Select(c => c.CuisineName).ToList();

            // 4. Cold Start Handling
            if (!userTools.Any())
            {
                _logger.LogInformation("Cold start for user {UserId}. Falling back to default equipment and popular dishes.", userId);
                userTools.Add("Stove/Pan");
                
                var allPopular = await _unitOfWork.DishCaches.GetAllAsync();
                var skipCount = mealType.ToLower() switch
                {
                    "breakfast" => 0,
                    "lunch" => 5,
                    "dinner" => 10,
                    _ => 0
                };
                
                var popularDishes = allPopular.Skip(skipCount).Take(5).ToList();
                if (!popularDishes.Any())
                {
                    popularDishes = allPopular.Take(5).ToList(); // Fallback if not enough dishes
                }
                
                foreach (var dish in popularDishes)
                {
                    var dto = new SuggestedDishDto { DishName = dish.Name, ImageUrl = dish.ImageUrl };
                    response.Dishes.Add(dto);
                }
                
                await SaveCacheAsync(userId, mealType, response);
                return response;
            }

            // 5. External Integration (Step 5B)
            // Using Spoonacular with User Dietary Profile (Tools, Allergies, Cuisines)
            _logger.LogInformation("Cache miss for user {UserId} meal {MealType}. Calling external APIs with dietary profile.", userId, mealType);
            
            var equipment = string.Join(",", userTools);
            var intolerances = string.Join(",", allergies);
            var cuisineParam = string.Join(",", cuisines);
            
            var recipes = await _spoonacularService.SearchRecipesAsync(equipment, string.Empty, intolerances, cuisineParam, mealType, limit: 5);
            
            if (recipes != null && recipes.Any())
            {
                foreach (var recipe in recipes)
                {
                    var dishDto = new SuggestedDishDto { DishName = recipe.Title, ImageUrl = recipe.Image };
                    response.Dishes.Add(dishDto);
                }
            }

            // Save to Cache
            await SaveCacheAsync(userId, mealType, response);
            return response;
        }

        private Task SaveCacheAsync(Guid userId, string mealType, MealSuggestionResponseDto response)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            options.AddExpirationToken(_cacheSignal.GetToken());

            _memoryCache.Set($"HomepageCache:User_{userId}_{mealType}", JsonSerializer.Serialize(response), options);
            return Task.CompletedTask;
        }
    }
}
