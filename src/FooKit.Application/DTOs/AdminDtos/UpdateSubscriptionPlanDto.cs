using System;
using System.Collections.Generic;

namespace FooKit.Application.DTOs.AdminDtos
{
    public class UpdateSubscriptionPlanDto
    {
        public string PlanName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "VND";
        public int DurationInDays { get; set; }
        public List<string> Features { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }
}
