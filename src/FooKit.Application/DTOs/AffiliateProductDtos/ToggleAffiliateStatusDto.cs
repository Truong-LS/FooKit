using System.Text.Json.Serialization;

namespace FooKit.Application.DTOs.AffiliateProductDtos
{
    public class ToggleAffiliateStatusDto
    {
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }
    }
}
