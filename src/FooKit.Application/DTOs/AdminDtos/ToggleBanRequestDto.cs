using System;
using System.Text.Json.Serialization;

namespace MyProject.Application.DTOs.AdminDtos
{
    public class ToggleBanRequestDto
    {
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}
