using FluentValidation;

using GolfLeague.Application.Database;
using GolfLeague.Application.Repositories;
using GolfLeague.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace GolfLeague.Application;

public static class GolfLeagueServiceCollectionExtensions
{
    public static IServiceCollection AddGolfLeagueApplication(this IServiceCollection services)
    {
        services.AddSingleton<IGolferService, GolferService>();
        services.AddSingleton<IGolferRepository, GolferRepository>();
        services.AddValidatorsFromAssemblyContaining<IGolfLeagueApplicationMarker>(ServiceLifetime.Singleton);
        return services;
    }

    public static IServiceCollection AddGolfLeagueDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ =>
            new SqlServerDbConnectionFactory(connectionString));
        return services;
    }
}