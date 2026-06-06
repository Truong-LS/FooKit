using System;

namespace FooKit.Application.DTOs.SubscriptionDtos
{
    public class UserSubscriptionDto
    {
        public bool IsPremium { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DaysRemaining { get; set; }
    }
}
