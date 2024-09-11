using FluentValidation;

using GolfLeague.Application.Mapping;
using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Application.Services;

public class TournamentParticipationService(
    ITournamentParticipationRepository repository,
    IValidator<TournamentParticipation> validator)
    : ITournamentParticipationService
{
    public async Task<TournamentParticipationResponse?> CreateAsync(
        int golferId,
        CreateParticipationDetailRequest request,
        CancellationToken token = default)
    {
        var participation = request.MapToTournamentParticipation(golferId);
        var result = await CreateAsync(participation, token);
        return result?.MapToResponse();
    }

    public async Task<TournamentParticipationResponse?> CreateAsync(
        int tournamentId,
        CreateTournamentDetailRequest request,
        CancellationToken token = default)
    {
        var participation = request.MapToTournamentParticipation(tournamentId);
        var result = await CreateAsync(participation, token);
        return result?.MapToResponse();
    }

    public async Task<bool> DeleteAsync(
        int golferId,
        int tournamentId,
        int year,
        CancellationToken cancellationToken = default)
    {
        var participation = new TournamentParticipation
        {
            GolferId = golferId, TournamentId = tournamentId, Year = year
        };
        return await repository.DeleteAsync(participation, cancellationToken);
    }

    private async Task<TournamentParticipation?> CreateAsync(
        TournamentParticipation tournamentParticipation,
        CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(tournamentParticipation, token);
        return await repository.CreateAsync(tournamentParticipation, token);
    }
}