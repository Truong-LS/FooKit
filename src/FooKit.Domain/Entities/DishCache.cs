using System;
using System.Collections.Generic;

namespace FooKit.Domain.Entities
{
    public class DishCache
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ExternalApiId { get; set; } = string.Empty;
        public int SpoonacularId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string DietaryTagsJson { get; set; } = "[]";
        public string RequiredToolsJson { get; set; } = "[]";
        public string RawIngredientsJson { get; set; } = "[]";
        public string InstructionsJson { get; set; } = "[]";
        public int ReadyInMinutes { get; set; } = 30;
        public int Servings { get; set; } = 2;
        public int Calories { get; set; } = 350;
        public DateTime LastFetchedAt { get; set; }

        public virtual ICollection<SuggestionResult> SuggestionResults { get; set; } = new List<SuggestionResult>();
    }
}
