using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public async Task<IActionResult> Register(Application.Services.Interfaces.RegisterRequest req)
        {
            var res = await _auth.RegisterAsync(req);
            if (!res.Success) return BadRequest(res.Errors);
            return Ok(new { token = res.Token, refreshToken = res.RefreshToken });
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(Application.Services.Interfaces.LoginRequest req)
        {
            var res = await _auth.LoginAsync(req);
            if (!res.Success) return Unauthorized(res.Errors);
            return Ok(new { token = res.Token, refreshToken = res.RefreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(Application.Services.Interfaces.RefreshRequest req)
        {
            var res = await _auth.RefreshTokenAsync(req.Token, req.RefreshToken);
            if (!res.Success) return Unauthorized(res.Errors);
            return Ok(new { token = res.Token, refreshToken = res.RefreshToken });
        }
    }

}
