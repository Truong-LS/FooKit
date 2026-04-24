namespace MyProject.Application.DTOs.AuthDtos;

public record TokenRequest(string AccessToken, string RefreshToken);