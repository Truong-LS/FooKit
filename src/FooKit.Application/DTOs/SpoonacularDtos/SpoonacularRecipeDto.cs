using System.Collections.Generic;

namespace FooKit.Application.DTOs.SpoonacularDtos
{
    public class SpoonacularRecipeDto
    {
        public int SpoonacularId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public int ReadyInMinutes { get; set; }
        public int Servings { get; set; }
        public int Calories { get; set; }
        public List<string> RawIngredients { get; set; } = new List<string>();
        public List<string> Diets { get; set; } = new List<string>();
        public List<string> AnalyzedInstructionsSteps { get; set; } = new List<string>();
    }
}
