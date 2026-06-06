using System;
using System.Text.Json.Serialization;

namespace FooKit.Application.DTOs.AdminDtos
{
    public class GrantPremiumRequestDto
    {
        [JsonPropertyName("plan_id")]
        public Guid PlanId { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}
