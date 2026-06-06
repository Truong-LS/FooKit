using System;

namespace FooKit.Domain.Entities
{
    public class UserHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public string DishName { get; set; } = string.Empty;
        public DateTime CookedAt { get; set; } = DateTime.UtcNow;
    }
}
