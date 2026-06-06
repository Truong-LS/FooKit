using System;
using FooKit.Domain.Enums;

namespace FooKit.Domain.Entities
{
    public class UserDietaryPreference
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DietaryType DietaryType { get; set; } = DietaryType.None;

        public virtual User? User { get; set; }
    }
}
