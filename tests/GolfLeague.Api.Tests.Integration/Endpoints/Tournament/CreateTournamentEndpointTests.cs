using System.Net;

using FluentAssertions;

using GolfLeague.Application.Models;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class CreateTournamentEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    private readonly string _tournamentsApiPath = "/api/tournaments";

    [Fact]
    public async Task CreateTournament_CreatesTournament_WhenDataIsCorrect()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Fakers.GenerateCreateTournamentRequest();

        // Act
        var response = await client.PostAsJsonAsync(_tournamentsApiPath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();

        response.Headers.Location.Should().Be($"http://localhost{_tournamentsApiPath}/{tournament!.TournamentId}");
        tournament.TournamentId.Should().NotBe(default);
        tournament.Name.Should().Be(request.Name);
        tournament.Format.Should().Be(request.Format);
    }

    [Fact]
    public async Task CreateTournament_Fails_WhenNameIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Fakers.GenerateCreateTournamentRequest(name: "");

        // Act
        var response = await client.PostAsJsonAsync(_tournamentsApiPath, request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("'Name' must not be empty.");
    }

    [Fact]
    public async Task CreateTournament_Fails_WhenFormatIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Fakers.GenerateCreateTournamentRequest(format: "");

        // Act
        var response = await client.PostAsJsonAsync(_tournamentsApiPath, request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Format");
        error.ErrorMessage.Should().Be("'Format' must not be empty.");
    }

    [Fact]
    public async Task CreateTournament_Fails_WhenTournamentWithNameAndFormatAlreadyExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournamentResponse = await client.PostAsJsonAsync(_tournamentsApiPath, createTournamentRequest);
        var createdTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        var request = Fakers.GenerateCreateTournamentRequest(createdTournament!.Name, createdTournament.Format);

        // Act
        var response = await client.PostAsJsonAsync(_tournamentsApiPath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        error.PropertyName.Should().Be("Tournament");
        error.ErrorMessage.Should()
            .Be("A Tournament with the Name and Format combination already exists in the system.");
    }

    [Fact]
    public async Task CreateTournament_Fails_WhenTournamentFormatIsNotAnAcceptableValue()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Fakers.GenerateCreateTournamentRequest(format: "Yo Momma");

        // Act
        var response = await client.PostAsJsonAsync(_tournamentsApiPath, request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Format");
        error.ErrorMessage.Should().Be($"'Format' must be one of any: {string.Join(", ", TournamentFormat.Values)}");
    }
}