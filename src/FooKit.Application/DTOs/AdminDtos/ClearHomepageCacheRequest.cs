using System;
using System.Text.Json.Serialization;

namespace MyProject.Application.DTOs.AdminDtos
{
    public class ClearHomepageCacheRequest
    {
        [JsonPropertyName("target_user_id")]
        public string TargetUserId { get; set; }
    }
}
