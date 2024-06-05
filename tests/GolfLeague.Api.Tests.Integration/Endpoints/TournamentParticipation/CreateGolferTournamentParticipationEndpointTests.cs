using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class CreateGolferTournamentParticipationEndpointTests(GolfApiFactory golfApiFactory)
    : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenDataIsCorrect_ShouldCreateTournamentParticipation()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var expectedGolfer = await Mother.CreateGolferAsync(client);
        var expectedTournament = await Mother.CreateTournamentAsync(client);
        var expectedYear = Mother.GenerateYear();
        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = expectedTournament.TournamentId, Year = expectedYear
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{expectedGolfer.GolferId}/tournamentparticipations",
            request);

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
    public async Task CreateGolferTournamentParticipation_WhenGolferIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' must not be empty."] }
        });

        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = createdTournament.TournamentId, Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{default(int)}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenGolferIdDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' does not exists in the system"] }
        });

        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = createdTournament.TournamentId, Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenTournamentIdInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' must not be empty."] }
        });

        var request = new CreateGolferTournamentParticipationRequest()
        {
            TournamentId = default, Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenTournamentIdDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' does not exists in the system"] }
        });

        var request = new CreateGolferTournamentParticipationRequest()
        {
            TournamentId = Mother.GeneratePositiveInteger(), Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenYearIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", ["'Year' must not be empty."] }
        });

        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = tournament.TournamentId, Year = default
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenYearIsLessThan1916_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);

        const int lowYear = 1910;
        var currentYear = DateTime.UtcNow.Year;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {lowYear}."] }
        });

        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = tournament.TournamentId, Year = lowYear
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenYearIsGreaterThanCurrentYear_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);

        var currentYear = DateTime.UtcNow.Year;
        var higherYear = currentYear + 3;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {higherYear}."] }
        });

        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = tournament.TournamentId, Year = higherYear
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client,
            golfer.GolferId,
            tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentParticipation", ["TournamentParticipation already exists in the system."] }
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = participation.TournamentId, Year = participation.Year
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenClientIsUnauthorized_ShouldFailWithUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = Mother.GeneratePositiveInteger(), Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateGolferTournamentParticipation_WhenInsufficientPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = Mother.GeneratePositiveInteger(), Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}