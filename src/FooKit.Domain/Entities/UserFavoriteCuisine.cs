using System;

namespace FooKit.Domain.Entities
{
    public class UserFavoriteCuisine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string CuisineName { get; set; } = string.Empty;
        public virtual User User { get; set; } = null!;
    }
}
