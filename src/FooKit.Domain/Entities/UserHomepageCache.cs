using System;

namespace MyProject.Domain.Entities
{
    public class UserHomepageCache
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public string SerializedMenuData { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
