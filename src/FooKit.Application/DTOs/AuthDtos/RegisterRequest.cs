namespace FooKit.Application.DTOs.AuthDtos;

public record RegisterRequest(string Username, string Password, string ConfirmPassword);

