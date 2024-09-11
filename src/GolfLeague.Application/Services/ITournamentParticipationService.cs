using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Application.Services;

public interface ITournamentParticipationService
{
    Task<TournamentParticipationResponse?> CreateAsync(
        int golferId,
        CreateParticipationDetailRequest request,
        CancellationToken token = default);

    Task<TournamentParticipationResponse?> CreateAsync(
        int tournamentId,
        CreateTournamentDetailRequest request,
        CancellationToken token = default);

    Task<bool> DeleteAsync(
        int golferId,
        int tournamentId,
        int year,
        CancellationToken cancellationToken = default);
}