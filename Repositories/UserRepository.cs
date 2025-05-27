// Repositories/UserRepository.cs
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JwtAuthApi.DTOs;
using JwtAuthApi.Models;
using JwtAuthApi.Services;

namespace JwtAuthApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EnterpriseContext _db;
        private readonly ILogger<AuthService> _logger;


        public UserRepository(EnterpriseContext db, ILogger<AuthService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public UserDto? GetUser(string username, string password)
        {
            // 1) Load the user row for hash checking
            _logger.LogInformation($"GetUser called for: {username}");

            var user = _db.Users
                          .FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                _logger.LogWarning("User not found in database");
                return null;
            }

            _logger.LogInformation($"User found: {user.UserName}");

            // 2) Verify password SHA‑256 hash
            using var sha = SHA256.Create();
            var passwordHash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

            _logger.LogInformation($"Computed hash: {Convert.ToBase64String(passwordHash)}");
            _logger.LogInformation($"Database hash: {Convert.ToBase64String(user.PasswordHash)}");

            if (!passwordHash.SequenceEqual(user.PasswordHash!))
            {
                _logger.LogWarning("Password hash mismatch");
                return null;
            }

            // 3) Fetch role names via explicit join
            var roleNames = _db.UserRoles
                            .Where(ur => ur.UserID == user.UserID)
                            .Select(ur => ur.Role.RoleName)
                            .ToList();

            // 4) Return only what the DTO needs
            return new UserDto
            {
                Username = user.UserName!,
                RoleNames = roleNames    
            };
        }
    }
}
