using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class CreateTournamentParticipationEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task CreateTournamentParticipation_CreatesTournamentParticipation_WhenDataIsCorrect()
    {
        // Arrange
        using var httpClient = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createGolferResponse = await httpClient
            .PostAsJsonAsync(golfApiFactory.GolfersApiBasePath, createGolferRequest);
        var expectedGolfer = await createGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournamentResponse = await httpClient
            .PostAsJsonAsync(golfApiFactory.TournamentsApiBasePath, createTournamentRequest);
        var expectedTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        var expectedYear = Fakers.GenerateYear();

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = expectedGolfer!.GolferId,
            TournamentId = expectedTournament!.TournamentId,
            Year = expectedYear
        };

        // Act
        var response = await httpClient.PostAsJsonAsync(golfApiFactory.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();

        response.Headers.Location.Should().Be(
            $"http://localhost{golfApiFactory.TournamentParticipationsApiBasePath}"
            + $"?golferId={actual!.GolferId}"
            + $"&tournamentId={actual.TournamentId}"
            + $"&year={actual.Year}");

        actual!.GolferId.Should().Be(expectedGolfer.GolferId);
        actual.TournamentId.Should().Be(expectedTournament.TournamentId);
        actual.Year.Should().Be(expectedYear);
    }

    [Fact]
    public async Task CreateTournamentParticipation_Fails_WhenGolferIdIsInvalid()
    {
        // Arrange
        using var httpClient = golfApiFactory.CreateClient();

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = default, TournamentId = 12323, Year = 2015
        };

        // Act
        var response = await httpClient.PostAsJsonAsync(golfApiFactory.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();
        error.PropertyName.Should().Be("GolferId");
        error.ErrorMessage.Should().Be("'Golfer Id' must not be empty.");
    }

    [Fact]
    public async Task CreateTournamentParticipation_Fails_WhenTournamentIdInvalid()
    {
        // Arrange
        using var httpClient = golfApiFactory.CreateClient();

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = 23112, TournamentId = default, Year = 2015
        };

        // Act
        var response = await httpClient.PostAsJsonAsync(golfApiFactory.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();
        error.PropertyName.Should().Be("TournamentId");
        error.ErrorMessage.Should().Be("'Tournament Id' must not be empty.");
    }

    [Fact]
    public async Task CreateTournamentParticipation_Fails_WhenYearIsInvalid()
    {
        // Arrange
        using var httpClient = golfApiFactory.CreateClient();

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = 3214, TournamentId = 12323, Year = default
        };

        // Act
        var response = await httpClient.PostAsJsonAsync(golfApiFactory.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();
        error.PropertyName.Should().Be("Year");
        error.ErrorMessage.Should().Be("'Year' must not be empty.");
    }

    [Fact]
    public async Task CreateTournamentParticipation_Fails_WhenTournamentParticipationAlreadyExists()
    {
        // Arrange
        using var httpClient = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createGolferResponse = await httpClient
            .PostAsJsonAsync(golfApiFactory.GolfersApiBasePath, createGolferRequest);
        var expectedGolfer = await createGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournamentResponse = await httpClient
            .PostAsJsonAsync(golfApiFactory.TournamentsApiBasePath, createTournamentRequest);
        var expectedTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        var expectedYear = Fakers.GenerateYear();

        var createParticipationRequest = new CreateTournamentParticipationsRequest
        {
            GolferId = expectedGolfer!.GolferId,
            TournamentId = expectedTournament!.TournamentId,
            Year = expectedYear
        };

        var createdTournamentParticipationResponse = await httpClient
            .PostAsJsonAsync(golfApiFactory.TournamentParticipationsApiBasePath, createParticipationRequest);
        var createdTournamentParticipation = await createdTournamentParticipationResponse.Content
            .ReadFromJsonAsync<TournamentParticipationResponse>();

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = createdTournamentParticipation!.GolferId,
            TournamentId = createdTournamentParticipation.TournamentId,
            Year = createdTournamentParticipation.Year
        };

        // Act
        var response = await httpClient.PostAsJsonAsync(golfApiFactory.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();
        error.PropertyName.Should().Be("TournamentParticipation");
        error.ErrorMessage.Should().Be("TournamentParticipation already exists in the system.");
    }
}