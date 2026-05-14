using System;
using System.Collections.Generic;
using MyProject.Domain.Enums;
using MyProject.Domain.ValueObjects;

namespace MyProject.Domain.Entities
{
    public class SuggestionRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Money TargetBudget { get; set; } = Money.Zero();
        public DietaryType DietaryRequirement { get; set; } = DietaryType.None;
        public string AvailableToolsJson { get; set; } = "[]";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User? User { get; set; }
        public virtual ICollection<SuggestionResult> SuggestionResults { get; set; } = new List<SuggestionResult>();
    }
}
