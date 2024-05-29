namespace GolfLeague.Application.Models;

public class Golfer
{
    public int GolferId { get; set; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required DateOnly JoinDate { get; init; }
    public int? Handicap { get; init; }
    public List<TournamentDetail> Tournaments { get; init; } = [];
}