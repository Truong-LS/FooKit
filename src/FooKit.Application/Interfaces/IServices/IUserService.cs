using System;
using System.Threading.Tasks;
using FooKit.Application.DTOs.UserDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<UserProfileResponse?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    }
}
