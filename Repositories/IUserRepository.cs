using JwtAuthApi.DTOs;
using JwtAuthApi.Models;

namespace JwtAuthApi.Repositories
{
    public interface IUserRepository
    {
        UserDto? GetUser(string username, string password);
    }
}