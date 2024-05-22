namespace GolfLeague.Application.Models;

public class Golfer
{
    public int GolferId { get; set; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required DateTime JoinDate { get; init; }
    public int? Handicap { get; init; }
}