using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class UpdateTournamentEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdTournamentIds = [];

    [Fact]
    public async Task UpdateTournament_UpdatesTournament_WhenDataIsCorrect()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateUpdateTournamentRequest();
        var createTournamentResponse = await client.PostAsJsonAsync("/api/tournaments", createTournamentRequest);
        var createdTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();
        _createdTournamentIds.Add(createdTournament!.TournamentId);

        const string changedFormat = "Match Play";
        var updateTournamentRequest = Fakers.GenerateUpdateTournamentRequest(
            createdTournament.Name,
            changedFormat);

        // Act
        var response = await client.PutAsJsonAsync($"/api/tournaments/{createdTournament.TournamentId}", updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedGolfer = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        updatedGolfer!.TournamentId.Should().Be(createdTournament.TournamentId);
        updatedGolfer.Name.Should().Be(createdTournament.Name);
        updatedGolfer.Format.Should().Be(changedFormat);
    }

    [Fact]
    public async Task UpdateTournament_ReturnsNotFound_WhenTournamentDoesNotExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var updateRequest = Fakers.GenerateUpdateTournamentRequest();
        var badId = new Faker().Random.Int(min: 999_999, max: 9_999_999);

        // Act
        var response = await client.PutAsJsonAsync($"/api/tournaments/{badId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTournament_Fails_WhenNameIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournamentResponse = await client.PostAsJsonAsync("/api/tournaments", createTournamentRequest);
        var createdTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();
        _createdTournamentIds.Add(createdTournament!.TournamentId);

        var updateTournamentRequest = Fakers.GenerateUpdateTournamentRequest(
            name: "",
            createdTournament.Format);

        // Act
        var response = await client.PutAsJsonAsync($"/api/tournaments/{createdTournament.TournamentId}", updateTournamentRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("'Name' must not be empty.");
    }

    [Fact]
    public async Task UpdateTournament_Fails_WhenFormatIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var updateTournamentRequest = Fakers.GenerateUpdateTournamentRequest();
        var updateTournamentResponse = await client.PostAsJsonAsync("/api/tournaments", updateTournamentRequest);
        var createdTournament = await updateTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();
        _createdTournamentIds.Add(createdTournament!.TournamentId);

        var updateGolferRequest = Fakers.GenerateUpdateTournamentRequest(
            createdTournament.Name,
            format: "");

        // Act
        var response = await client.PutAsJsonAsync($"/api/tournaments/{createdTournament.TournamentId}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Format");
        error.ErrorMessage.Should().Be("'Format' must not be empty.");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var httpClient = golfApiFactory.CreateClient();
        foreach (var tournamentId in _createdTournamentIds)
        {
            await httpClient.DeleteAsync($"/api/tournaments/{tournamentId}");
        }
    }
}