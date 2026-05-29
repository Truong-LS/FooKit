using System;

namespace MyProject.Application.DTOs.AdminDtos
{
    public class DashboardOverviewDto
    {
        public DateTime Timestamp { get; set; }
        public UserMetricsDto UsersMetrics { get; set; } = new();
        public ContentMetricsDto ContentMetrics { get; set; } = new();
        public SystemHealthDto SystemHealth { get; set; } = new();
    }

    public class UserMetricsDto
    {
        public int TotalUsers { get; set; }
        public int PremiumUsers { get; set; }
        public int NewUsersToday { get; set; }
    }

    public class ContentMetricsDto
    {
        public int MealsGeneratedToday { get; set; }
        public int TotalActiveAffiliateLinks { get; set; }
    }

    public class SystemHealthDto
    {
        public bool IsWorkerRunning { get; set; }
        public DateTime? LastAffiliateSync { get; set; }
    }
}
