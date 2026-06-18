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
            var cachedSteps = new List<string>();
            if (!string.IsNullOrEmpty(dishCache.InstructionsJson) && dishCache.InstructionsJson != "[]")
            {
                cachedSteps = JsonSerializer.Deserialize<List<string>>(dishCache.InstructionsJson) ?? new List<string>();
            }

            List<string> steps;
            if (cachedSteps.Any())
            {
                _logger.LogInformation("Cache hit for recipe instructions of dish '{DishName}'. Skipping AI call.", dishCache.Name);
                steps = cachedSteps;
            }
            else
            {
                _logger.LogInformation("Cache miss for recipe instructions of dish '{DishName}'. Calling Gemini AI.", dishCache.Name);
                steps = await _aiMatchingService.GenerateRecipeAsync(dishCache.Name, rawIngredients);

                // Save AI result to DishCache for future use
                if (steps.Any())
                {
                    dishCache.InstructionsJson = JsonSerializer.Serialize(steps);
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
            var activeAffiliates = (await _unitOfWork.AffiliateProducts.FindAsync(ap => ap.IsActive)).ToList();

            var dishDto = await DishPricingHelper.CalculateDishPriceAsync(dummyRecipe, mappedIngredientsLookup, allStandardIngredients, activeAffiliates);

            return new DishRecipeDetailDto
            {
                DishCacheId = dishCache.Id,
                DishName = dishCache.Name,
                ImageUrl = dishCache.ImageUrl,
                CookingSteps = steps,
                Ingredients = dishDto.Ingredients,
                TotalCost = dishDto.TotalCost
            };
        }
    }
}
