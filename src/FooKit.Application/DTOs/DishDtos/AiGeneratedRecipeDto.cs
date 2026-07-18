using System.Collections.Generic;

namespace FooKit.Application.DTOs.DishDtos
{
    public class AiGeneratedRecipeDto
    {
        public string Description { get; set; } = string.Empty;
        public int CookingTimeMinutes { get; set; }
        public int Servings { get; set; }
        public int Calories { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Tools { get; set; } = new List<string>();
        public AiRecipeNutritionDto Nutrition { get; set; } = new AiRecipeNutritionDto();
        public List<string> Steps { get; set; } = new List<string>();
        public Dictionary<string, AiRecipeIngredientInfoDto> IngredientQuantities { get; set; } = new Dictionary<string, AiRecipeIngredientInfoDto>();
    }

    public class AiRecipeNutritionDto
    {
        public int Protein { get; set; }
        public int Carbs { get; set; }
        public int Fat { get; set; }
        public int Fiber { get; set; }
    }

    public class AiRecipeIngredientInfoDto
    {
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
