using System;
using System.Collections.Generic;
using FooKit.Domain.ValueObjects;

namespace FooKit.Domain.Entities
{
    public class SubscriptionPlan
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PlanName { get; set; } = string.Empty;
        public Money Price { get; set; } = Money.Zero();
        public int DurationInDays { get; set; }
        public string FeaturesJson { get; set; } = "[]";

        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }
}
