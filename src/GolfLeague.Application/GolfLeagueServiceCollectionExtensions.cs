using Dapper;

using FluentValidation;

using GolfLeague.Application.Database;
using GolfLeague.Application.Repositories;
using GolfLeague.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace GolfLeague.Application;

public static class GolfLeagueServiceCollectionExtensions
{
    public static void AddGolfLeagueApplication(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        services.AddSingleton<IGolferService, GolferService>();
        services.AddSingleton<IGolferRepository, GolferRepository>();

        services.AddSingleton<ITournamentService, TournamentService>();
        services.AddSingleton<ITournamentRepository, TournamentRepository>();

        services.AddSingleton<ITournamentParticipationService, TournamentParticipationService>();
        services.AddSingleton<ITournamentParticipationRepository, TournamentParticipationRepository>();

        services.AddValidatorsFromAssemblyContaining<IGolfLeagueApplicationMarker>(ServiceLifetime.Singleton);
    }

    public static void AddGolfLeagueDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerDbConnectionFactory(connectionString));
    }
}