using JwtAuthApi.Models;

namespace JwtAuthApi.Repositories
{
    public interface IUserRepository
    {
        User? GetUser(string username, string password);
    }
}