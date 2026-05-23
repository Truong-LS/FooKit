using System;

namespace MyProject.Domain.Entities
{
    public class UserAllergy
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public string AllergenName { get; set; } = string.Empty;
    }
}
