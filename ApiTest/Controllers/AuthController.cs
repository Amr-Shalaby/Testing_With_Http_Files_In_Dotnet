using Microsoft.AspNetCore.Mvc;
using ApiTest.Models;
using ApiTest.Services;

namespace ApiTest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for username: {Username}", request.Username);

        // Basic validation
        if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3)
        {
            return BadRequest(new AuthResponse(
                Success: false,
                Message: "Username must be at least 3 characters long",
                User: null,
                Token: null
            ));
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            return BadRequest(new AuthResponse(
                Success: false,
                Message: "Invalid email address",
                User: null,
                Token: null
            ));
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            return BadRequest(new AuthResponse(
                Success: false,
                Message: "Password must be at least 6 characters long",
                User: null,
                Token: null
            ));
        }

        var response = await _authService.RegisterAsync(request);

        if (!response.Success)
        {
            _logger.LogWarning("Registration failed for username: {Username}. Reason: {Reason}",
                request.Username, response.Message);
            return BadRequest(response);
        }

        _logger.LogInformation("User registered successfully: {Username}", request.Username);
        return CreatedAtAction(nameof(GetUserInfo), new { username = request.Username }, response);
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for username: {Username}", request.Username);

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new AuthResponse(
                Success: false,
                Message: "Username and password are required",
                User: null,
                Token: null
            ));
        }

        var response = await _authService.LoginAsync(request);

        if (!response.Success)
        {
            _logger.LogWarning("Login failed for username: {Username}", request.Username);
            return Unauthorized(response);
        }

        _logger.LogInformation("User logged in successfully: {Username}", request.Username);
        return Ok(response);
    }

    /// <summary>
    /// Get user information by username
    /// </summary>
    [HttpGet("user/{username}")]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInfo>> GetUserInfo(string username)
    {
        _logger.LogInformation("Getting user info for: {Username}", username);

        var user = await _authService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        var userInfo = new UserInfo(user.Id, user.Username, user.Email, user.Role);
        return Ok(userInfo);
    }

    /// <summary>
    /// Test endpoint to verify authentication is working
    /// </summary>
    [HttpGet("test")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public ActionResult<object> Test()
    {
        return Ok(new
        {
            Message = "Authentication API is working",
            Timestamp = DateTime.UtcNow,
            TestUsers = new[]
            {
                new { Username = "testuser", Password = "password123", Role = "User" },
                new { Username = "admin", Password = "admin123", Role = "Admin" }
            }
        });
    }
}

