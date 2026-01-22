using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Application.Interfaces.Security;

namespace PostManagement.Api.Security;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher)
        : base(options, logger, encoder, clock)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHasher = passwordHasher;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.NoResult();
        }

        if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out var headerValue) ||
            !string.Equals(headerValue.Scheme, "Basic", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        string? username;
        string? password;

        try
        {
            var credentialBytes = Convert.FromBase64String(headerValue.Parameter ?? string.Empty);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            username = credentials.FirstOrDefault();
            password = credentials.Length > 1 ? credentials[1] : null;
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization header.");
        }

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return AuthenticateResult.Fail("Missing credentials.");
        }

        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null || !user.IsActive)
        {
            return AuthenticateResult.Fail("Invalid username or password.");
        }

        if (!_passwordHasher.Verify(password, user.PasswordHash))
        {
            return AuthenticateResult.Fail("Invalid username or password.");
        }

        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
