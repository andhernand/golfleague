using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class TournamentParticipationEndpointTests(GolfApiFactory golfApiFactory)
    : IClassFixture<GolfApiFactory>, IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];
    private readonly List<int> _createdTournamentIds = [];
    private readonly List<(int gid, int tid, int year)> _createdParticipationIds = [];

    [Fact]
    public async Task CreateParticipationDetail_WhenDataIsCorrect_ShouldCreateTournamentParticipation()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        var expectedYear = Mother.GenerateYear();
        var expectedScore = Mother.GenerateScore();
        var request = new CreateParticipationDetailRequest
        {
            TournamentId = tournament.TournamentId, Year = expectedYear, Score = expectedScore
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();

        _createdParticipationIds.Add((
            actual!.GolferId,
            actual.TournamentId,
            actual.Year));

        response.Headers.Location.Should().Be($"http://localhost{Mother.GolfersApiBasePath}/{actual.GolferId}");

        actual.GolferId.Should().Be(golfer.GolferId);
        actual.TournamentId.Should().Be(tournament.TournamentId);
        actual.Year.Should().Be(expectedYear);
        actual.Score.Should().Be(expectedScore);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenGolferIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' must not be empty."] }
        });

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = createdTournament.TournamentId,
            Year = Mother.GenerateYear(),
            Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{default(int)}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenGolferIdDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' does not exists in the system"] }
        });

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = createdTournament.TournamentId,
            Year = Mother.GenerateYear(),
            Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenTournamentIdInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' must not be empty."] }
        });

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = default, Year = Mother.GenerateYear(), Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenTournamentIdDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' does not exists in the system"] }
        });

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = Mother.GeneratePositiveInteger(),
            Year = Mother.GenerateYear(),
            Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenYearIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", ["'Year' must not be empty."] }
        });

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = tournament.TournamentId, Year = default, Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenYearIsLessThan1916_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        const int lowYear = 1910;
        var currentYear = DateTime.UtcNow.Year;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {lowYear}."] }
        });

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = tournament.TournamentId, Year = lowYear, Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenYearIsGreaterThanCurrentYear_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        var currentYear = DateTime.UtcNow.Year;
        var higherYear = currentYear + 3;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {higherYear}."] }
        });

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = tournament.TournamentId, Year = higherYear, Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenScoreIsBelow50_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        const int lowScore = 49;

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Score", [$"'Score' must be between 50 and 130. You entered {lowScore}."] }
        });

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = tournament.TournamentId, Year = Mother.GenerateYear(), Score = lowScore
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenScoreIsAbove130_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        const int highScore = 131;

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Score", [$"'Score' must be between 50 and 130. You entered {highScore}."] }
        });

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = tournament.TournamentId, Year = Mother.GenerateYear(), Score = highScore
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        var participation = await Mother.CreateParticipationDetailAsync(
            client,
            golfer.GolferId,
            tournament.TournamentId);
        _createdParticipationIds.Add((participation.GolferId, participation.TournamentId, participation.Year));

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentParticipation", ["TournamentParticipation already exists in the system."] }
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = participation.TournamentId, Year = participation.Year, Score = participation.Score
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{golfer.GolferId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenClientIsUnauthorized_ShouldFailWithUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var request = new CreateParticipationDetailRequest
        {
            TournamentId = Mother.GeneratePositiveInteger(),
            Year = Mother.GenerateYear(),
            Score = Mother.GenerateScore()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateParticipationDetail_WhenInsufficientPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var request = new CreateParticipationDetailRequest
        {
            TournamentId = Mother.GeneratePositiveInteger(),
            Year = Mother.GenerateYear(),
            Score = Mother.GenerateScore()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenDataIsCorrect_ShouldCreateTournamentParticipation()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        var expectedYear = Mother.GenerateYear();
        var expectedScore = Mother.GenerateScore();
        var request = new CreateTournamentDetailRequest
        {
            GolferId = golfer.GolferId, Year = expectedYear, Score = expectedScore
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();
        _createdParticipationIds.Add((actual!.GolferId, actual.TournamentId, actual.Year));

        response.Headers.Location.Should().Be($"http://localhost{Mother.TournamentsApiBasePath}/{actual.TournamentId}");

        actual.GolferId.Should().Be(golfer.GolferId);
        actual.TournamentId.Should().Be(tournament.TournamentId);
        actual.Year.Should().Be(expectedYear);
        actual.Score.Should().Be(expectedScore);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenGolferIdIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' must not be empty."] }
        });

        var request = new CreateTournamentDetailRequest
        {
            GolferId = default, Year = Mother.GenerateYear(), Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{createdTournament.TournamentId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenGolferIdDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "GolferId", ["'Golfer Id' does not exists in the system"] }
        });

        var request = new CreateTournamentDetailRequest
        {
            GolferId = Mother.GeneratePositiveInteger(),
            Year = Mother.GenerateYear(),
            Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{createdTournament.TournamentId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenTournamentIdInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' must not be empty."] }
        });

        var request = new CreateTournamentDetailRequest
        {
            GolferId = golfer.GolferId, Year = Mother.GenerateYear(), Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{default(int)}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenTournamentIdDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentId", ["'Tournament Id' does not exists in the system"] }
        });

        var request = new CreateTournamentDetailRequest
        {
            GolferId = golfer.GolferId, Year = Mother.GenerateYear(), Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenYearIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);
        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", ["'Year' must not be empty."] }
        });

        var request = new CreateTournamentDetailRequest
        {
            GolferId = golfer.GolferId, Year = default, Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenYearIsLessThan1916_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);
        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        const int lowYear = 1910;
        var currentYear = DateTime.UtcNow.Year;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {lowYear}."] }
        });

        var request = new CreateTournamentDetailRequest
        {
            GolferId = golfer.GolferId, Year = lowYear, Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenYearIsGreaterThanCurrentYear_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);
        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        var currentYear = DateTime.UtcNow.Year;
        var higherYear = currentYear + 3;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Year", [$"'Year' must be between 1916 and {currentYear}. You entered {higherYear}."] }
        });

        var request = new CreateTournamentDetailRequest
        {
            GolferId = golfer.GolferId, Year = higherYear, Score = Mother.GenerateScore()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenScoreIsLessThan50_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);
        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        const int lowScore = 44;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Score", [$"'Score' must be between 50 and 130. You entered {lowScore}."] }
        });

        var request = new CreateTournamentDetailRequest
        {
            GolferId = golfer.GolferId, Year = Mother.GenerateYear(), Score = lowScore
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenScoreIsGreaterThan130_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);
        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);

        const int highScore = 156;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Score", [$"'Score' must be between 50 and 130. You entered {highScore}."] }
        });

        var request = new CreateTournamentDetailRequest
        {
            GolferId = golfer.GolferId, Year = Mother.GenerateYear(), Score = highScore
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var golfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(golfer.GolferId);
        var tournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(tournament.TournamentId);
        var participation = await Mother.CreateTournamentDetailAsync(
            client,
            golfer.GolferId,
            tournament.TournamentId);
        _createdParticipationIds.Add((participation.GolferId, participation.TournamentId, participation.Year));

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "TournamentParticipation", ["TournamentParticipation already exists in the system."] }
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        var request = new CreateTournamentDetailRequest
        {
            GolferId = participation.GolferId, Year = participation.Year, Score = participation.Score
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{tournament.TournamentId}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenClientIsUnauthorized_ShouldFailWithUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var request = new CreateTournamentDetailRequest
        {
            GolferId = Mother.GeneratePositiveInteger(),
            Year = Mother.GenerateYear(),
            Score = Mother.GenerateScore()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTournamentDetail_WhenInsufficientPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var request = new CreateTournamentDetailRequest
        {
            GolferId = Mother.GeneratePositiveInteger(),
            Year = Mother.GenerateYear(),
            Score = Mother.GenerateScore()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}/tournamentparticipation",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteTournamentParticipation_WhenTournamentParticipationIsDeleted_ShouldReturnNoContent()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var createdGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(createdGolfer.GolferId);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);
        var createdTournamentParticipation = await Mother.CreateTournamentDetailAsync(
            client,
            createdGolfer.GolferId,
            createdTournament.TournamentId);
        _createdParticipationIds.Add((
            createdTournamentParticipation.GolferId,
            createdTournamentParticipation.TournamentId,
            createdTournamentParticipation.Year));

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentParticipationApiBasePath}"
            + $"?golferId={createdTournamentParticipation.GolferId}"
            + $"&tournamentId={createdTournamentParticipation.TournamentId}"
            + $"&year={createdTournamentParticipation.Year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTournamentParticipation_WhenTournamentParticipationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var golferId = Mother.GeneratePositiveInteger();
        var tournamentId = Mother.GeneratePositiveInteger();
        var year = Mother.GenerateYear();

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentParticipationApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTournamentParticipation_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var golferId = Mother.GeneratePositiveInteger();
        var tournamentId = Mother.GeneratePositiveInteger();
        var year = Mother.GenerateYear();

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentParticipationApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteTournamentParticipation_WhenClientDoesNotHaveProperPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golferId = Mother.GeneratePositiveInteger();
        var tournamentId = Mother.GeneratePositiveInteger();
        var year = Mother.GenerateYear();

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentParticipationApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        foreach (var createdParticipationId in _createdParticipationIds)
        {
            _ = await client.DeleteAsync(
                $"{Mother.TournamentParticipationApiBasePath}"
                + $"?golferId={createdParticipationId.gid}"
                + $"&tournamentId={createdParticipationId.tid}"
                + $"&year={createdParticipationId.year}");
        }

        foreach (var golferId in _createdGolferIds)
        {
            _ = await client.DeleteAsync($"{Mother.GolfersApiBasePath}/{golferId}");
        }

        foreach (var tournamentId in _createdTournamentIds)
        {
            _ = await client.DeleteAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");
        }
    }
}