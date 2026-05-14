using System;
using MyProject.Domain.ValueObjects;

namespace MyProject.Domain.Entities
{
    public class AffiliateProduct
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StandardIngredientId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;
        public Money CurrentPrice { get; set; } = Money.Zero();
        public string Platform { get; set; } = string.Empty;
        public DateTime LastUpdatedPriceAt { get; set; }

        public virtual StandardIngredient? StandardIngredient { get; set; }
    }
}
