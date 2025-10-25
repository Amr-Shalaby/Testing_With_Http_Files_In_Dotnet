using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ApiTest.Authentication;

public class SimpleAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public SimpleAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) 
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if Authorization header exists
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));
        }

        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            
            // Expected format: "Bearer {token}"
            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header format"));
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            // Decode the simple token (format: userId:username:role:timestamp)
            var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var parts = tokenData.Split(':');
            
            if (parts.Length < 3)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid token format"));
            }

            var userId = parts[0];
            var username = parts[1];
            var role = parts[2];

            // Create claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Authentication failed");
            return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }
    }
}

