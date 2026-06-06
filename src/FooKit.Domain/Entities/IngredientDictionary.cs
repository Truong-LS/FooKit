using System;

namespace FooKit.Domain.Entities
{
    public class IngredientDictionary
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RawKeywordFromApi { get; set; } = string.Empty;
        public Guid StandardIngredientId { get; set; }

        public virtual StandardIngredient? StandardIngredient { get; set; }
    }
}
