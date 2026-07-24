using System;
using System.Collections.Generic;

namespace FooKit.Application.DTOs.DishDtos
{
    public class DishSuggestionResponseDto
    {
        public List<SuggestedDishDto> SuggestedDishes { get; set; } = new List<SuggestedDishDto>();
    }

    public class SuggestedDishDto
    {
        public string DishCacheId { get; set; } = string.Empty;
        public string DishName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int CookingTimeMinutes { get; set; }
        public int Calories { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public int Servings { get; set; }
        public decimal TotalCost { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public string Instructions { get; set; } = string.Empty;
        public List<SuggestedDishIngredientDto> Ingredients { get; set; } = new List<SuggestedDishIngredientDto>();
    }

    public class SuggestedDishIngredientDto
    {
        public string RawEnglishName { get; set; } = string.Empty;
        public string StandardIngredientName { get; set; } = string.Empty;
        public bool IsMapped { get; set; }
        public SuggestedAffiliateProductDto? AffiliateProduct { get; set; }
    }

    public class SuggestedAffiliateProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Platform { get; set; } = string.Empty;
    }
}
