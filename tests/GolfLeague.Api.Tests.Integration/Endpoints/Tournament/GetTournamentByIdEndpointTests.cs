using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class GetTournamentByIdEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdTournamentIds = [];

    [Fact]
    public async Task GetTournamentById_ReturnsTournament_WhenTournamentExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournament = await client.PostAsJsonAsync("/api/tournaments", createTournamentRequest);
        var createdTournament = await createTournament.Content.ReadFromJsonAsync<TournamentResponse>();
        _createdTournamentIds.Add(createdTournament!.TournamentId);

        // Act
        var response = await client.GetAsync($"/api/tournaments/{createdTournament.TournamentId}");

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
        int tournamentId = new Faker().Random.Int(999_999, 9_999_999);

        // Act
        var result = await client.GetAsync($"/api/tournaments/{tournamentId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var httpClient = golfApiFactory.CreateClient();
        foreach (int tourmamentId in _createdTournamentIds)
        {
            await httpClient.DeleteAsync($"/api/tournaments/{tourmamentId}");
        }
    }
}