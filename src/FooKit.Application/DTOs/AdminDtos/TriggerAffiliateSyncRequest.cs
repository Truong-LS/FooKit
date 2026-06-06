using System;
using System.Text.Json.Serialization;

namespace FooKit.Application.DTOs.AdminDtos
{
    public class TriggerAffiliateSyncRequest
    {
        [JsonPropertyName("target_ingredient_id")]
        public string TargetIngredientId { get; set; }

        [JsonPropertyName("force_sync_all")]
        public bool ForceSyncAll { get; set; }
    }
}
