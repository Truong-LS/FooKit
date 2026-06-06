using System;
using System.Text.Json.Serialization;

namespace FooKit.Application.DTOs.AdminDtos
{
    public class ClearHomepageCacheRequest
    {
        [JsonPropertyName("target_user_id")]
        public string TargetUserId { get; set; }
    }
}
