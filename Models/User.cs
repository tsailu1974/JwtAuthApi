using System;
using System.Collections.Generic;

namespace JwtAuthApi.Models;

public partial class User
{
    public int UserID { get; set; }

    public string? UserName { get; set; }

    public string? RoleName { get; set; }

    public byte[]? PasswordHash { get; set; }

    public bool? IsActive { get; set; }

    public string? Email { get; set; }

    public DateOnly? CreatedDate { get; set; }
}
