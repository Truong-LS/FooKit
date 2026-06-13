using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FooKit.Application.DTOs.DishDtos;
using FooKit.Application.DTOs.IngredientDtos;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;
using FooKit.Domain.Enums;
using FooKit.Domain.ValueObjects;

using FooKit.Application.Helpers;

namespace FooKit.Application.Services
{
    public class DishSuggestionService : IDishSuggestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpoonacularService _spoonacularService;
        private readonly IAiMatchingService _aiMatchingService;
        private readonly ILogger<DishSuggestionService> _logger;

        public DishSuggestionService(
            IUnitOfWork unitOfWork,
            ISpoonacularService spoonacularService,
            IAiMatchingService aiMatchingService,
            ILogger<DishSuggestionService> logger)
        {
            _unitOfWork = unitOfWork;
            _spoonacularService = spoonacularService;
            _aiMatchingService = aiMatchingService;
            _logger = logger;
        }

        public async Task<DishSuggestionResponseDto> GetSuggestionsAsync(Guid userId, DishSuggestionRequestDto request)
        {
            _logger.LogInformation("Processing GetSuggestionsAsync for User: {UserId}, Equipment: {Equipment}, Diet: {Diet}, Budget: {Budget}",
                userId, request.Equipment, request.Diet, request.Budget);

            // Step 1: Verify Premium status if a special dietary preference is chosen
            var isSpecialDiet = request.Diet != DietaryType.None;
            if (isSpecialDiet)
            {
                var activeSub = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionAsync(userId);
                var isPremium = activeSub != null && activeSub.IsActive; // Check premium subscription
                
                if (!isPremium)
                {
                    _logger.LogWarning("User {UserId} attempted to search special diet {Diet} but does not have a premium subscription.", userId, request.Diet);
                    throw new UnauthorizedAccessException("Bạn cần nâng cấp gói tài khoản Premium để sử dụng các chế độ ăn đặc biệt như Keto, Vegan, v.v.");
                }
            }

            // Step 1.5: Fetch User Profile for Fallbacks and Additional Dietary Info
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            var userTools = user?.Tools?.Select(t => t.ToolName).ToList() ?? new List<string>();
            var allergies = (await _unitOfWork.UserAllergies.FindAsync(a => a.UserId == userId)).Select(a => a.AllergenName).ToList();
            var cuisines = (await _unitOfWork.UserFavoriteCuisines.FindAsync(c => c.UserId == userId)).Select(c => c.CuisineName).ToList();

            var finalEquipment = !string.IsNullOrWhiteSpace(request.Equipment) 
                ? request.Equipment 
                : (userTools.Any() ? string.Join(",", userTools) : "Stove/Pan");

            var finalIntolerances = string.Join(",", allergies);
            var finalCuisines = string.Join(",", cuisines);

            // Step 1.8: Determine Meal Type based on current time (UTC+7)
            var currentHour = DateTime.UtcNow.AddHours(7);
            string currentMealType = "dinner";
            if (currentHour.Hour >= 5 && currentHour.Hour < 10) currentMealType = "breakfast";
            else if (currentHour.Hour >= 10 && currentHour.Hour < 14) currentMealType = "lunch";

            var seed = currentHour.Year * 10000 + currentHour.Month * 100 + currentHour.Day + currentHour.Hour;
            var offset = new Random(seed).Next(0, 30);

            // Step 2: Fetch recipes from Spoonacular
            var recipes = await _spoonacularService.SearchRecipesAsync(
                equipment: finalEquipment, 
                diet: request.Diet.ToString(), 
                intolerances: finalIntolerances, 
                cuisine: finalCuisines, 
                mealType: currentMealType, 
                limit: 5,
                offset: offset);
                
            if (recipes == null || !recipes.Any())
            {
                _logger.LogWarning("No recipes found from Spoonacular API for equipment: {Equipment}, diet: {Diet}", finalEquipment, request.Diet);
                return new DishSuggestionResponseDto();
            }

            // Collect all unique raw ingredients across all recipes
            var allRawIngredients = recipes.SelectMany(r => r.RawIngredients).Distinct().ToList();

            // Step 3: AI entity matching and dictionary caching
            var mappedIngredientsLookup = await DishPricingHelper.GetOrMatchIngredientsAsync(_unitOfWork, _aiMatchingService, _logger, allRawIngredients);

            // Fetch standard ingredients for naming reference
            var allStandardIngredients = (await _unitOfWork.StandardIngredients.GetAllAsync()).ToDictionary(si => si.Id, si => si);

            // Fetch active affiliate products
            var activeAffiliates = (await _unitOfWork.AffiliateProducts.FindAsync(ap => ap.IsActive)).ToList();

            // Step 4: Budget Calculation & Affiliate Link Binding
            var suggestedDishes = new List<SuggestedDishDto>();

            // Begin db transaction using execution strategy
            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                // Create Suggestion Request Log
                var suggestionRequest = new SuggestionRequest
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TargetBudget = new Money(request.Budget, "VND"),
                    DietaryRequirement = request.Diet,
                    AvailableToolsJson = JsonSerializer.Serialize(new List<string> { request.Equipment }),
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.SuggestionRequests.AddAsync(suggestionRequest);

                foreach (var recipe in recipes)
                {
                    var dishDto = await DishPricingHelper.CalculateDishPriceAsync(recipe, mappedIngredientsLookup, allStandardIngredients, activeAffiliates);
                    var recipeTotalCost = dishDto.TotalCost;
                    var suggestedIngredients = dishDto.Ingredients;

                    // Check Budget limit: If exceeds budget, exclude dish
                    if (recipeTotalCost > request.Budget)
                    {
                        _logger.LogInformation("Excluding recipe '{Title}' as its total ingredient cost ({Cost} VND) exceeds the user budget ({Budget} VND).",
                            recipe.Title, recipeTotalCost, request.Budget);
                        continue;
                    }

                    // Retrieve or insert DishCache
                    var externalId = recipe.Title.GetHashCode().ToString(); // Fallback identifier since Spoonacular recipe doesn't store direct ID in DTO
                    var dishCache = (await _unitOfWork.DishCaches.FindAsync(dc => dc.ExternalApiId == externalId)).FirstOrDefault();

                    if (dishCache == null)
                    {
                        dishCache = new DishCache
                        {
                            Id = Guid.NewGuid(),
                            ExternalApiId = externalId,
                            Name = recipe.Title,
                            ImageUrl = recipe.Image,
                            DietaryTagsJson = JsonSerializer.Serialize(recipe.Diets),
                            RequiredToolsJson = JsonSerializer.Serialize(new List<string> { request.Equipment }),
                            RawIngredientsJson = JsonSerializer.Serialize(recipe.RawIngredients),
                            LastFetchedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.DishCaches.AddAsync(dishCache);
                    }

                    // Log Suggestion Result
                    var suggestionResult = new SuggestionResult
                    {
                        Id = Guid.NewGuid(),
                        SuggestionRequestId = suggestionRequest.Id,
                        DishCacheId = dishCache.Id,
                        TotalEstimatedPrice = new Money(recipeTotalCost, "VND"),
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.SuggestionResults.AddAsync(suggestionResult);

                    suggestedDishes.Add(dishDto);
                }

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction failed while logging suggestions, rolling back.");
                throw;
            }

            return new DishSuggestionResponseDto
            {
                SuggestedDishes = suggestedDishes
            };
        }
    }
}
