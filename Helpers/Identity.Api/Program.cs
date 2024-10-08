using Identity.Api.Endpoints;
using Identity.Api.Logging;
using Identity.Api.Options;
using Identity.Api.Services;

using Serilog;

Log.Logger = IdentityApiLogging.CreateBootstrapLogger();

try
{
    Log.Information("Identity API application is starting...");

    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Host.UseSerilog((context, logConfig) =>
            logConfig.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddOptions<JwtOptions>()
            .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton<JwtService>();
    }

    await using var app = builder.Build();
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();
        app.MapCreateToken();
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Identity API application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}