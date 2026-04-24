namespace MyProject.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
    }
}
