namespace ApiTest.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // User, Admin
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public record RegisterRequest(
    string Username,
    string Email,
    string Password
);

public record LoginRequest(
    string Username,
    string Password
);

public record AuthResponse(
    bool Success,
    string? Message,
    UserInfo? User,
    string? Token
);

public record UserInfo(
    int Id,
    string Username,
    string Email,
    string Role
);

