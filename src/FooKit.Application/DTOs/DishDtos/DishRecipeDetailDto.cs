using System;
using System.Collections.Generic;

namespace FooKit.Application.DTOs.DishDtos
{
    public class DishRecipeDetailDto
    {
        public Guid DishCacheId { get; set; }
        public string DishName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> CookingSteps { get; set; } = new List<string>();
        public List<SuggestedDishIngredientDto> Ingredients { get; set; } = new List<SuggestedDishIngredientDto>();
        public decimal TotalCost { get; set; }
    }
}
