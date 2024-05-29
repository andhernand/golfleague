namespace GolfLeague.Application.Models;

public class Tournament
{
    public int TournamentId { get; set; }
    public required string Name { get; init; }
    public required string Format { get; init; }
    public List<ParticipationDetail> Participants { get; init; } = [];
}