using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.Infrastructure.ExternalServices;

public class GoogleAuthProvider : IGoogleAuthProvider
{
    private readonly IConfiguration _config;

    public GoogleAuthProvider(IConfiguration config)
    {
        _config = config;
    }

    public async Task<(string GoogleId, string Email)?> ValidateTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _config["GOOGLE_CLIENT_ID"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            
            return (payload.Subject, payload.Email);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}
