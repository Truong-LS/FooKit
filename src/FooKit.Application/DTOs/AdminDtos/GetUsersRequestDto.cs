using System;
using System.Text.Json.Serialization;

namespace FooKit.Application.DTOs.AdminDtos
{
    public class GetUsersRequestDto
    {
        [JsonPropertyName("page")]
        public int Page { get; set; } = 1;

        [JsonPropertyName("size")]
        public int Size { get; set; } = 20;

        [JsonPropertyName("search")]
        public string? Search { get; set; }

        [JsonPropertyName("is_premium")]
        public bool? IsPremium { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
    }
}
