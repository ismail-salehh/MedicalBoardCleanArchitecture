using MedicalBoard.Application.Authorization;
using MedicalBoard.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        => FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (PermissionCodes.All.Contains(policyName))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
            return Task.FromResult(policy);
        }
        return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();
}