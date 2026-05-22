using System;
using System.Collections.Generic;

namespace MyProject.Application.DTOs.DishDtos
{
    public class DishSuggestionResponseDto
    {
        public List<SuggestedDishDto> SuggestedDishes { get; set; } = new List<SuggestedDishDto>();
    }

    public class SuggestedDishDto
    {
        public string DishName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
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
