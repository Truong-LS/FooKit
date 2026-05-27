using System;

namespace MyProject.Domain.Entities
{
    public class ThirdPartyApiLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ServiceName { get; set; } = string.Empty; // e.g. "GoogleGemini", "Spoonacular"
        public string Endpoint { get; set; } = string.Empty;
        public int TokensUsed { get; set; }
        public bool WasCacheHit { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
