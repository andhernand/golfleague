using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class CreateTournamentParticipationEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task CreateTournamentParticipation_WhenDataIsCorrect_ShouldCreateTournamentParticipation()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var expectedGolfer = await Mother.CreateGolferAsync(client);
        var expectedTournament = await Mother.CreateTournamentAsync(client);
        var expectedYear = Mother.GenerateYear();
        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = expectedGolfer.GolferId, TournamentId = expectedTournament.TournamentId, Year = expectedYear
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();

        response.Headers.Location.Should().Be(
            $"http://localhost{Mother.TournamentParticipationsApiBasePath}"
            + $"?golferId={actual!.GolferId}"
            + $"&tournamentId={actual.TournamentId}"
            + $"&year={actual.Year}");

        actual!.GolferId.Should().Be(expectedGolfer.GolferId);
        actual.TournamentId.Should().Be(expectedTournament.TournamentId);
        actual.Year.Should().Be(expectedYear);
    }

    [Fact]
    public async Task CreateTournamentParticipation_WhenGolferIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' must not be empty."] }
        });

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = default, TournamentId = 12323, Year = 2015
        };

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentParticipation_WhenTournamentIdInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' must not be empty."] }
        });

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = 23112, TournamentId = default, Year = 2015
        };

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentParticipation_WhenYearIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", ["'Year' must not be empty."] }
        });

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = 3214, TournamentId = 12323, Year = default
        };

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentParticipation_WhenTournamentParticipationAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        var createdTournamentParticipation = await Mother.CreateTournamentParticipationAsync(
            client,
            createdGolfer.GolferId,
            createdTournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentParticipation", ["TournamentParticipation already exists in the system."] }
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = createdTournamentParticipation.GolferId,
            TournamentId = createdTournamentParticipation.TournamentId,
            Year = createdTournamentParticipation.Year
        };

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentParticipation_WhenClientIsUnauthorized_ShouldFailWithUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = Mother.GeneratePositiveInteger(),
            TournamentId = Mother.GeneratePositiveInteger(),
            Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTournamentParticipation_WhenClientDoesNotHaveProperPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var request = new CreateTournamentParticipationsRequest
        {
            GolferId = Mother.GeneratePositiveInteger(),
            TournamentId = Mother.GeneratePositiveInteger(),
            Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentParticipationsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}