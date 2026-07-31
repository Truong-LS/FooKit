using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FooKit.Application.Configuration;
using FooKit.Application.DTOs.DishDtos;
using FooKit.Application.DTOs.IngredientDtos;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;
using FooKit.Infrastructure.Data.DBContext;

namespace FooKit.Infrastructure.ExternalServices
{
    public class GeminiMatchingService : IAiMatchingService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiOptions _options;
        private readonly ILogger<GeminiMatchingService> _logger;
        private readonly FooKitDbContext _context;

        public GeminiMatchingService(
            HttpClient httpClient,
            IOptions<GeminiOptions> options,
            ILogger<GeminiMatchingService> _log,
            FooKitDbContext context)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = _log;
            _context = context;
        }

        public async Task<Dictionary<string, Guid?>> MatchIngredientsAsync(
            List<string> rawEnglishIngredients,
            List<StandardIngredientDto> standardIngredients)
        {
            var resultDict = new Dictionary<string, Guid?>();

            if (rawEnglishIngredients == null || !rawEnglishIngredients.Any())
            {
                return resultDict;
            }

            // Initialize all entries as null first
            foreach (var item in rawEnglishIngredients)
            {
                resultDict[item] = null;
            }

            if (standardIngredients == null || !standardIngredients.Any())
            {
                return resultDict;
            }

            try
            {
                var apiKey = _options.ApiKey;
                var baseUrl = _options.BaseUrl.TrimEnd('/');
                var model = _options.Model;

                var requestUrl = $"{baseUrl}/v1beta/models/{model}:generateContent?key={apiKey}";

                // Build prompt
                var promptBuilder = new StringBuilder();
                promptBuilder.AppendLine("You are an expert culinary entity matching system.");
                promptBuilder.AppendLine("Match each raw English ingredient from the provided list to the most appropriate Standard Ingredient from the Vietnamese database list.");
                promptBuilder.AppendLine("If there is no logical match (e.g. extremely different ingredients), map standardIngredientId to null.");
                promptBuilder.AppendLine();

                promptBuilder.AppendLine("Input list of English raw ingredients to match:");
                foreach (var raw in rawEnglishIngredients)
                {
                    promptBuilder.AppendLine($"- {raw}");
                }
                promptBuilder.AppendLine();

                promptBuilder.AppendLine("Standard Vietnamese Database ingredients available (Guid, Name, Category):");
                foreach (var std in standardIngredients)
                {
                    promptBuilder.AppendLine($"- {std.Id} | {std.Name} | {std.Category}");
                }
                promptBuilder.AppendLine();

                promptBuilder.AppendLine("Output ONLY a JSON object in this exact schema:");
                promptBuilder.AppendLine("{");
                promptBuilder.AppendLine("  \"matches\": [");
                promptBuilder.AppendLine("    {");
                promptBuilder.AppendLine("      \"raw\": \"string (the exact original raw English ingredient)\",");
                promptBuilder.AppendLine("      \"standardIngredientId\": \"string (the matched Guid or null if no match)\"");
                promptBuilder.AppendLine("    }");
                promptBuilder.AppendLine("  ]");
                promptBuilder.AppendLine("}");

                var requestPayload = new GeminiRequest
                {
                    Contents = new List<GeminiContent>
                    {
                        new GeminiContent
                        {
                            Parts = new List<GeminiPart>
                            {
                                new GeminiPart { Text = promptBuilder.ToString() }
                            }
                        }
                    },
                    GenerationConfig = new GeminiConfig
                    {
                        ResponseMimeType = "application/json"
                    }
                };

                var serializeOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestPayload, serializeOptions),
                    Encoding.UTF8,
                    "application/json");

                _logger.LogInformation("Calling Gemini API for matching {Count} ingredients...", rawEnglishIngredients.Count);
                
                var response = await _httpClient.PostAsync(requestUrl, jsonContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                var aiText = apiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                var tokensUsed = apiResponse?.UsageMetadata?.TotalTokenCount ?? 0;
                var apiLog = new ThirdPartyApiLog
                {
                    Id = Guid.NewGuid(),
                    ServiceName = "GoogleGemini",
                    Endpoint = "generateContent",
                    TokensUsed = tokensUsed,
                    WasCacheHit = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ThirdPartyApiLogs.Add(apiLog);
                await _context.SaveChangesAsync();

                if (string.IsNullOrWhiteSpace(aiText))
                {
                    _logger.LogWarning("Gemini API returned an empty response.");
                    return resultDict;
                }

                _logger.LogDebug("Gemini Response Text: {Text}", aiText);
                aiText = SanitizeJsonString(aiText);

                var matchResult = JsonSerializer.Deserialize<GeminiMatchResult>(aiText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (matchResult?.Matches != null)
                {
                    foreach (var match in matchResult.Matches)
                    {
                        if (string.IsNullOrWhiteSpace(match.Raw)) continue;

                        Guid? matchedId = null;
                        if (!string.IsNullOrWhiteSpace(match.StandardIngredientId) && 
                            Guid.TryParse(match.StandardIngredientId, out var parsedGuid))
                        {
                            matchedId = parsedGuid;
                        }

                        // Use simple loose containment or exact matching to correlate back if AI changed spacing slightly
                        var originalKey = rawEnglishIngredients.FirstOrDefault(r => r.Equals(match.Raw, StringComparison.OrdinalIgnoreCase)) 
                                          ?? rawEnglishIngredients.FirstOrDefault(r => r.Contains(match.Raw) || match.Raw.Contains(r)) 
                                          ?? match.Raw;

                        resultDict[originalKey] = matchedId;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GeminiMatchingService: {Message}", ex.Message);
            }

            return resultDict;
        }

        #region Gemini API JSON Models
        private class GeminiRequest
        {
            [JsonPropertyName("contents")]
            public List<GeminiContent>? Contents { get; set; }

            [JsonPropertyName("generationConfig")]
            public GeminiConfig? GenerationConfig { get; set; }
        }

        private class GeminiContent
        {
            [JsonPropertyName("parts")]
            public List<GeminiPart>? Parts { get; set; }
        }

        private class GeminiPart
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;
        }

        private class GeminiConfig
        {
            [JsonPropertyName("responseMimeType")]
            public string ResponseMimeType { get; set; } = "application/json";
        }

        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<GeminiCandidate>? Candidates { get; set; }

            [JsonPropertyName("usageMetadata")]
            public GeminiUsageMetadata? UsageMetadata { get; set; }
        }

        private class GeminiUsageMetadata
        {
            [JsonPropertyName("promptTokenCount")]
            public int PromptTokenCount { get; set; }

            [JsonPropertyName("candidatesTokenCount")]
            public int CandidatesTokenCount { get; set; }

            [JsonPropertyName("totalTokenCount")]
            public int TotalTokenCount { get; set; }
        }

        private class GeminiCandidate
        {
            [JsonPropertyName("content")]
            public GeminiContent? Content { get; set; }
        }

        private class GeminiMatchResult
        {
            public List<GeminiMatchEntry>? Matches { get; set; }
        }

        private class GeminiMatchEntry
        {
            public string Raw { get; set; } = string.Empty;
            public string StandardIngredientId { get; set; } = string.Empty;
        }

        #endregion

        public async Task<AiGeneratedRecipeDto> GenerateRecipeAsync(string dishName, List<string> ingredients)
        {
            try
            {
                var apiKey = _options.ApiKey;
                var baseUrl = _options.BaseUrl.TrimEnd('/');
var model = _options.Model;

                _logger.LogInformation("GenerateRecipeAsync called. DishName: '{DishName}', Ingredients count: {Count}, Model: {Model}", 
                    dishName, ingredients?.Count ?? 0, model);

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogError("Gemini API key is not configured. Cannot generate recipe.");
                    return new AiGeneratedRecipeDto { Description = "Gemini API key is not configured." };
                }

                // Defensively clean up baseUrl if user accidentally provided the full path in .env
                if (baseUrl.Contains("/v1beta"))
                {
                    baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("/v1beta"));
                }
                
                var requestUrl = $"{baseUrl}/v1beta/models/{model}:generateContent?key={apiKey}";

                var ingredientList = ingredients != null && ingredients.Any() 
                    ? string.Join(", ", ingredients) 
                    : "các nguyên liệu cơ bản phù hợp với món ăn";

                var promptBuilder = new StringBuilder();
                promptBuilder.AppendLine("Bạn là một đầu bếp chuyên nghiệp với nhiều năm kinh nghiệm.");
                promptBuilder.AppendLine($"Hãy viết hướng dẫn nấu món \"{dishName}\" với các nguyên liệu sau: {ingredientList}.");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("Yêu cầu:");
                promptBuilder.AppendLine("- Viết bằng Tiếng Việt");
                promptBuilder.AppendLine("- Chia thành từng bước rõ ràng, ngắn gọn, dễ hiểu");
                promptBuilder.AppendLine("- Mỗi bước bắt đầu bằng \"Bước X:\" và là một câu hoàn chỉnh");
                promptBuilder.AppendLine("- Bao gồm thời gian nấu ước tính nếu cần");
                promptBuilder.AppendLine("- Từ 4 đến 8 bước");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("Output CHỈ là một JSON object theo schema sau (schema tôi gửi chỉ là forrmat không sử dụng như dữ liệu đầu ra) (lưu ý: calories tính theo kcal, các thuộc tính nutrition tính theo gram, cookingTimeMinutes tính bằng phút):");
                promptBuilder.AppendLine("{");
                promptBuilder.AppendLine("  \"description\": \"string (mô tả ngắn ngọn, hấp dẫn về món ăn)\",");
                promptBuilder.AppendLine("  \"cookingTimeMinutes\": 30,");
                promptBuilder.AppendLine("  \"servings\": 2,");
                promptBuilder.AppendLine("  \"calories\": 350,");
                promptBuilder.AppendLine("  \"difficulty\": \"Dễ/Trung bình/Khó\",");
                promptBuilder.AppendLine("  \"categories\": [\"Món Việt\", \"Bữa chính\"],");
                promptBuilder.AppendLine("  \"tools\": [\"Nồi\", \"Chảo\"],");
                promptBuilder.AppendLine("  \"nutrition\": {");
                promptBuilder.AppendLine("    \"protein\": 25,");
                promptBuilder.AppendLine("    \"carbs\": 40,");
                promptBuilder.AppendLine("    \"fat\": 12,");
                promptBuilder.AppendLine("    \"fiber\": 5");
                promptBuilder.AppendLine("  },");
                promptBuilder.AppendLine("  \"steps\": [");
                promptBuilder.AppendLine("    \"Bước 1: ...\",");
                promptBuilder.AppendLine("    \"Bước 2: ...\"");
                promptBuilder.AppendLine("  ],");
                promptBuilder.AppendLine("  \"ingredientQuantities\": {");
                promptBuilder.AppendLine("    \"tên nguyên liệu 1\": { \"quantity\": 100, \"unit\": \"g\" },");
                promptBuilder.AppendLine("    \"tên nguyên liệu 2\": { \"quantity\": 10, \"unit\": \"ml\" }");
                promptBuilder.AppendLine("  }");
                promptBuilder.AppendLine("}");

                var requestPayload = new GeminiRequest
                {
                    Contents = new List<GeminiContent>
                    {
                        new GeminiContent
                        {
                            Parts = new List<GeminiPart>
                            {
                                new GeminiPart { Text = promptBuilder.ToString() }
                            }
                        }
                    },
                    GenerationConfig = new GeminiConfig
                    {
                        ResponseMimeType = "application/json"
                    }
                };

                var serializeOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestPayload, serializeOptions),
                    Encoding.UTF8,
                    "application/json");

                _logger.LogInformation("Sending request to Gemini API for recipe: {DishName}. URL: {Url}", dishName, requestUrl.Replace(apiKey, "HIDDEN_KEY"));

                var response = await _httpClient.PostAsync(requestUrl, jsonContent);
                
                _logger.LogInformation("Gemini API response status: {StatusCode}", response.StatusCode);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API returned error {StatusCode}: {Body}", response.StatusCode, errorBody);
                    return new AiGeneratedRecipeDto { Description = $"Gemini HTTP Error: {response.StatusCode}. Details: {errorBody}" };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Gemini API raw response length: {Length} chars", responseContent?.Length ?? 0);

                var apiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                var aiText = apiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                _logger.LogInformation("Gemini AI text extracted: {AiText}", aiText ?? "NULL");

                var tokensUsed = apiResponse?.UsageMetadata?.TotalTokenCount ?? 0;
                var apiLog = new ThirdPartyApiLog
                {
                    Id = Guid.NewGuid(),
                    ServiceName = "GoogleGemini",
                    Endpoint = "generateContent/recipe",
                    TokensUsed = tokensUsed,
                    WasCacheHit = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ThirdPartyApiLogs.Add(apiLog);
                await _context.SaveChangesAsync();

                if (string.IsNullOrWhiteSpace(aiText))
                {
                    _logger.LogWarning("Gemini API returned an empty AI text for recipe generation of '{DishName}'.", dishName);
                    return new AiGeneratedRecipeDto { Description = "Gemini returned empty text response." };
                }

                aiText = SanitizeJsonString(aiText);

                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                var recipeResult = JsonSerializer.Deserialize<AiGeneratedRecipeDto>(aiText, options);
                
                _logger.LogInformation("GenerateRecipeAsync completed for '{DishName}'.", dishName);
                return recipeResult ?? new AiGeneratedRecipeDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GenerateRecipeAsync for dish '{DishName}': {Message}", dishName, ex.Message);
                return new AiGeneratedRecipeDto { Description = $"Exception: {ex.Message} \n {ex.StackTrace}" };
            }
        }

        private string SanitizeJsonString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            
            int startIndex = input.IndexOf('{');
            int endIndex = input.LastIndexOf('}');
            
            if (startIndex >= 0 && endIndex >= startIndex)
            {
                return input.Substring(startIndex, endIndex - startIndex + 1);
            }
            
            return input.Trim();
        }
    }
}
