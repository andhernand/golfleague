namespace GolfLeague.Contracts.Requests;

public class CreateTournamentGolferParticipationRequest
{
    public int GolferId { get; set; }
    public int Year { get; set; }
}