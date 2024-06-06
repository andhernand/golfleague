namespace GolfLeague.Application.Models;

public class ParticipationDetail
{
    public required int GolferId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required int Year { get; init; }
    public int? Score { get; init; }
}