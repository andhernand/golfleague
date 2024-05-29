using GolfLeague.Api.Endpoints;
using GolfLeague.Api.Health;
using GolfLeague.Api.Mapping;
using GolfLeague.Api.OpenApi;
using GolfLeague.Application;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
using var config = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<CreateTournamentRequestSchemaFilter>();
    options.SchemaFilter<UpdateTournamentRequestSchemaFilter>();
    options.SchemaFilter<TournamentResponseSchemaFilter>();
});

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

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapApiEndpoints();

app.Run();