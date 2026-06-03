namespace ECommerce.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticationResult> RegisterAsync(RegisterRequest request);
        Task<AuthenticationResult> LoginAsync(LoginRequest request);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }

    public record RegisterRequest(string Email, string Password, string? FirstName = null, string? LastName = null);
    public record LoginRequest(string Email, string Password);
    public record AuthenticationResult(bool Success, string? Token, string? RefreshToken, IEnumerable<string>? Errors);
    public record RefreshRequest(string Token, string RefreshToken);
}
