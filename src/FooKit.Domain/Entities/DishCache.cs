using System;
using System.Collections.Generic;

namespace MyProject.Domain.Entities
{
    public class DishCache
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ExternalApiId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string DietaryTagsJson { get; set; } = "[]";
        public string RequiredToolsJson { get; set; } = "[]";
        public string RawIngredientsJson { get; set; } = "[]";
        public DateTime LastFetchedAt { get; set; }

        public virtual ICollection<SuggestionResult> SuggestionResults { get; set; } = new List<SuggestionResult>();
    }
}
