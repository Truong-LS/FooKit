using System;
using System.ComponentModel.DataAnnotations;

namespace FooKit.Application.DTOs.AdminDtos
{
    public class CreateUserAdminRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? FullName { get; set; }
    }
}
