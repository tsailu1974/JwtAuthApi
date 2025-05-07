using Microsoft.AspNetCore.Mvc;
using JwtAuthApi.Services;
using JwtAuthApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthApi.Controllers 
{

    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var token = _authService.Authenticate(login.Username, login.Password);

            if (token == null) return Unauthorized("Invalid credentials");

            return Ok(new {token});
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("protected")]
        public IActionResult GetSecret()
        {
            return Ok("🎉 This is protected content");
        }
    }
}