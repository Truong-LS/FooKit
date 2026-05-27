using System;
using System.Collections.Generic;

namespace MyProject.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
        public virtual ICollection<UserDietaryPreference> DietaryPreferences { get; set; } = new List<UserDietaryPreference>();
        public virtual ICollection<UserTool> Tools { get; set; } = new List<UserTool>();
        public virtual ICollection<SuggestionRequest> SuggestionRequests { get; set; } = new List<SuggestionRequest>();
        public virtual ICollection<UserAllergy> Allergies { get; set; } = new List<UserAllergy>();
        public virtual ICollection<UserHistory> UserHistories { get; set; } = new List<UserHistory>();
        public virtual ICollection<UserHomepageCache> HomepageCaches { get; set; } = new List<UserHomepageCache>();
    }
}
