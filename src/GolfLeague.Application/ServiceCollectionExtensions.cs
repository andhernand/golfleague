using Dapper;

using FluentValidation;

using GolfLeague.Application.Database;
using GolfLeague.Application.Repositories;
using GolfLeague.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace GolfLeague.Application;

public static class ServiceCollectionExtensions
{
    public static void AddGolfLeagueServices(this IServiceCollection services)
    {
        services.AddSingleton<IGolferRepository, GolferRepository>();
        services.AddSingleton<ITournamentRepository, TournamentRepository>();
        services.AddSingleton<ITournamentParticipationRepository, TournamentParticipationRepository>();

        services.AddTransient<IGolferService, GolferService>();
        services.AddTransient<ITournamentService, TournamentService>();
        services.AddTransient<ITournamentParticipationService, TournamentParticipationService>();

        services.AddValidatorsFromAssemblyContaining<IGolfLeagueApplicationMarker>(ServiceLifetime.Singleton);
    }

    public static void AddGolfLeagueDatabase(this IServiceCollection services, string? connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerDbConnectionFactory(connectionString));
    }
}