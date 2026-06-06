using System;

namespace FooKit.Application.DTOs.AffiliateProductDtos
{
    public class AffiliateLinkDto
    {
        public Guid Id { get; set; }
        public Guid StandardIngredientId { get; set; }
        public string StandardIngredientName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;
        public decimal CurrentPriceAmount { get; set; }
        public string CurrentPriceCurrency { get; set; } = "VND";
        public string Platform { get; set; } = string.Empty;
        public DateTime LastUpdatedPriceAt { get; set; }
        public bool IsActive { get; set; }
    }
}
