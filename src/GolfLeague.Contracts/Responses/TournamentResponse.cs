// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GolfLeague.Contracts.Responses;

public class TournamentResponse
{
    public required int TournamentId { get; init; }
    public required string Name { get; init; }
    public required string Format { get; init; }
    public required IEnumerable<ParticipationDetailResponse> Participants { get; init; } = [];
}