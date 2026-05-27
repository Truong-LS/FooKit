using System;

namespace MyProject.Application.DTOs.AiDictionaryDtos
{
    public class AiDictionaryDto
    {
        public Guid Id { get; set; }
        public string RawKeywordFromApi { get; set; } = string.Empty;
        public Guid StandardIngredientId { get; set; }
        public string StandardIngredientName { get; set; } = string.Empty;
    }
}
