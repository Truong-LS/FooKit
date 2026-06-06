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
    }
}
