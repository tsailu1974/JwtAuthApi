// Repositories/UserRepository.cs
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JwtAuthApi.DTOs;
using JwtAuthApi.Models;

namespace JwtAuthApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EnterpriseContext _db;

        public UserRepository(EnterpriseContext db)
        {
            _db = db;
        }

        public UserDto? GetUser(string username, string password)
        {
            // 1) Load the user row for hash checking
            var user = _db.Users
                          .FirstOrDefault(u => u.UserName == username);

            if (user == null) 
                return null;

            // 2) Verify password SHA‑256 hash
            using var sha = SHA256.Create();
            var passwordHash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            if (!passwordHash.SequenceEqual(user.PasswordHash!))
                return null;

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
