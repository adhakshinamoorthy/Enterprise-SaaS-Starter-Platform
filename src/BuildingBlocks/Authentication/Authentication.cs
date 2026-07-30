using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseSaaS.Authentication;

public sealed class JwtOptions { public const string SectionName = "Jwt"; public required string Issuer { get; init; } public required string Audience { get; init; } public required string SigningKey { get; init; } public int AccessTokenMinutes { get; init; } = 15; public int RefreshTokenDays { get; init; } = 7; }
public sealed record TokenResult(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
public interface ITokenService { TokenResult Create(Guid userId, Guid tenantId, string email, IEnumerable<string> roles, IEnumerable<string> permissions); }
public sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    public TokenResult Create(Guid userId, Guid tenantId, string email, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var now = DateTimeOffset.UtcNow; var expires = now.AddMinutes(options.Value.AccessTokenMinutes);
        var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, userId.ToString()), new(JwtRegisteredClaimNames.Email, email), new("tenant_id", tenantId.ToString()), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) };
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x))); claims.AddRange(permissions.Select(x => new Claim("permission", x)));
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SigningKey)), SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(options.Value.Issuer, options.Value.Audience, claims, now.UtcDateTime, expires.UtcDateTime, credentials);
        return new(new JwtSecurityTokenHandler().WriteToken(jwt), Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)), expires);
    }
}
public interface IOAuthProvider { string Name { get; } Uri GetAuthorizationUri(string state, string redirectUri); Task<OAuthIdentity> ExchangeAsync(string code, string redirectUri, CancellationToken cancellationToken); }
public sealed record OAuthIdentity(string Provider, string Subject, string Email, string DisplayName);
