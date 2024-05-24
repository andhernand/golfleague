using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class GetAllTournamentsEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdTournamentIds = [];

    [Fact]
    public async Task GetAllTournaments_ReturnTournaments_WhenTournamentsExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournamentResponse = await client.PostAsJsonAsync("/api/tournaments", createTournamentRequest);
        var createdTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();
        _createdTournamentIds.Add(createdTournament!.TournamentId);

        // Act
        var response = await client.GetAsync("/api/tournaments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournaments = await response.Content.ReadFromJsonAsync<TournamentsResponse>();
        tournaments!.Tournaments.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllTournaments_ReturnsNoTournaments_WhenNoTournamentsExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/tournaments");
        var tournaments = await response.Content.ReadFromJsonAsync<TournamentsResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        tournaments!.Tournaments.Should().BeEmpty();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using HttpClient httpClient = golfApiFactory.CreateClient();
        foreach (int tournamentId in _createdTournamentIds)
        {
            await httpClient.DeleteAsync($"/api/tournaments/{tournamentId}");
        }
    }
}