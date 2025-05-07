using JwtAuthApi.Models;

namespace JwtAuthApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        public User? GetUser(string username, string password)
        {
            if(username == "admin" && password == "1234")
            {
                return new User {Username = "admin", Role = "Admin"};
            }
            return null;
        }
    }
}