namespace JwtAuthApi.DTOs
{
    public class UserDto
    {
        public string Username     { get; set; } = null!;
        public List<string> RoleNames { get; set; } = new();
    }
}