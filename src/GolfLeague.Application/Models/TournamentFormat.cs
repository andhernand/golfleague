namespace GolfLeague.Application.Models;

public static class TournamentFormat
{
    public static readonly IEnumerable<string> Values =
    [
        "Match Play",
        "Stroke Play",
        "Best Ball",
        "Scramble",
        "Alternate Shot",
        "Four Ball",
        "Skins Game"
    ];
}