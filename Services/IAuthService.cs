namespace JwtAuthApi.Services
{
    public interface IAuthService
    {
        string? Authenticate(string username, string password);
    }
}