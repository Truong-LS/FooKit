using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FooKit.Application.DTOs.DishDtos;
using FooKit.Application.DTOs.IngredientDtos;
using FooKit.Application.DTOs.SpoonacularDtos;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;

namespace FooKit.Application.Helpers
{
    public static class DishPricingHelper
    {
        public static async Task<Dictionary<string, Guid?>> GetOrMatchIngredientsAsync(
            IUnitOfWork unitOfWork,
            IAiMatchingService aiMatchingService,
            ILogger logger,
            List<string> rawIngredients)
        {
            var lookup = new Dictionary<string, Guid?>(StringComparer.OrdinalIgnoreCase);

            var existingDictionaries = (await unitOfWork.IngredientDictionaries.GetAllAsync())
                .ToDictionary(id => id.RawKeywordFromApi, id => id.StandardIngredientId, StringComparer.OrdinalIgnoreCase);

            var uncachedRawIngredients = new List<string>();
            var logs = new List<ThirdPartyApiLog>();

            foreach (var rawIng in rawIngredients)
            {
                if (existingDictionaries.TryGetValue(rawIng, out var standardId))
                {
                    lookup[rawIng] = standardId;
                    logs.Add(new ThirdPartyApiLog
                    {
                        Id = Guid.NewGuid(),
                        ServiceName = "GoogleGemini",
                        Endpoint = "TranslateIngredient",
                        TokensUsed = 0,
                        WasCacheHit = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    uncachedRawIngredients.Add(rawIng);
                    logs.Add(new ThirdPartyApiLog
                    {
                        Id = Guid.NewGuid(),
                        ServiceName = "GoogleGemini",
                        Endpoint = "TranslateIngredient",
                        TokensUsed = 0,
                        WasCacheHit = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (logs.Any())
            {
                await unitOfWork.ThirdPartyApiLogs.AddRangeAsync(logs);
            }

            if (uncachedRawIngredients.Any())
            {
                logger.LogInformation("Found {Count} uncached ingredients. Fetching standard ingredients to prompt AI...", uncachedRawIngredients.Count);

                var standardIngredients = await unitOfWork.StandardIngredients.GetAllAsync();
                var standardDtos = standardIngredients.Select(si => new StandardIngredientDto
                {
                    Id = si.Id,
                    Name = si.Name,
                    Category = si.Category.ToString()
                }).ToList();

                var aiMatches = await aiMatchingService.MatchIngredientsAsync(uncachedRawIngredients, standardDtos);

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
                        await unitOfWork.IngredientDictionaries.AddAsync(newDict);
                    }
                }
            }

            if (logs.Any())
            {
                await unitOfWork.SaveChangesAsync();
            }

            return lookup;
        }

        public static async Task<SuggestedDishDto> CalculateDishPriceAsync(
            SpoonacularRecipeDto recipe,
            Dictionary<string, Guid?> mappedIngredientsLookup,
            Dictionary<Guid, StandardIngredient> allStandardIngredients,
            List<AffiliateProduct> activeAffiliates)
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

            // Cooking time
            var cookingTime = recipe.ReadyInMinutes > 0 ? recipe.ReadyInMinutes : 30;

            // Calories
            var calories = recipe.Calories > 0 ? recipe.Calories : 350;

            // Difficulty
            var difficulty = cookingTime switch
            {
                <= 15 => "Rất dễ",
                <= 30 => "Dễ",
                <= 60 => "Trung bình",
                _ => "Khó"
            };

            // Servings
            var servings = recipe.Servings > 0 ? recipe.Servings : 2;

            // Categories
            var categories = new List<string>();
            if (recipe.Diets != null && recipe.Diets.Any())
            {
                categories.AddRange(recipe.Diets.Take(2).Select(d => d.ToLower() switch
                {
                    "vegan" => "Thuần chay",
                    "vegetarian" => "Chay",
                    "gluten free" => "Không gluten",
                    "dairy free" => "Không sữa",
                    "ketogenic" => "Keto",
                    "paleo" => "Paleo",
                    "whole30" => "Eat Clean",
                    _ => char.ToUpper(d[0]) + d.Substring(1)
                }));
            }
            if (!categories.Any()) categories.Add("Món Âu");

            return new SuggestedDishDto
            {
                DishName = recipe.Title,
                ImageUrl = recipe.Image,
                CookingTimeMinutes = cookingTime,
                Calories = calories,
                Difficulty = difficulty,
                Servings = servings,
                TotalCost = recipeTotalCost,
                Categories = categories,
                Instructions = recipe.Instructions,
                Ingredients = suggestedIngredients
            };
        }

        public static Task<SuggestedDishDto> CalculateDishPriceFromDbAsync(
            SpoonacularRecipeDto recipe,
            Dictionary<string, Guid?> mappedIngredientsLookup,
            Dictionary<Guid, StandardIngredient> allStandardIngredients)
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
                    AffiliateProduct = null,
                    EstimatedPrice = 0
                };

                if (mappedIngredientsLookup.TryGetValue(rawIng, out var standardId) && standardId.HasValue)
                {
                    var stdId = standardId.Value;
                    ingredientDto.IsMapped = true;

                    if (allStandardIngredients.TryGetValue(stdId, out var standardIng))
                    {
                        ingredientDto.StandardIngredientName = standardIng.Name;
                        var effectivePrice = standardIng.EstimatedUses > 1
                            ? Math.Round(standardIng.DefaultPrice / standardIng.EstimatedUses, 0)
                            : standardIng.DefaultPrice;
                        ingredientDto.EstimatedPrice = effectivePrice;
                        recipeTotalCost += effectivePrice;
                    }
                }

                suggestedIngredients.Add(ingredientDto);
            }

            // Cooking time
            var cookingTime = recipe.ReadyInMinutes > 0 ? recipe.ReadyInMinutes : 30;

            // Calories
            var calories = recipe.Calories > 0 ? recipe.Calories : 350;

            // Difficulty
            var difficulty = cookingTime switch
            {
                <= 15 => "Rất dễ",
                <= 30 => "Dễ",
                <= 60 => "Trung bình",
                _ => "Khó"
            };

            // Servings
            var servings = recipe.Servings > 0 ? recipe.Servings : 2;

            // Categories
            var categories = new List<string>();
            if (recipe.Diets != null && recipe.Diets.Any())
            {
                categories.AddRange(recipe.Diets.Take(2).Select(d => d.ToLower() switch
                {
                    "vegan" => "Thuần chay",
                    "vegetarian" => "Chay",
                    "gluten free" => "Không gluten",
                    "dairy free" => "Không sữa",
                    "ketogenic" => "Keto",
                    "paleo" => "Paleo",
                    "whole30" => "Eat Clean",
                    _ => char.ToUpper(d[0]) + d.Substring(1)
                }));
            }
            if (!categories.Any()) categories.Add("Món Âu");

            return Task.FromResult(new SuggestedDishDto
            {
                DishName = recipe.Title,
                ImageUrl = recipe.Image,
                CookingTimeMinutes = cookingTime,
                Calories = calories,
                Difficulty = difficulty,
                Servings = servings,
                TotalCost = recipeTotalCost,
                Categories = categories,
                Instructions = recipe.Instructions,
                Ingredients = suggestedIngredients
            });
        }
    }
}
