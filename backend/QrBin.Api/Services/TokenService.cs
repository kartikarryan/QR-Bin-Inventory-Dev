using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QrBin.Api.Common.Options;
using QrBin.Data.Entities;

namespace QrBin.Api.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateManagerToken(ManagerUser manager);
}

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string Token, DateTime ExpiresAt) CreateManagerToken(ManagerUser manager)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ManagerTokenExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, manager.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, manager.Email ?? string.Empty),
            new(ClaimTypes.Name, manager.FullName),
            new(ClaimTypes.Role, "Manager"),
            new("orgId", manager.OrganizationId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
