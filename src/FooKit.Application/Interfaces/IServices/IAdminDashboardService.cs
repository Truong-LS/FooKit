using System;
using System.Threading.Tasks;
using FooKit.Application.DTOs.AdminDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IAdminDashboardService
    {
        Task<DashboardOverviewDto> GetOverviewAsync();
        Task<ApiUsageDto> GetApiUsageAsync(DateTime startDate, DateTime endDate);
    }
}
