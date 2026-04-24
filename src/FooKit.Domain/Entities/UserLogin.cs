namespace MyProject.Domain.Entities
{
    public class UserLogin
    {
        public string LoginProvider { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public string? ProviderDisplayName { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
