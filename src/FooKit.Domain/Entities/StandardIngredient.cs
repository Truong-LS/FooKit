using System;
using System.Collections.Generic;
using MyProject.Domain.Enums;

namespace MyProject.Domain.Entities
{
    public class StandardIngredient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public IngredientCategory Category { get; set; } = IngredientCategory.Other;

        public virtual ICollection<IngredientDictionary> IngredientDictionaries { get; set; } = new List<IngredientDictionary>();
        public virtual ICollection<AffiliateProduct> AffiliateProducts { get; set; } = new List<AffiliateProduct>();
    }
}
