using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Bogus;

using Identity.Api.Options;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Services;

public class JwtService(IOptions<JwtOptions> options, Serilog.ILogger logger)
{
    private const string UserIdClaim = "userid";
    private const string AdminClaim = "admin";
    private const string TrustedUserClaim = "trusted_user";

    private readonly byte[] _key = Encoding.UTF8.GetBytes(options.Value.Key);
    private readonly string _customIssuer = options.Value.Issuer;
    private readonly string _customAudience = options.Value.Audience;
    private readonly Faker _faker = new();
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly Serilog.ILogger _logger = logger.ForContext<JwtService>();

    public string GenerateToken(bool isAdmin = false, bool isTrusted = false)
    {
        try
        {
            _logger.Information("Generating JWT Token");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, _faker.Random.AlphaNumeric(8)),
                new(JwtRegisteredClaimNames.Sub, _faker.Person.UserName),
                new(JwtRegisteredClaimNames.Email, _faker.Person.Email),
                new(UserIdClaim, _faker.Random.Int(1).ToString())
            };

            if (isAdmin) { claims.Add(new Claim(AdminClaim, "true", ClaimValueTypes.Boolean)); }

            if (isTrusted) { claims.Add(new Claim(TrustedUserClaim, "true", ClaimValueTypes.Boolean)); }

            var expiresAt = DateTime.UtcNow.Add(TimeSpan.FromHours(1));

            _logger.Information("The JWT expires at: {JwtExpiration}", expiresAt);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = _customIssuer,
                Audience = _customAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            var jwt = _tokenHandler.WriteToken(token);
            return jwt;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to generate JWT Token");
            throw new InvalidOperationException("Failed to generate JWT", ex);
        }
    }
}