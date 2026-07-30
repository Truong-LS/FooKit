using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FooKit.Application.DTOs.DishDtos;
using FooKit.Application.Helpers;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;

namespace FooKit.Application.Services
{
    public class DishRecipeService : IDishRecipeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAiMatchingService _aiMatchingService;
        private readonly ILogger<DishRecipeService> _logger;

        public DishRecipeService(
            IUnitOfWork unitOfWork,
            IAiMatchingService aiMatchingService,
            ILogger<DishRecipeService> logger)
        {
            _unitOfWork = unitOfWork;
            _aiMatchingService = aiMatchingService;
            _logger = logger;
        }

        public async Task<DishRecipeDetailDto> GetRecipeDetailAsync(Guid dishCacheId)
        {
            var dishCache = (await _unitOfWork.DishCaches.FindAsync(dc => dc.Id == dishCacheId)).FirstOrDefault();
            if (dishCache == null)
            {
                throw new NotFoundException("Món ăn không được tìm thấy.");
            }

            var rawIngredients = string.IsNullOrEmpty(dishCache.RawIngredientsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(dishCache.RawIngredientsJson) ?? new List<string>();

            // Check cache first: if InstructionsJson already has data, skip AI call
            var cachedRecipe = new AiGeneratedRecipeDto();
            if (!string.IsNullOrEmpty(dishCache.InstructionsJson) && dishCache.InstructionsJson != "[]")
            {
                try
                {
                    // For backward compatibility, check if it's an old cache (array of strings)
                    if (dishCache.InstructionsJson.TrimStart().StartsWith("["))
                    {
                        var oldSteps = JsonSerializer.Deserialize<List<string>>(dishCache.InstructionsJson);
                        cachedRecipe.Steps = oldSteps ?? new List<string>();
                        // Old format only has steps - treat as cache miss so AI regenerates full data
                        // but keep the old steps as a fallback
                        _logger.LogInformation("Old format cache detected for dish '{DishName}'. Will regenerate full recipe data.", dishCache.Name);
                    }
                    else
                    {
                        cachedRecipe = JsonSerializer.Deserialize<AiGeneratedRecipeDto>(dishCache.InstructionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new AiGeneratedRecipeDto();
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize InstructionsJson for dish '{DishName}'. Treating as cache miss. Raw value: {RawValue}", dishCache.Name, dishCache.InstructionsJson);
                    cachedRecipe = new AiGeneratedRecipeDto();
                }
            }

            AiGeneratedRecipeDto recipeData;
            if (cachedRecipe.Steps != null && cachedRecipe.Steps.Any())
            {
                _logger.LogInformation("Cache hit for recipe instructions of dish '{DishName}'. Skipping AI call.", dishCache.Name);
                recipeData = cachedRecipe;
            }
            else
            {
                _logger.LogInformation("Cache miss for recipe instructions of dish '{DishName}'. Calling Gemini AI.", dishCache.Name);
                recipeData = await _aiMatchingService.GenerateRecipeAsync(dishCache.Name, rawIngredients);

                // Save AI result to DishCache for future use
                if (recipeData.Steps != null && recipeData.Steps.Any())
                {
                    dishCache.InstructionsJson = JsonSerializer.Serialize(recipeData);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Cached AI-generated recipe for dish '{DishName}'.", dishCache.Name);
                }
            }

            // Calculate ingredient pricing
            var dummyRecipe = new FooKit.Application.DTOs.SpoonacularDtos.SpoonacularRecipeDto
            {
                SpoonacularId = dishCache.SpoonacularId,
                Title = dishCache.Name,
                Image = dishCache.ImageUrl,
                RawIngredients = rawIngredients
            };

            var mappedIngredientsLookup = await DishPricingHelper.GetOrMatchIngredientsAsync(_unitOfWork, _aiMatchingService, _logger, rawIngredients);
            var allStandardIngredients = (await _unitOfWork.StandardIngredients.GetAllAsync()).ToDictionary(si => si.Id, si => si);
            // var activeAffiliates = (await _unitOfWork.AffiliateProducts.FindAsync(ap => ap.IsActive)).ToList();

            var dishDto = await DishPricingHelper.CalculateDishPriceFromDbAsync(dummyRecipe, mappedIngredientsLookup, allStandardIngredients);

            return new DishRecipeDetailDto
            {
                DishCacheId = dishCache.Id.ToString(),
                DishName = dishCache.Name,
                ImageUrl = dishCache.ImageUrl,
                Description = string.IsNullOrWhiteSpace(recipeData.Description) ? "Món ăn hấp dẫn, dễ thực hiện." : recipeData.Description,
                CookingTimeMinutes = recipeData.CookingTimeMinutes > 0
                    ? recipeData.CookingTimeMinutes
                    : (dishCache.ReadyInMinutes > 0 ? dishCache.ReadyInMinutes : 30),
                Servings = recipeData.Servings > 0
                    ? recipeData.Servings
                    : (dishCache.Servings > 0 ? dishCache.Servings : 2),
                Calories = recipeData.Calories > 0
                    ? recipeData.Calories
                    : (dishCache.Calories > 0 ? dishCache.Calories : 350),
                Difficulty = string.IsNullOrWhiteSpace(recipeData.Difficulty) ? "Dễ" : recipeData.Difficulty,
                Categories = (recipeData.Categories != null && recipeData.Categories.Any()) ? recipeData.Categories : new List<string> { "Món Việt", "Bữa chính" },
                Tools = (recipeData.Tools != null && recipeData.Tools.Any()) ? recipeData.Tools : new List<string> { "Nồi", "Chảo", "Dao", "Thớt" },
                Nutrition = new NutritionDto
                {
                    Protein = recipeData.Nutrition?.Protein ?? 25,
                    Carbs = recipeData.Nutrition?.Carbs ?? 40,
                    Fat = recipeData.Nutrition?.Fat ?? 12,
                    Fiber = recipeData.Nutrition?.Fiber ?? 5
                },
                CookingSteps = recipeData.Steps ?? new List<string>(),
                Ingredients = dishDto.Ingredients.Select(i => 
                {
                    decimal qty = 0;
                    string unit = "none";
                    
                    if (recipeData.IngredientQuantities != null)
                    {
                        var aiIngredientInfo = recipeData.IngredientQuantities.FirstOrDefault(iq => 
                            iq.Key.Equals(i.RawEnglishName, StringComparison.OrdinalIgnoreCase) || 
                            i.RawEnglishName.Contains(iq.Key, StringComparison.OrdinalIgnoreCase) ||
                            iq.Key.Contains(i.StandardIngredientName, StringComparison.OrdinalIgnoreCase));
                            
                        if (aiIngredientInfo.Key != null && aiIngredientInfo.Value != null)
                        {
                            qty = aiIngredientInfo.Value.Quantity;
                            unit = aiIngredientInfo.Value.Unit;
                        }
                    }

                    return new DishRecipeIngredientDto
                    {
                        RawIngredientName = i.RawEnglishName,
                        StandardIngredientId = mappedIngredientsLookup.TryGetValue(i.RawEnglishName, out var id) && id.HasValue ? id.Value.ToString() : string.Empty,
                        StandardIngredientName = i.StandardIngredientName,
                        Quantity = qty,
                        Unit = unit,
                        IsMatched = i.IsMapped,
                        IsPriced = i.EstimatedPrice > 0,
                        AffiliateUrl = string.Empty,
                        EstimatedPrice = i.EstimatedPrice
                    };
                }).ToList(),
                TotalCost = dishDto.TotalCost
            };
        }
    }
}
