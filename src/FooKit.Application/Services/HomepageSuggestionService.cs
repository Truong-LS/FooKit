using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyProject.Application.DTOs.DishDtos;
using MyProject.Application.DTOs.HomepageDtos;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Application.Interfaces.IServices;
using MyProject.Domain.Entities;
using MyProject.Domain.Enums;

namespace MyProject.Application.Services
{
    public class HomepageSuggestionService : IHomepageSuggestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpoonacularService _spoonacularService;
        private readonly IAiMatchingService _aiMatchingService;
        private readonly ILogger<HomepageSuggestionService> _logger;

        public HomepageSuggestionService(
            IUnitOfWork unitOfWork,
            ISpoonacularService spoonacularService,
            IAiMatchingService aiMatchingService,
            ILogger<HomepageSuggestionService> logger)
        {
            _unitOfWork = unitOfWork;
            _spoonacularService = spoonacularService;
            _aiMatchingService = aiMatchingService;
            _logger = logger;
        }

        public async Task<HomepageSuggestionResponseDto> GetDailySuggestionsAsync(Guid userId)
        {
            var response = new HomepageSuggestionResponseDto();

            // 1. Premium Validation
            var activeSub = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionAsync(userId);
            var isPremium = activeSub != null && activeSub.IsActive;
            if (!isPremium)
            {
                response.IsPremiumExpired = true;
            }

            // 2. Cache Checking (Step 5A)
            var today = DateTime.UtcNow.Date;
            var cache = (await _unitOfWork.UserHomepageCaches.FindAsync(c => c.UserId == userId && c.ExpirationTime > DateTime.UtcNow)).FirstOrDefault();
            if (cache != null)
            {
                _logger.LogInformation("Cache hit for user {UserId}. Returning serialized menu data.", userId);
                return JsonSerializer.Deserialize<HomepageSuggestionResponseDto>(cache.SerializedMenuData) ?? response;
            }

            // 3. User Profile Fetching
            var userTools = (await _unitOfWork.Users.GetByIdAsync(userId))?.Tools?.Select(t => t.ToolName).ToList() ?? new List<string>();
            var allergies = (await _unitOfWork.UserAllergies.FindAsync(a => a.UserId == userId)).Select(a => a.AllergenName).ToList();

            // 4. Cold Start Handling
            if (!userTools.Any())
            {
                _logger.LogInformation("Cold start for user {UserId}. Falling back to default equipment and popular dishes.", userId);
                userTools.Add("Stove/Pan");
                var popularDishes = (await _unitOfWork.DishCaches.GetAllAsync()).Take(5).ToList();
                
                foreach (var dish in popularDishes)
                {
                    var dto = new SuggestedDishDto { DishName = dish.Name, ImageUrl = dish.ImageUrl };
                    response.Dinner.Add(dto); // Defaulting to dinner for cold start
                }
                
                await SaveCacheAsync(userId, response);
                return response;
            }

            // 5. External Integration (Step 5B) - Simplified for planning artifact structure
            // In a real implementation, we would call Spoonacular, filter by History/Allergies, and use AI to map ingredients
            _logger.LogInformation("Cache miss for user {UserId}. Calling external APIs.", userId);
            
            // Mocking Spoonacular Call
            var equipment = string.Join(",", userTools);
            var recipes = await _spoonacularService.SearchRecipesAsync(equipment, string.Empty, limit: 9);
            
            if (recipes != null && recipes.Any())
            {
                var timeOfDay = DateTime.UtcNow.Hour; // Very basic contextual sorting

                foreach (var recipe in recipes)
                {
                    var dishDto = new SuggestedDishDto { DishName = recipe.Title, ImageUrl = recipe.Image };
                    
                    // Contextual Sorting Logic
                    if (timeOfDay < 11) response.Breakfast.Add(dishDto);
                    else if (timeOfDay < 16) response.Lunch.Add(dishDto);
                    else response.Dinner.Add(dishDto);
                }
            }

            // Save to Cache
            await SaveCacheAsync(userId, response);
            return response;
        }

        private async Task SaveCacheAsync(Guid userId, HomepageSuggestionResponseDto response)
        {
            var newCache = new UserHomepageCache
            {
                UserId = userId,
                SerializedMenuData = JsonSerializer.Serialize(response),
                ExpirationTime = DateTime.UtcNow.AddHours(24)
            };
            await _unitOfWork.UserHomepageCaches.AddAsync(newCache);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
