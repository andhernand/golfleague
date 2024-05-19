using GolfLeague.Api.Endpoints;
using GolfLeague.Application;

var builder = WebApplication.CreateBuilder(args);
using var config = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();
builder.Services.AddHealthChecks();

builder.Services.AddGolfLeagueApplication();
builder.Services.AddGolfLeagueDatabase(config["Database:ConnectionString"]!);

await using var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.MapApiEndpoints();

app.Run();