namespace FooKit.Application.DTOs.AuthDtos;

public record SetCredentialsRequest(string Username, string Password, string ConfirmPassword);
