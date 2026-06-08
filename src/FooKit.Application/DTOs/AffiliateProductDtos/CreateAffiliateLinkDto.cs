using System;

namespace FooKit.Application.DTOs.AffiliateProductDtos
{
    public class CreateAffiliateLinkDto
    {
        public Guid StandardIngredientId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;
        public decimal CurrentPriceAmount { get; set; }
        public string CurrentPriceCurrency { get; set; } = "VND";
        public string Platform { get; set; } = string.Empty;
    }
}
