using System.Text.Json.Serialization;

namespace MyProject.Application.DTOs.AffiliateProductDtos
{
    public class ToggleAffiliateStatusDto
    {
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }
    }
}
