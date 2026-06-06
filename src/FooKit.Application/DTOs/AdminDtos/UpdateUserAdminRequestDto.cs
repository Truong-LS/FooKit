using Microsoft.AspNetCore.Http;

namespace FooKit.Application.DTOs.AdminDtos
{
    public class UpdateUserAdminRequestDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }
}
