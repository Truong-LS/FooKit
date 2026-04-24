namespace MyProject.Application.Interfaces.IServices;

public interface IGoogleAuthProvider
{
    /// <summary>
    /// Validates a Google ID Token.
    /// </summary>
    /// <param name="idToken">The token received from the frontend.</param>
    /// <returns>Returns a tuple of (GoogleId, Email) if valid, or null if invalid.</returns>
    Task<(string GoogleId, string Email)?> ValidateTokenAsync(string idToken);
}
