using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class UpdateGolferTournamentParticipationEndpointTests(GolfApiFactory golfApiFactory)
    : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenDataIsCorrect_ShouldUpdateTournamentParticipation()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);
        var secondTournament = await Mother.CreateTournamentAsync(client);

        var expected = new TournamentParticipationResponse
        {
            TournamentId = secondTournament.TournamentId, GolferId = golfer.GolferId, Year = Mother.GenerateYear()
        };

        var request = new UpdateGolferTournamentParticipationRequest
        {
            OriginalTournamentId = tournament.TournamentId,
            OriginalYear = participation.Year,
            NewTournamentId = secondTournament.TournamentId,
            NewYear = expected.Year
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenGolferIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);
        var newTournament = await Mother.CreateTournamentAsync(client);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' must not be empty."] }
        });

        var request = new UpdateGolferTournamentParticipationRequest
        {
            OriginalTournamentId = tournament.TournamentId,
            OriginalYear = participation.Year,
            NewTournamentId = newTournament.TournamentId,
            NewYear = Mother.GenerateYear()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{default(int)}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenGolferDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);
        var newTournament = await Mother.CreateTournamentAsync(client);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' does not exists in the system"] }
        });

        var request = new UpdateGolferTournamentParticipationRequest
        {
            OriginalTournamentId = tournament.TournamentId,
            OriginalYear = participation.Year,
            NewTournamentId = newTournament.TournamentId,
            NewYear = Mother.GenerateYear()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenTournamentIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' must not be empty."] }
        });

        var request = new UpdateGolferTournamentParticipationRequest
        {
            OriginalTournamentId = tournament.TournamentId,
            OriginalYear = participation.Year,
            NewTournamentId = default,
            NewYear = Mother.GenerateYear()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenTournamentDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' does not exists in the system"] }
        });

        var request = new UpdateGolferTournamentParticipationRequest
        {
            OriginalTournamentId = tournament.TournamentId,
            OriginalYear = participation.Year,
            NewTournamentId = Mother.GeneratePositiveInteger(),
            NewYear = Mother.GenerateYear()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenYearIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);
        var newTournament = await Mother.CreateTournamentAsync(client);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", ["'Year' must not be empty."] }
        });

        var request = new UpdateGolferTournamentParticipationRequest
        {
            OriginalTournamentId = tournament.TournamentId,
            OriginalYear = participation.Year,
            NewTournamentId = newTournament.TournamentId,
            NewYear = default
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenYearIsLessThan1916_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);
        var newTournament = await Mother.CreateTournamentAsync(client);

        const int lowYear = 1910;
        var currentYear = DateTime.UtcNow.Year;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {lowYear}."] }
        });

        var request = new UpdateGolferTournamentParticipationRequest
        {
            OriginalTournamentId = tournament.TournamentId,
            OriginalYear = participation.Year,
            NewTournamentId = newTournament.TournamentId,
            NewYear = lowYear
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenYearIsGreaterThanCurrentYear_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);
        var newTournament = await Mother.CreateTournamentAsync(client);

        var currentYear = DateTime.UtcNow.Year;
        var higherYear = currentYear + 3;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {higherYear}."] }
        });

        var request = new UpdateGolferTournamentParticipationRequest
        {
            OriginalTournamentId = tournament.TournamentId,
            OriginalYear = participation.Year,
            NewTournamentId = newTournament.TournamentId,
            NewYear = higherYear
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var newTournament = await Mother.CreateTournamentAsync(client);
        var newParticipation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, newTournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentParticipation", ["TournamentParticipation already exists in the system."] }
        });

        var request = new UpdateGolferTournamentParticipationRequest
        {
            OriginalTournamentId = tournament.TournamentId,
            OriginalYear = participation.Year,
            NewTournamentId = newTournament.TournamentId,
            NewYear = newParticipation.Year
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = Mother.GeneratePositiveInteger(), Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateGolferTournamentParticipation_WhenInsufficientPermissions_ShouldReturnForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = Mother.GeneratePositiveInteger(), Year = Mother.GenerateYear()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}