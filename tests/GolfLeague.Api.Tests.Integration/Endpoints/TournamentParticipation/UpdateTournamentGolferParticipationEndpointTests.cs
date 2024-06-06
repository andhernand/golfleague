using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class UpdateTournamentGolferParticipationEndpointTests(GolfApiFactory golfApiFactory)
    : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenDataIsCorrect_ShouldUpdateTournamentParticipation()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var expected = new TournamentParticipationResponse
        {
            TournamentId = tournament.TournamentId,
            GolferId = golfer.GolferId,
            Year = Mother.GenerateYear(),
            Score = Mother.GenerateScore()
        };

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = golfer.GolferId,
            NewYear = expected.Year,
            NewScore = expected.Score
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenGolferIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' must not be empty."] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = default,
            NewYear = Mother.GenerateYear(),
            NewScore = participation.Score
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenGolferDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateTournamentGolferParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' does not exists in the system"] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = Mother.GeneratePositiveInteger(),
            NewYear = participation.Year,
            NewScore = participation.Score
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenTournamentIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateTournamentGolferParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' must not be empty."] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = golfer.GolferId,
            NewYear = Mother.GenerateYear(),
            NewScore = Mother.GenerateScore()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{default(int)}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenTournamentDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateTournamentGolferParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' does not exists in the system"] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = golfer.GolferId,
            NewYear = Mother.GenerateYear(),
            NewScore = Mother.GenerateScore()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenYearIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", ["'Year' must not be empty."] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = golfer.GolferId,
            NewYear = default,
            NewScore = participation.Score
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenYearIsLessThan1916_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        const int lowYear = 1910;
        var currentYear = DateTime.UtcNow.Year;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {lowYear}."] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = golfer.GolferId,
            NewYear = lowYear,
            NewScore = participation.Score
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenYearIsGreaterThanCurrentYear_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var currentYear = DateTime.UtcNow.Year;
        var higherYear = currentYear + 3;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {higherYear}."] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = golfer.GolferId,
            NewYear = higherYear,
            NewScore = participation.Score
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenScoreIsLessThan50_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);
        var newGolfer = await Mother.CreateGolferAsync(client);

        const int lowScore = 44;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Score", [$"'Score' must be between 50 and 130. You entered {lowScore}."] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = newGolfer.GolferId,
            NewYear = participation.Year,
            NewScore = lowScore
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenScoreIsGreaterThan130_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);
        var newGolfer = await Mother.CreateGolferAsync(client);

        const int highScore = 166;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Score", [$"'Score' must be between 50 and 130. You entered {highScore}."] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = newGolfer.GolferId,
            NewYear = participation.Year,
            NewScore = highScore
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);
        var participation = await Mother.CreateGolferTournamentParticipationAsync(
            client, golfer.GolferId, tournament.TournamentId);

        var newGolfer = await Mother.CreateGolferAsync(client);
        var newParticipation = await Mother.CreateGolferTournamentParticipationAsync(
            client, newGolfer.GolferId, tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentParticipation", ["TournamentParticipation already exists in the system."] }
        });

        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = golfer.GolferId,
            OriginalYear = participation.Year,
            NewGolferId = newGolfer.GolferId,
            NewYear = newParticipation.Year,
            NewScore = newParticipation.Score
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golfer = await Mother.CreateGolferAsync(client);
        var tournament = await Mother.CreateTournamentAsync(client);

        var expected = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Title = "Not Found",
            Status = StatusCodes.Status404NotFound
        };

        var request = new UpdateTournamentGolferParticipationRequest()
        {
            OriginalGolferId = Mother.GeneratePositiveInteger(),
            OriginalYear = Mother.GenerateYear(),
            NewGolferId = golfer.GolferId,
            NewYear = Mother.GenerateYear(),
            NewScore = Mother.GenerateScore()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = Mother.GeneratePositiveInteger(),
            OriginalYear = Mother.GenerateYear(),
            NewGolferId = Mother.GeneratePositiveInteger(),
            NewYear = Mother.GenerateYear(),
            NewScore = Mother.GenerateScore()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateTournamentGolferParticipation_WhenInsufficientPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var request = new UpdateTournamentGolferParticipationRequest
        {
            OriginalGolferId = Mother.GeneratePositiveInteger(),
            OriginalYear = Mother.GenerateYear(),
            NewGolferId = Mother.GeneratePositiveInteger(),
            NewYear = Mother.GenerateYear(),
            NewScore = Mother.GenerateScore()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipations",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}