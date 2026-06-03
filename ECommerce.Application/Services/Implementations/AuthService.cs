using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<AuthenticationResult> RegisterAsync(RegisterRequest request)
        {
            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
                return new AuthenticationResult(false, null, null, new[] { "User already exists" });

            var user = new ApplicationUser { Email = request.Email, UserName = request.Email, FirstName = request.FirstName, LastName = request.LastName };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return new AuthenticationResult(false, null, null, result.Errors.Select(e => e.Description));

            // Optionally add default role
            await _userManager.AddToRoleAsync(user, "User");

            // generate tokens
            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(); // implement secure random token and store it

            // persist refresh token with user (e.g., in DB)
            // ...

            return new AuthenticationResult(true, token, refreshToken, null);
        }

        public async Task<AuthenticationResult> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return new AuthenticationResult(false, null, null, new[] { "Invalid credentials" });

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return new AuthenticationResult(false, null, null, new[] { "Invalid credentials" });

            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            // store refresh token...

            return new AuthenticationResult(true, token, refreshToken, null);
        }

        public Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            // validate existing refresh token stored in DB, expiration, rotation, etc.
            throw new NotImplementedException();
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwt = _config.GetSection("JwtSettings");
            var secret = jwt["Secret"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiresInMinutes"] ?? "60")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
