using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using FooKit.Application.DTOs.UserDtos;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;
using System.Linq;

namespace FooKit.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public UserService(IUnitOfWork unitOfWork, IImageService imageService, AutoMapper.IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                throw new BadRequestException("New password and confirm password do not match.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new BadRequestException("This account is currently linked via a third-party provider and does not have a password set. Please use the set credentials feature first.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new BadRequestException("Incorrect current password.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<UserProfileResponse?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            user.FullName = request.FullName;

            if (request.AvatarFile != null)
            {
                var avatarUrl = await _imageService.UploadImageAsync(request.AvatarFile);
                user.AvatarUrl = avatarUrl;
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserProfileResponse>(user);
        }

        public async Task<DietaryProfileResponseDto> GetDietaryProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not found.");

            var diets = (await _unitOfWork.UserDietaryPreferences.FindAsync(d => d.UserId == userId)).Select(d => d.DietaryType).ToList();
            var allergies = (await _unitOfWork.UserAllergies.FindAsync(a => a.UserId == userId)).Select(a => a.AllergenName).ToList();
            var cuisines = (await _unitOfWork.UserFavoriteCuisines.FindAsync(c => c.UserId == userId)).Select(c => c.CuisineName).ToList();

            return new DietaryProfileResponseDto
            {
                Diets = diets,
                Allergies = allergies,
                FavoriteCuisines = cuisines,
                WeeklyBudget = user.WeeklyBudget
            };
        }

        public async Task UpdateDietaryProfileAsync(Guid userId, SaveDietaryProfileRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not found.");

            user.WeeklyBudget = request.WeeklyBudget;
            _unitOfWork.Users.Update(user);

            var oldDiets = await _unitOfWork.UserDietaryPreferences.FindAsync(d => d.UserId == userId);
            _unitOfWork.UserDietaryPreferences.RemoveRange(oldDiets);

            var oldAllergies = await _unitOfWork.UserAllergies.FindAsync(a => a.UserId == userId);
            _unitOfWork.UserAllergies.RemoveRange(oldAllergies);

            var oldCuisines = await _unitOfWork.UserFavoriteCuisines.FindAsync(c => c.UserId == userId);
            _unitOfWork.UserFavoriteCuisines.RemoveRange(oldCuisines);

            if (request.Diets != null && request.Diets.Any())
            {
                var newDiets = request.Diets.Select(d => new FooKit.Domain.Entities.UserDietaryPreference { UserId = userId, DietaryType = d });
                await _unitOfWork.UserDietaryPreferences.AddRangeAsync(newDiets);
            }

            if (request.Allergies != null && request.Allergies.Any())
            {
                var newAllergies = request.Allergies.Select(a => new FooKit.Domain.Entities.UserAllergy { UserId = userId, AllergenName = a });
                await _unitOfWork.UserAllergies.AddRangeAsync(newAllergies);
            }

            if (request.FavoriteCuisines != null && request.FavoriteCuisines.Any())
            {
                var newCuisines = request.FavoriteCuisines.Select(c => new FooKit.Domain.Entities.UserFavoriteCuisine { UserId = userId, CuisineName = c });
                await _unitOfWork.UserFavoriteCuisines.AddRangeAsync(newCuisines);
            }

            await _unitOfWork.SaveChangesAsync();

            // Invalidate homepage suggestions cache để gợi ý mới phản ánh dietary profile mới
            _memoryCache.Remove($"HomepageCache:User_{userId}_breakfast");
            _memoryCache.Remove($"HomepageCache:User_{userId}_lunch");
            _memoryCache.Remove($"HomepageCache:User_{userId}_dinner");
        }
    }
}
