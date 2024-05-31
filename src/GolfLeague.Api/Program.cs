using System.Text;

using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints;
using GolfLeague.Api.Health;
using GolfLeague.Api.Mapping;
using GolfLeague.Api.Swagger;
using GolfLeague.Application;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Serilog;

using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!));
        options.TokenValidationParameters.ValidIssuer = config["JwtSettings:Issuer"];
        options.TokenValidationParameters.ValidAudience = config["JwtSettings:Audience"];
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

builder.Services.AddGolfLeagueApplication();
builder.Services.AddGolfLeagueDatabase(config["Database:ConnectionString"]!);

await using var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHealthChecks(new PathString("/_health"));

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();
app.UseMiddleware<ValidationMappingMiddleware>();
app.MapApiEndpoints();

app.Run();