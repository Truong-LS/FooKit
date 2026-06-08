using System;
using System.Linq;
using System.Threading.Tasks;
using FooKit.Application.DTOs.AdminDtos;
using FooKit.Application.DTOs.Common;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using FooKit.Domain.Exceptions;

namespace FooKit.Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IImageService _imageService;
        private readonly AutoMapper.IMapper _mapper;

        public AdminUserService(IUnitOfWork unitOfWork, IMemoryCache memoryCache, IImageService imageService, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
            _imageService = imageService;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserAdminResponseDto>> GetUsersAsync(GetUsersRequestDto request)
        {
            var (users, totalCount) = await _unitOfWork.Users.GetUsersWithSubscriptionsAsync(
                request.Search, request.IsPremium, request.IsActive, request.Page, request.Size);

            var items = _mapper.Map<IEnumerable<UserAdminResponseDto>>(users);

            return new PagedResult<UserAdminResponseDto>
            {
                Items = items,
                Page = request.Page,
                Size = request.Size,
                TotalCount = totalCount
            };
        }

        public async Task<UserAdminSubscriptionStatusDto> GrantPremiumAsync(Guid userId, GrantPremiumRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new NotFoundException($"User with ID {userId} not found.");

            var plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(request.PlanId);
            if (plan == null) throw new NotFoundException($"Subscription plan with ID {request.PlanId} not found.");

            var activeSub = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionAsync(userId);
            
            var startDate = DateTime.UtcNow;
            if (activeSub != null && activeSub.IsActive && activeSub.EndDate > startDate)
            {
                startDate = activeSub.EndDate;
            }

            var newSub = new UserSubscription
            {
                UserId = userId,
                PlanId = request.PlanId,
                StartDate = startDate,
                EndDate = startDate.AddDays(plan.DurationInDays),
                IsActive = true
            };

            await _unitOfWork.UserSubscriptions.AddAsync(newSub);
            await _unitOfWork.SaveChangesAsync();

            return new UserAdminSubscriptionStatusDto
            {
                IsPremium = true,
                PlanName = plan.PlanName,
                EndDate = newSub.EndDate
            };
        }

        public async Task ToggleBanAsync(Guid userId, ToggleBanRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new NotFoundException($"User with ID {userId} not found.");

            user.IsActive = request.IsActive;
            user.BanReason = request.Reason;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Clear cache to make ban take effect immediately
            _memoryCache.Remove($"UserActiveStatus_{userId}");
        }

        public async Task<UserAdminResponseDto> UpdateUserAsync(Guid userId, UpdateUserAdminRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new NotFoundException($"User with ID {userId} not found.");

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Username))
                user.Username = request.Username;

            if (request.AvatarFile != null)
            {
                var avatarUrl = await _imageService.UploadImageAsync(request.AvatarFile);
                user.AvatarUrl = avatarUrl;
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserAdminResponseDto>(user);
        }
        
        public async Task<UserAdminResponseDto> CreateUserAsync(CreateUserAdminRequestDto request)
        {
            // Simple check for existing username/email
            var existingUser = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
            if (existingUser != null)
                throw new ConflictException("Username already exists.");

            var userByEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (userByEmail != null)
                throw new ConflictException("Email already exists.");

            var userRole = (await _unitOfWork.Roles.FindAsync(r => r.Name == "User")).FirstOrDefault();
            if (userRole == null)
                throw new Exception("Default user role not found.");

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = userRole.Id,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserAdminResponseDto>(newUser);
        }
    }
}
