using System;
using System.Text.Json.Serialization;

namespace FooKit.Application.DTOs.AiDictionaryDtos
{
    public class UpdateAiDictionaryDto
    {
        [JsonPropertyName("new_standard_ingredient_id")]
        public Guid NewStandardIngredientId { get; set; }
    }
}
