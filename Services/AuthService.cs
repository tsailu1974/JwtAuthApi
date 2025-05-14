using JwtAuthApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuthApi.Models;

namespace JwtAuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }
        
        [Authorize]
        public string? Authenticate(string username, string password)
        {
            var user = _userRepo.GetUser(username, password);

            if ( user == null ) return null;

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Role, user.Rolename!)
            };

            var jwtKey = _config["Jwt:Key"] ?? throw new Exception("Jwt:Key is missing");
            var jwtIssuer = _config["Jwt:Issuer"] ?? throw new Exception("Jwt:Issuer is missing");
            var jwtAudience = _config["Jwt:Audience"] ?? throw new Exception("Jwt:Audience is missing");
            var jwtExpire = int.TryParse(_config["Jwt:ExpireMinutes"], out int minutes) ? minutes : 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes((jwtExpire)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}