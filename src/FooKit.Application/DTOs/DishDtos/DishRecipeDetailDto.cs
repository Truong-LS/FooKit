using System;
using System.Collections.Generic;

namespace FooKit.Application.DTOs.DishDtos
{
    public class NutritionDto
    {
        public int Protein { get; set; }
        public int Carbs { get; set; }
        public int Fat { get; set; }
        public int Fiber { get; set; }
    }

    public class DishRecipeIngredientDto
    {
        public string RawIngredientName { get; set; } = string.Empty;
        public string StandardIngredientId { get; set; } = string.Empty;
        public string StandardIngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsMatched { get; set; }
        public bool IsPriced { get; set; }
        public string AffiliateUrl { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
    }

    public class DishRecipeDetailDto
    {
        public string DishCacheId { get; set; } = string.Empty;
        public string DishName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CookingTimeMinutes { get; set; }
        public int Servings { get; set; }
        public int Calories { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Tools { get; set; } = new List<string>();
        public NutritionDto Nutrition { get; set; } = new NutritionDto();
        public List<string> CookingSteps { get; set; } = new List<string>();
        public List<DishRecipeIngredientDto> Ingredients { get; set; } = new List<DishRecipeIngredientDto>();
        public decimal TotalCost { get; set; }
    }
}
