using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Bogus;

using Microsoft.IdentityModel.Tokens;

namespace GolfLeague.Api.Tests.Integration;

public static class JwtGenerator
{
    private static readonly byte[] Key;
    private static readonly string CustomIssuer;
    private static readonly string CustomAudience;
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(2);
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    static JwtGenerator()
    {
        Key = Encoding.UTF8.GetBytes(GetConfigurationValueOrThrow("JwtSettings:Key"));
        CustomIssuer = GetConfigurationValueOrThrow("JwtSettings:Issuer");
        CustomAudience = GetConfigurationValueOrThrow("JwtSettings:Audience");
    }

    public static string GenerateToken(
        bool isAdmin = false,
        bool isTrusted = false,
        TimeSpan? customTimeSpan = default)
    {
        try
        {
            var faker = new Faker();
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, faker.Random.AlphaNumeric(8)),
                new(JwtRegisteredClaimNames.Sub, faker.Person.UserName),
                new(JwtRegisteredClaimNames.Email, faker.Person.Email),
                new(JwtRegisteredClaimNames.UniqueName, faker.Person.Email),
                new(CustomClaimNames.UserId, faker.Random.Int(1).ToString())
            };

            if (isAdmin) { claims.Add(new Claim(CustomClaimNames.Admin, "true", ClaimValueTypes.Boolean)); }

            if (isTrusted) { claims.Add(new Claim(CustomClaimNames.TrustedUser, "true", ClaimValueTypes.Boolean)); }

            var expiresAt = customTimeSpan is not null
                ? DateTime.UtcNow.Add(customTimeSpan.Value)
                : DateTime.UtcNow.Add(TokenLifetime);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = CustomIssuer,
                Audience = CustomAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = TokenHandler.CreateToken(tokenDescriptor);
            var jwt = TokenHandler.WriteToken(token);
            return jwt;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to generate JWT", ex);
        }
    }

    private static class CustomClaimNames
    {
        public const string UserId = "userid";
        public const string Admin = "admin";
        public const string TrustedUser = "trusted_user";
    }

    private static string GetConfigurationValueOrThrow(string key)
    {
        return ConfigurationHelper.Configuration[key]
               ?? throw new InvalidOperationException($"{key} not found configuration");
    }
}