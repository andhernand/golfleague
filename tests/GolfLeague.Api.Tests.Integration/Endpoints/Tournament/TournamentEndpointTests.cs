using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using GolfLeague.Application.Models;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class TournamentEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>, IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];
    private readonly List<int> _createdTournamentIds = [];
    private readonly List<(int gid, int tid, int year)> _createdParticipationIds = [];

    [Fact]
    public async Task CreateTournament_WhenDataIsCorrect_ShouldCreateTournament()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var request = Mother.GenerateCreateTournamentRequest();

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        _createdTournamentIds.Add(tournament!.TournamentId);

        response.Headers.Location.Should()
            .Be($"http://localhost{Mother.TournamentsApiBasePath}/{tournament.TournamentId}");
        tournament.TournamentId.Should().NotBe(default);
        tournament.Name.Should().Be(request.Name);
        tournament.Format.Should().Be(request.Format);
    }

    [Fact]
    public async Task CreateTournament_WhenNameIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var request = Mother.GenerateCreateTournamentRequest(name: "");

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Name", ["'Name' must not be empty."] }
        });

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournament_WhenFormatIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var request = Mother.GenerateCreateTournamentRequest(format: "");

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Format", ["'Format' must not be empty."] }
        });

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournament_WhenTournamentWithNameAndFormatAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);

        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Tournament", ["A Tournament with the Name and Format combination already exists in the system."] }
        });

        var request = Mother.GenerateCreateTournamentRequest(createdTournament.Name, createdTournament.Format);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournament_WhenTournamentFormatIsNotAnAcceptableValue_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Format", [$"'Format' must be one of any: {string.Join(", ", TournamentFormat.Values)}"] }
        });

        var request = Mother.GenerateCreateTournamentRequest(format: "Yo Momma");

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournament_WhenClientIsUnauthorized_ShouldFailWithUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Mother.GenerateCreateTournamentRequest();

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateGolfer_WhenClientDoesNotHaveProperPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var request = Mother.GenerateCreateTournamentRequest();

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllTournaments_WhenTournamentsExist_ShouldReturnTournaments()
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

        var expected = new[]
        {
            new TournamentResponse
            {
                TournamentId = createdTournament.TournamentId,
                Name = createdTournament.Name,
                Format = createdTournament.Format,
                Participants = new[]
                {
                    new ParticipationDetailResponse
                    {
                        GolferId = createdGolfer.GolferId,
                        FirstName = createdGolfer.FirstName,
                        LastName = createdGolfer.LastName,
                        Year = createdTournamentParticipation.Year,
                        Score = createdTournamentParticipation.Score
                    }
                }
            }
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync(Mother.TournamentsApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournaments = await response.Content.ReadFromJsonAsync<IEnumerable<TournamentResponse>>();
        tournaments.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task
        GetAllTournaments_WhenTournamentsWithNoParticipantsExist_ShouldReturnTournamentsWithNoParticipants()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);

        var expectedTournaments = new[]
        {
            new TournamentResponse
            {
                TournamentId = createdTournament.TournamentId,
                Name = createdTournament.Name,
                Format = createdTournament.Format,
                Participants = []
            }
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync(Mother.TournamentsApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournaments = await response.Content.ReadFromJsonAsync<IEnumerable<TournamentResponse>>();
        tournaments.Should().BeEquivalentTo(expectedTournaments);
    }

    [Fact]
    public async Task GetAllTournaments_WhenNoTournamentsExist_ShouldReturnNoTournaments()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var expectedTournaments = Array.Empty<TournamentResponse>();

        // Act
        var response = await client.GetAsync(Mother.TournamentsApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournaments = await response.Content.ReadFromJsonAsync<IEnumerable<TournamentResponse>>();
        tournaments.Should().BeEquivalentTo(expectedTournaments);
    }

    [Fact]
    public async Task GetAllTournaments_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        // Act
        var response = await client.GetAsync(Mother.TournamentsApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTournamentById_WhenTournamentExists_ShouldReturnTournament()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);

        var expectedTournament = new TournamentResponse
        {
            TournamentId = createdTournament.TournamentId,
            Name = createdTournament.Name,
            Format = createdTournament.Format,
            Participants = []
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync(
            $"{Mother.TournamentsApiBasePath}/{createdTournament.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        tournament.Should().BeEquivalentTo(expectedTournament);
    }

    [Fact]
    public async Task GetTournamentById_WhenTournamentDoesNotExists_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        int tournamentId = Mother.GeneratePositiveInteger(999_999, 9_999_999);

        // Act
        var result = await client.GetAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTournamentById_WhenTournamentWithGolfersExists_ShouldReturnTournamentWithGolfers()
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

        var expectedTournament = new TournamentResponse
        {
            TournamentId = createdTournament.TournamentId,
            Name = createdTournament.Name,
            Format = createdTournament.Format,
            Participants = new[]
            {
                new ParticipationDetailResponse
                {
                    GolferId = createdGolfer.GolferId,
                    FirstName = createdGolfer.FirstName,
                    LastName = createdGolfer.LastName,
                    Year = createdTournamentParticipation.Year,
                    Score = createdTournamentParticipation.Score
                }
            }
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync(
            $"{Mother.TournamentsApiBasePath}/{createdTournament.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        tournament.Should().BeEquivalentTo(expectedTournament);
    }

    [Fact]
    public async Task GetTournamentById_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int tournamentId = Mother.GeneratePositiveInteger(999_999, 9_999_999);

        // Act
        var result = await client.GetAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateTournament_WhenDataIsCorrect_ShouldUpdateTournament()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);

        const string changedFormat = "Match Play";
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest(
            createdTournament.Name,
            changedFormat);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{createdTournament.TournamentId}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        updatedTournament!.TournamentId.Should().Be(createdTournament.TournamentId);
        updatedTournament.Name.Should().Be(createdTournament.Name);
        updatedTournament.Format.Should().Be(changedFormat);
    }

    [Fact]
    public async Task UpdateTournament_WhenTournamentDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var updateRequest = Mother.GenerateUpdateTournamentRequest();
        var badId = Mother.GeneratePositiveInteger(min: 999_999, max: 9_999_999);

        // Act
        var response = await client.PutAsJsonAsync($"{Mother.TournamentsApiBasePath}/{badId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTournament_WhenNameIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest(name: "");
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Name", ["'Name' must not be empty."] }
        });

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournament_WhenFormatIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest(format: "");
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Format", ["'Format' must not be empty."] }
        });

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournament_WhenTournamentWithNameAndFormatAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var firstCreatedTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(firstCreatedTournament.TournamentId);
        var secondCreatedTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(secondCreatedTournament.TournamentId);
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest(
            firstCreatedTournament.Name,
            firstCreatedTournament.Format);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Tournament", ["A Tournament with the Name and Format combination already exists in the system."] }
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{secondCreatedTournament.TournamentId}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournament_WhenTournamentFormatIsNotAnAcceptableValue_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest(format: "Yo Momma");
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Format", [$"'Format' must be one of any: {string.Join(", ", TournamentFormat.Values)}"] }
        });

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournament_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest();

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateTournamentRequest);

        // Act
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateGolfer_WhenDoesNotHaveProperPermissions_ShouldReturnForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest();

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateTournamentRequest);

        // Act
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteTournament_WhenTournamentIsDeleted_ShouldReturnNoContent()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentsApiBasePath}/{createdTournament.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTournament_WhenTournamentDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        int tournamentId = Mother.GeneratePositiveInteger(999_999, 9_999_999);

        // Act
        var response = await client.DeleteAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTournament_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int tournamentId = Mother.GeneratePositiveInteger();

        // Act
        var response = await client.DeleteAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteTournament_WhenClientDoesNotHaveProperPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        int tournamentId = Mother.GeneratePositiveInteger();

        // Act
        var response = await client.DeleteAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

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