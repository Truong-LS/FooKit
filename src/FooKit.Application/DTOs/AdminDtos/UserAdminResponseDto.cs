using System;
using System.Text.Json.Serialization;

namespace MyProject.Application.DTOs.AdminDtos
{
    public class UserAdminResponseDto
    {
        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("subscription_status")]
        public UserAdminSubscriptionStatusDto? SubscriptionStatus { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class UserAdminSubscriptionStatusDto
    {
        [JsonPropertyName("is_premium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("plan_name")]
        public string? PlanName { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }
    }
}
