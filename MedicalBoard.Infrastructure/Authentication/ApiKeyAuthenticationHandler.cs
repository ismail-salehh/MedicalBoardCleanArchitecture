using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MedicalBoard.Infrastructure.Authentication;

// Milestone 10 -- custom authentication exercise. Validates a header-based key against
// a controlled configuration source (appsettings / user-secrets / environment variable),
// never against a hard-coded production secret and never against raw passwords.
// Apply with [Authorize(AuthenticationSchemes = ApiKeyAuthenticationSchemeOptions.DefaultScheme)]
// on the specific internal endpoint that needs it.
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var providedKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        var configuredKey = _configuration["InternalApi:Key"];
        if (string.IsNullOrEmpty(configuredKey) || providedKey != configuredKey)
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "internal-service"),
            new Claim(ClaimTypes.NameIdentifier, "0"),
            new Claim(ClaimTypes.Role, "Service")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}