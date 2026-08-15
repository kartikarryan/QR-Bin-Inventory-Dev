using System.IdentityModel.Tokens.Jwt;
using QrBin.Api.Exceptions;

namespace QrBin.Api.Services;

public interface ICurrentManagerContext
{
    int ManagerId { get; }
    int OrganizationId { get; }
}

/// <summary>
/// Reads the authenticated manager's identity straight off the JWT claims already validated
/// by the auth middleware — no DB round trip needed since OrganizationId is embedded at login.
/// </summary>
public class CurrentManagerContext : ICurrentManagerContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentManagerContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int ManagerId => GetRequiredClaim(JwtRegisteredClaimNames.Sub);
    public int OrganizationId => GetRequiredClaim("orgId");

    private int GetRequiredClaim(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;
        if (string.IsNullOrEmpty(value) || !int.TryParse(value, out var result))
            throw new UnauthorizedException("No valid manager identity found in token.");

        return result;
    }
}
