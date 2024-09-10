namespace GolfLeague.Application.Models;

public record Tournament
{
    public int TournamentId { get; set; }
    public required string Name { get; init; }
    public required string Format { get; init; }
    public List<ParticipationDetail> Participants { get; init; } = [];
}