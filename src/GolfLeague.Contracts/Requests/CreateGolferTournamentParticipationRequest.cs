namespace GolfLeague.Contracts.Requests;

public class CreateGolferTournamentParticipationRequest
{
    public int TournamentId { get; set; }
    public int Year { get; set; }
}