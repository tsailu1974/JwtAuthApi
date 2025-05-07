namespace JwtAuthApi.Models 
{
    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class User
    {
        public required string Username { get; set; }
        public required string Role { get; set; }
    }
}