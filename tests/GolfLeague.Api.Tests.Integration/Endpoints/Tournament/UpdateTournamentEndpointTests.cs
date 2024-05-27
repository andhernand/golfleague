using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class UpdateTournamentEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    private readonly string _tournamentsApiPath = "/api/tournaments";

    [Fact]
    public async Task UpdateTournament_UpdatesTournament_WhenDataIsCorrect()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateUpdateTournamentRequest();
        var createTournamentResponse = await client.PostAsJsonAsync(_tournamentsApiPath, createTournamentRequest);
        var createdTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        const string changedFormat = "Match Play";
        var updateTournamentRequest = Fakers.GenerateUpdateTournamentRequest(
            createdTournament!.Name,
            changedFormat);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{_tournamentsApiPath}/{createdTournament.TournamentId}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        updatedTournament!.TournamentId.Should().Be(createdTournament.TournamentId);
        updatedTournament.Name.Should().Be(createdTournament.Name);
        updatedTournament.Format.Should().Be(changedFormat);
    }

    [Fact]
    public async Task UpdateTournament_ReturnsNotFound_WhenTournamentDoesNotExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var updateRequest = Fakers.GenerateUpdateTournamentRequest();
        var badId = new Faker().Random.Int(min: 999_999, max: 9_999_999);

        // Act
        var response = await client.PutAsJsonAsync($"{_tournamentsApiPath}/{badId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTournament_Fails_WhenNameIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournamentResponse = await client.PostAsJsonAsync(_tournamentsApiPath, createTournamentRequest);
        var createdTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        var updateTournamentRequest = Fakers.GenerateUpdateTournamentRequest(
            name: "",
            createdTournament!.Format);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{_tournamentsApiPath}/{createdTournament.TournamentId}",
            updateTournamentRequest);

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

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournamentResponse = await client.PostAsJsonAsync(_tournamentsApiPath, createTournamentRequest);
        var createdTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        var updateTournamentRequest = Fakers.GenerateUpdateTournamentRequest(
            createdTournament!.Name,
            format: "");

        // Act
        var response = await client.PutAsJsonAsync(
            $"{_tournamentsApiPath}/{createdTournament.TournamentId}",
            updateTournamentRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Format");
        error.ErrorMessage.Should().Be("'Format' must not be empty.");
    }

    [Fact]
    public async Task UpdateTournament_Fails_WhenTournamentWithNameAndFormatAlreadyExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var firstCreateTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var firstCreateTournamentResponse = await client
            .PostAsJsonAsync(_tournamentsApiPath, firstCreateTournamentRequest);
        var firstCreatedTournament = await firstCreateTournamentResponse
            .Content.ReadFromJsonAsync<TournamentResponse>();

        var secondCreateTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var secondCreateTournamentResponse =
            await client.PostAsJsonAsync(_tournamentsApiPath, secondCreateTournamentRequest);
        var secondCreatedTournament = await secondCreateTournamentResponse
            .Content.ReadFromJsonAsync<TournamentResponse>();

        var updateTournamentRequest = Fakers.GenerateUpdateTournamentRequest(
            firstCreatedTournament!.Name,
            firstCreateTournamentRequest.Format);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{_tournamentsApiPath}/{secondCreatedTournament!.TournamentId}",
            updateTournamentRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Tournament");
        error.ErrorMessage.Should()
            .Be("A Tournament with the Name and Format combination already exists in the system.");
    }
}