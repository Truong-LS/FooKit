using System;
using FooKit.Domain.ValueObjects;

namespace FooKit.Domain.Entities
{
    public class SuggestionResult
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SuggestionRequestId { get; set; }
        public Guid DishCacheId { get; set; }
        public Money TotalEstimatedPrice { get; set; } = Money.Zero();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual SuggestionRequest? SuggestionRequest { get; set; }
        public virtual DishCache? DishCache { get; set; }
    }
}
