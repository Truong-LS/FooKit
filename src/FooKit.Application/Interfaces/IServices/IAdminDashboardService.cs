using System;
using System.Threading.Tasks;
using MyProject.Application.DTOs.AdminDashboardDtos;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IAdminDashboardService
    {
        Task<DashboardOverviewDto> GetOverviewAsync();
        Task<ApiUsageDto> GetApiUsageAsync(DateTime startDate, DateTime endDate);
    }
}
