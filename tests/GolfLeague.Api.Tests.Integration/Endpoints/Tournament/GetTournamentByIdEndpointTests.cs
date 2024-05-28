using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class GetTournamentByIdEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    private readonly string _tournamentsApiPath = "/api/tournaments";

    [Fact]
    public async Task GetTournamentById_ReturnsTournament_WhenTournamentExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournament = await client.PostAsJsonAsync(_tournamentsApiPath, createTournamentRequest);
        var createdTournament = await createTournament.Content.ReadFromJsonAsync<TournamentResponse>();

        // Act
        var response = await client.GetAsync($"{_tournamentsApiPath}/{createdTournament!.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        tournament!.TournamentId.Should().Be(createdTournament.TournamentId);
        tournament.Name.Should().Be(createdTournament.Name);
        tournament.Format.Should().Be(createdTournament.Format);
    }

    [Fact]
    public async Task GetTournamentById_ReturnsNotFound_WhenTournamentDoesNotExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int tournamentId = Fakers.GeneratePositiveInteger(999_999, 9_999_999);

        // Act
        var result = await client.GetAsync($"{_tournamentsApiPath}/{tournamentId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}