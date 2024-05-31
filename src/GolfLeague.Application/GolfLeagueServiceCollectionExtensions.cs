using Dapper;

using FluentValidation;

using GolfLeague.Application.Database;
using GolfLeague.Application.Repositories;
using GolfLeague.Application.Services;

using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMethodReturnValue.Global

namespace GolfLeague.Application;

public static class GolfLeagueServiceCollectionExtensions
{
    public static IServiceCollection AddGolfLeagueApplication(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        services.AddSingleton<IGolferService, GolferService>();
        services.AddSingleton<IGolferRepository, GolferRepository>();

        services.AddSingleton<ITournamentService, TournamentService>();
        services.AddSingleton<ITournamentRepository, TournamentRepository>();

        services.AddSingleton<ITournamentParticipationService, TournamentParticipationService>();
        services.AddSingleton<ITournamentParticipationRepository, TournamentParticipationRepository>();

        services.AddValidatorsFromAssemblyContaining<IGolfLeagueApplicationMarker>(ServiceLifetime.Singleton);
        return services;
    }

    public static IServiceCollection AddGolfLeagueDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerDbConnectionFactory(connectionString));
        return services;
    }
}