using System.Security.Cryptography;
using System.Text;
using ApiTest.Models;

namespace ApiTest.Services;

public class AuthService : IAuthService
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public AuthService()
    {
        // Seed with a test user
        _users.Add(new User
        {
            Id = _nextId++,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = HashPassword("password123"),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        });

        _users.Add(new User
        {
            Id = _nextId++,
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = HashPassword("admin123"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Validate username doesn't exist
        if (_users.Any(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase)))
        {
            return Task.FromResult(new AuthResponse(
                Success: false,
                Message: "Username already exists",
                User: null,
                Token: null
            ));
        }

        // Validate email doesn't exist
        if (_users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return Task.FromResult(new AuthResponse(
                Success: false,
                Message: "Email already exists",
                User: null,
                Token: null
            ));
        }

        // Create new user
        var user = new User
        {
            Id = _nextId++,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        _users.Add(user);

        var userInfo = new UserInfo(user.Id, user.Username, user.Email, user.Role);
        var token = GenerateSimpleToken(user);

        return Task.FromResult(new AuthResponse(
            Success: true,
            Message: "Registration successful",
            User: userInfo,
            Token: token
        ));
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            return Task.FromResult(new AuthResponse(
                Success: false,
                Message: "Invalid username or password",
                User: null,
                Token: null
            ));
        }

        // Verify password
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return Task.FromResult(new AuthResponse(
                Success: false,
                Message: "Invalid username or password",
                User: null,
                Token: null
            ));
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;

        var userInfo = new UserInfo(user.Id, user.Username, user.Email, user.Role);
        var token = GenerateSimpleToken(user);

        return Task.FromResult(new AuthResponse(
            Success: true,
            Message: "Login successful",
            User: userInfo,
            Token: token
        ));
    }

    public Task<User?> GetUserByIdAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    // Simple password hashing (for testing only - use proper hashing in production!)
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password + "SimpleSalt");
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == hash;
    }

    // Simple token generation (for testing only - use JWT in production!)
    private static string GenerateSimpleToken(User user)
    {
        var tokenData = $"{user.Id}:{user.Username}:{user.Role}:{DateTime.UtcNow.Ticks}";
        var bytes = Encoding.UTF8.GetBytes(tokenData);
        return Convert.ToBase64String(bytes);
    }
}

