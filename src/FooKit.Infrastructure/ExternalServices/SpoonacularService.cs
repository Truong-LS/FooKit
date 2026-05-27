using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyProject.Application.Configuration;
using MyProject.Application.DTOs.SpoonacularDtos;
using MyProject.Application.Interfaces.IServices;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Data.DBContext;

namespace MyProject.Infrastructure.ExternalServices
{
    public class SpoonacularService : ISpoonacularService
    {
        private readonly HttpClient _httpClient;
        private readonly SpoonacularOptions _options;
        private readonly ILogger<SpoonacularService> _logger;
        private readonly FooKitDbContext _context;

        public SpoonacularService(
            HttpClient httpClient,
            IOptions<SpoonacularOptions> options,
            ILogger<SpoonacularService> logger,
            FooKitDbContext context)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
            _context = context;
        }

        public async Task<List<SpoonacularRecipeDto>> SearchRecipesAsync(string equipment, string diet, int limit = 3)
        {
            try
            {
                var apiKey = _options.ApiKey;
                var baseUrl = _options.BaseUrl.TrimEnd('/');

                // Construct query parameters
                var queryParams = new List<string>
                {
                    $"apiKey={apiKey}",
                    $"number={limit}",
                    "fillIngredients=true",
                    "addRecipeInformation=true"
                };

                if (!string.IsNullOrWhiteSpace(equipment))
                {
                    queryParams.Add($"equipment={Uri.EscapeDataString(equipment)}");
                }

                if (!string.IsNullOrWhiteSpace(diet) && !diet.Equals("None", StringComparison.OrdinalIgnoreCase))
                {
                    var mappedDiet = MapDietToSpoonacular(diet);
                    if (!string.IsNullOrEmpty(mappedDiet))
                    {
                        queryParams.Add($"diet={Uri.EscapeDataString(mappedDiet)}");
                    }
                }

                var requestUrl = $"{baseUrl}/recipes/complexSearch?{string.Join("&", queryParams)}";
                _logger.LogInformation("Calling Spoonacular API search. URL: {Url}", requestUrl.Replace(apiKey, "HIDDEN_KEY"));

                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var apiLog = new ThirdPartyApiLog
                {
                    Id = Guid.NewGuid(),
                    ServiceName = "Spoonacular",
                    Endpoint = "recipes/complexSearch",
                    TokensUsed = 0,
                    WasCacheHit = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ThirdPartyApiLogs.Add(apiLog);
                await _context.SaveChangesAsync();

                var apiResponse = await response.Content.ReadFromJsonAsync<SpoonacularSearchResponse>();
                if (apiResponse?.Results == null)
                {
                    return new List<SpoonacularRecipeDto>();
                }

                var results = new List<SpoonacularRecipeDto>();
                foreach (var recipe in apiResponse.Results)
                {
                    var stepsList = new List<string>();
                    if (recipe.AnalyzedInstructions != null)
                    {
                        foreach (var instruction in recipe.AnalyzedInstructions)
                        {
                            if (instruction.Steps != null)
                            {
                                stepsList.AddRange(instruction.Steps.Select(s => s.StepText));
                            }
                        }
                    }

                    // Extract original ingredients strings (e.g. "2 cups of flour")
                    var rawIngredients = new List<string>();
                    if (recipe.ExtendedIngredients != null)
                    {
                        rawIngredients.AddRange(recipe.ExtendedIngredients
                            .Select(i => i.Original)
                            .Where(o => !string.IsNullOrWhiteSpace(o)));
                    }

                    // Fallback to missed/used if extendedIngredients is empty
                    if (!rawIngredients.Any())
                    {
                        if (recipe.MissedIngredients != null)
                        {
                            rawIngredients.AddRange(recipe.MissedIngredients.Select(i => i.Original));
                        }
                        if (recipe.UsedIngredients != null)
                        {
                            rawIngredients.AddRange(recipe.UsedIngredients.Select(i => i.Original));
                        }
                    }

                    results.Add(new SpoonacularRecipeDto
                    {
                        Title = recipe.Title ?? string.Empty,
                        Image = recipe.Image ?? string.Empty,
                        Instructions = string.Join("\n", stepsList),
                        RawIngredients = rawIngredients.Distinct().ToList(),
                        Diets = recipe.Diets ?? new List<string>(),
                        AnalyzedInstructionsSteps = stepsList
                    });
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling Spoonacular API: {Message}", ex.Message);
                return new List<SpoonacularRecipeDto>();
            }
        }

        private string MapDietToSpoonacular(string diet)
        {
            return diet.ToLower() switch
            {
                "vegan" => "vegan",
                "vegetarian" => "vegetarian",
                "keto" => "ketogenic",
                "eatclean" => "whole30",
                "paleo" => "paleo",
                "glutenfree" => "gluten free",
                "dairyfree" => "dairy free",
                _ => string.Empty
            };
        }

        #region Spoonacular JSON Models
        private class SpoonacularSearchResponse
        {
            [JsonPropertyName("results")]
            public List<SpoonacularRecipeResult>? Results { get; set; }
        }

        private class SpoonacularRecipeResult
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("image")]
            public string? Image { get; set; }

            [JsonPropertyName("diets")]
            public List<string>? Diets { get; set; }

            [JsonPropertyName("extendedIngredients")]
            public List<SpoonacularExtendedIngredient>? ExtendedIngredients { get; set; }

            [JsonPropertyName("missedIngredients")]
            public List<SpoonacularExtendedIngredient>? MissedIngredients { get; set; }

            [JsonPropertyName("usedIngredients")]
            public List<SpoonacularExtendedIngredient>? UsedIngredients { get; set; }

            [JsonPropertyName("analyzedInstructions")]
            public List<SpoonacularInstruction>? AnalyzedInstructions { get; set; }
        }

        private class SpoonacularExtendedIngredient
        {
            [JsonPropertyName("original")]
            public string Original { get; set; } = string.Empty;

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
        }

        private class SpoonacularInstruction
        {
            [JsonPropertyName("steps")]
            public List<SpoonacularStep>? Steps { get; set; }
        }

        private class SpoonacularStep
        {
            [JsonPropertyName("number")]
            public int Number { get; set; }

            [JsonPropertyName("step")]
            public string StepText { get; set; } = string.Empty;
        }
        #endregion
    }
}
