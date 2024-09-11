using System.Text;

using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints;
using GolfLeague.Api.Health;
using GolfLeague.Api.Logging;
using GolfLeague.Api.Mapping;
using GolfLeague.Api.Options;
using GolfLeague.Api.Swagger;
using GolfLeague.Application;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Serilog;

using Swashbuckle.AspNetCore.SwaggerGen;

Log.Logger = ApiLogging.CreateBootstrapLogger();

try
{
    Log.Information("Golf League API Application starting...");

    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Host.UseSerilog((context, logConfig) =>
            logConfig.ReadFrom.Configuration(context.Configuration));

        using var config = builder.Configuration;

        var jwtOptions = new JwtOptions();
        config.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Key));
                options.TokenValidationParameters.ValidIssuer = jwtOptions.Issuer;
                options.TokenValidationParameters.ValidAudience = jwtOptions.Audience;
                options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(5);
                options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                options.TokenValidationParameters.ValidateLifetime = true;
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateAudience = true;
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AuthConstants.AdminPolicyName, p => p.RequireClaim(AuthConstants.AdminClaimName, "true"))
            .AddPolicy(AuthConstants.TrustedPolicyName, p => p.RequireAssertion(c =>
                c.User.HasClaim(m => m is { Type: AuthConstants.AdminClaimName, Value: "true" })
                || c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        builder.Services
            .AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

        builder.Services.AddGolfLeagueDatabase(config.GetConnectionString("GolfLeagueDb"));
        builder.Services.AddGolfLeagueServices();
    }

    await using var app = builder.Build();
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseHealthChecks(new PathString("/_health"));

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ValidationMappingMiddleware>();
        app.MapApiEndpoints();
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}