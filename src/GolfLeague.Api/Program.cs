using GolfLeague.Api.Endpoints;
using GolfLeague.Api.Mapping;
using GolfLeague.Application;

var builder = WebApplication.CreateBuilder(args);
using var config = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddGolfLeagueApplication();
builder.Services.AddGolfLeagueDatabase(config["Database:ConnectionString"]!);

await using var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapApiEndpoints();

app.Run();