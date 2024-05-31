using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Bogus;

using Microsoft.IdentityModel.Tokens;

using Serilog;

namespace Identity.Api;

public static class JwtGenerator
{
    private static readonly byte[] Key;
    private static readonly string CustomIssuer;
    private static readonly string CustomAudience;
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);
    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    private static readonly Serilog.ILogger Logger;

    static JwtGenerator()
    {
        Logger = Log.ForContext(typeof(JwtGenerator));

        var keyString = ConfigurationHelper.Configuration["JwtSettings:Key"];
        if (string.IsNullOrEmpty(keyString))
        {
            var ex = new InvalidOperationException("JWT Key not found in configuration");
            Logger.Error(ex, "JWT Key not found in configuration");
            throw ex;
        }

        Key = System.Text.Encoding.UTF8.GetBytes(keyString);

        var issuer = ConfigurationHelper.Configuration["JwtSettings:Issuer"]!;
        if (string.IsNullOrEmpty(issuer))
        {
            var ex = new InvalidOperationException("JWT Issuer not found in configuration");
            Logger.Error(ex, "JWT Issuer not found in configuration");
            throw ex;
        }

        CustomIssuer = issuer;

        var audience = ConfigurationHelper.Configuration["JwtSettings:Audience"];
        if (string.IsNullOrEmpty(audience))
        {
            var ex = new InvalidOperationException("JWT Audience not found in configuration");
            Logger.Error(ex, "JWT Audience not found in configuration");
            throw ex;
        }

        CustomAudience = audience;
    }

    public static string GenerateToken(
        bool isAdmin = false,
        bool isTrusted = false,
        TimeSpan? customTimeSpan = default)
    {
        try
        {
            Logger.Verbose("Generating JWT Token");

            var faker = new Faker();
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, faker.Random.AlphaNumeric(8)),
                new(JwtRegisteredClaimNames.Sub, faker.Person.UserName),
                new(JwtRegisteredClaimNames.Email, faker.Person.Email),
                new(CustomClaimNames.UserId, faker.Random.Int(1).ToString())
            };

            if (isAdmin) { claims.Add(new Claim(CustomClaimNames.Admin, "true", ClaimValueTypes.Boolean)); }

            if (isTrusted) { claims.Add(new Claim(CustomClaimNames.TrustedUser, "true", ClaimValueTypes.Boolean)); }

            var expiresAt = customTimeSpan is not null
                ? DateTime.UtcNow.Add(customTimeSpan.Value)
                : DateTime.UtcNow.Add(TokenLifetime);

            Logger.Verbose("The JWT Token expires at: {ExpiresAt}", expiresAt);

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
            Logger.Error(ex, "Failed to generate JWT Token");
            throw new InvalidOperationException("Failed to generate JWT", ex);
        }
    }

    private static class CustomClaimNames
    {
        public const string UserId = "userid";
        public const string Admin = "admin";
        public const string TrustedUser = "trusted_user";
    }
}