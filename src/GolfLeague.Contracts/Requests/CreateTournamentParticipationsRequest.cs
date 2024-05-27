namespace GolfLeague.Contracts.Requests;

public class CreateTournamentParticipationsRequest
{
    public required int GolferId { get; init; }
    public required int TournamentId { get; init; }
    public required int Year { get; set; }
}