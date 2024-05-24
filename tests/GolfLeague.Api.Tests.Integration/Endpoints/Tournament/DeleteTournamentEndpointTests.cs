using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class DeleteTournamentEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdTournamentIds = [];
    private readonly string _tournamentsApiPath = "/api/tournaments";

    [Fact]
    public async Task DeleteTournament_ReturnsNoContent_WhenTournamentIsDeleted()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createdTournamentResponse = await client.PostAsJsonAsync(_tournamentsApiPath, createTournamentRequest);
        var createdTournament = await createdTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();
        _createdTournamentIds.Add(createdTournament!.TournamentId);

        // Act
        var response = await client.DeleteAsync($"{_tournamentsApiPath}/{createdTournament.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTournament_ReturnsNotFound_WhenTournamentDoesNotExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int tournamentId = new Faker().Random.Int(999_999, 9_999_999);

        // Act
        var response = await client.DeleteAsync($"{_tournamentsApiPath}/{tournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var httpClient = golfApiFactory.CreateClient();
        foreach (var tournamentId in _createdTournamentIds)
        {
            await httpClient.DeleteAsync($"{_tournamentsApiPath}/{tournamentId}");
        }
    }
}