using Microsoft.AspNetCore.Http;

namespace FooKit.Application.DTOs.UserDtos
{
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }
}
