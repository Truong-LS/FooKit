using System;
using System.Collections.Generic;
using FooKit.Domain.Enums;

namespace FooKit.Domain.Entities
{
    public class StandardIngredient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public IngredientCategory Category { get; set; } = IngredientCategory.DairyAndOther;
        public decimal DefaultPrice { get; set; } = 0;
        public int EstimatedUses { get; set; } = 1;
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<IngredientDictionary> IngredientDictionaries { get; set; } = new List<IngredientDictionary>();
        public virtual ICollection<AffiliateProduct> AffiliateProducts { get; set; } = new List<AffiliateProduct>();
    }
}
