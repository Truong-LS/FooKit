using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyProject.Application.DTOs.DishDtos;
using MyProject.Application.DTOs.IngredientDtos;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Application.Interfaces.IServices;
using MyProject.Domain.Entities;
using MyProject.Domain.Enums;
using MyProject.Domain.ValueObjects;

namespace MyProject.Application.Services
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

            // Step 2: Fetch recipes from Spoonacular
            var recipes = await _spoonacularService.SearchRecipesAsync(request.Equipment, request.Diet.ToString(), limit: 3);
            if (recipes == null || !recipes.Any())
            {
                _logger.LogWarning("No recipes found from Spoonacular API for equipment: {Equipment}, diet: {Diet}", request.Equipment, request.Diet);
                return new DishSuggestionResponseDto();
            }

            // Collect all unique raw ingredients across all recipes
            var allRawIngredients = recipes.SelectMany(r => r.RawIngredients).Distinct().ToList();

            // Step 3: AI entity matching and dictionary caching
            var mappedIngredientsLookup = await GetOrMatchIngredientsAsync(allRawIngredients);

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
                    var suggestedIngredients = new List<SuggestedDishIngredientDto>();
                    decimal recipeTotalCost = 0;

                    foreach (var rawIng in recipe.RawIngredients)
                    {
                        var ingredientDto = new SuggestedDishIngredientDto
                        {
                            RawEnglishName = rawIng,
                            IsMapped = false,
                            StandardIngredientName = "Khác",
                            AffiliateProduct = null
                        };

                        if (mappedIngredientsLookup.TryGetValue(rawIng, out var standardId) && standardId.HasValue)
                        {
                            var stdId = standardId.Value;
                            ingredientDto.IsMapped = true;

                            if (allStandardIngredients.TryGetValue(stdId, out var standardIng))
                            {
                                ingredientDto.StandardIngredientName = standardIng.Name;
                            }

                            // Find cheapest active affiliate product for this standard ingredient
                            var cheapestAffiliate = activeAffiliates
                                .Where(ap => ap.StandardIngredientId == stdId)
                                .OrderBy(ap => ap.CurrentPrice.Amount)
                                .FirstOrDefault();

                            if (cheapestAffiliate != null)
                            {
                                ingredientDto.AffiliateProduct = new SuggestedAffiliateProductDto
                                {
                                    ProductId = cheapestAffiliate.Id,
                                    ProductName = cheapestAffiliate.ProductName,
                                    ProductUrl = cheapestAffiliate.ProductUrl,
                                    Price = cheapestAffiliate.CurrentPrice.Amount,
                                    Platform = cheapestAffiliate.Platform
                                };
                                recipeTotalCost += cheapestAffiliate.CurrentPrice.Amount;
                            }
                        }

                        suggestedIngredients.Add(ingredientDto);
                    }

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

                    suggestedDishes.Add(new SuggestedDishDto
                    {
                        DishName = recipe.Title,
                        ImageUrl = recipe.Image,
                        Instructions = recipe.Instructions,
                        TotalCost = recipeTotalCost,
                        Ingredients = suggestedIngredients
                    });
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

        private async Task<Dictionary<string, Guid?>> GetOrMatchIngredientsAsync(List<string> rawIngredients)
        {
            var lookup = new Dictionary<string, Guid?>(StringComparer.OrdinalIgnoreCase);

            // Fetch already cached dictionaries from DB
            var existingDictionaries = (await _unitOfWork.IngredientDictionaries.GetAllAsync())
                .ToDictionary(id => id.RawKeywordFromApi, id => id.StandardIngredientId, StringComparer.OrdinalIgnoreCase);

            var uncachedRawIngredients = new List<string>();

            foreach (var rawIng in rawIngredients)
            {
                if (existingDictionaries.TryGetValue(rawIng, out var standardId))
                {
                    lookup[rawIng] = standardId;
                }
                else
                {
                    uncachedRawIngredients.Add(rawIng);
                }
            }

            if (uncachedRawIngredients.Any())
            {
                _logger.LogInformation("Found {Count} uncached ingredients. Fetching standard ingredients to prompt AI...", uncachedRawIngredients.Count);

                // Fetch standard ingredients
                var standardIngredients = await _unitOfWork.StandardIngredients.GetAllAsync();
                var standardDtos = standardIngredients.Select(si => new StandardIngredientDto
                {
                    Id = si.Id,
                    Name = si.Name,
                    Category = si.Category.ToString()
                }).ToList();

                // Call AI matching
                var aiMatches = await _aiMatchingService.MatchIngredientsAsync(uncachedRawIngredients, standardDtos);

                // Save new matches to DB cache
                foreach (var match in aiMatches)
                {
                    lookup[match.Key] = match.Value;

                    if (match.Value.HasValue)
                    {
                        var newDict = new IngredientDictionary
                        {
                            Id = Guid.NewGuid(),
                            RawKeywordFromApi = match.Key,
                            StandardIngredientId = match.Value.Value
                        };
                        await _unitOfWork.IngredientDictionaries.AddAsync(newDict);
                    }
                }

                if (aiMatches.Any(m => m.Value.HasValue))
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Successfully saved new mapped ingredient dictionaries to DB cache.");
                }
            }

            return lookup;
        }
    }
}
