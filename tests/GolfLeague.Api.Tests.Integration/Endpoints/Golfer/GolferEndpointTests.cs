using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class GolferEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>, IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];
    private readonly List<int> _createdTournamentIds = [];
    private readonly List<(int gid, int tid, int year)> _createdParticipationIds = [];

    [Fact]
    public async Task CreateGolfer_WhenDataIsCorrect_ShouldCreateGolfer()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true, isAdmin: false);
        var request = Mother.GenerateCreateGolferRequest();

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(golfer!.GolferId);

        response.Headers.Location.Should().Be($"http://localhost{Mother.GolfersApiBasePath}/{golfer.GolferId}");
        golfer.GolferId.Should().NotBe(default);
        golfer.FirstName.Should().Be(request.FirstName);
        golfer.LastName.Should().Be(request.LastName);
        golfer.Email.Should().Be(request.Email);
        golfer.JoinDate.Should().Be(request.JoinDate);
        golfer.Handicap.Should().Be(request.Handicap);
    }

    [Fact]
    public async Task CreateGolfer_WhenFirstNameIsInvalid_ShouldFailWithBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true, isAdmin: false);
        var request = Mother.GenerateCreateGolferRequest(firstName: "");
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "FirstName", ["'First Name' must not be empty."] }
        });

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolfer_WhenLastNameIsInvalid_ShouldFailWithBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true, isAdmin: false);
        var request = Mother.GenerateCreateGolferRequest(lastName: "");
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "LastName", ["'Last Name' must not be empty."] }
        });

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolfer_WhenEmailIsInvalid_ShouldFailWithBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true, isAdmin: false);
        var request = Mother.GenerateCreateGolferRequest(email: "badEmail");
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Email", ["'Email' is not in the correct format."] }
        });

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolfer_WhenEmailAlreadyExists_ShouldFailWithBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: false, isAdmin: true);
        var existingGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(existingGolfer.GolferId);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Email", ["This Email already exists in the system."] }
        });

        var request = Mother.GenerateCreateGolferRequest(email: existingGolfer.Email);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true, isAdmin: false));

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolfer_WhenJoinDateIsInvalid_ShouldFailWithBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true, isAdmin: false);
        var request = Mother.GenerateCreateGolferRequest(joinDate: default(DateOnly));
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "JoinDate", ["'Join Date' must not be empty."] }
        });

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolfer_WhenJoinDateIsInTheFuture_ShouldFailWithBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true, isAdmin: false);
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = Mother.GenerateCreateGolferRequest(joinDate: currentDate.AddYears(1));
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "JoinDate", [$"'Join Date' must be less than or equal to '{currentDate.Year}'."] }
        });

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolfer_WhenHandicapIsLessThanZero_ShouldFailWithBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true, isAdmin: false);
        const int lessThanLowerBound = -2;
        var request = Mother.GenerateCreateGolferRequest(handicap: lessThanLowerBound);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Handicap", [$"'Handicap' must be between 0 and 54. You entered {lessThanLowerBound}."] }
        });

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolfer_WhenHandicapIsGreaterThanFiftyFour_ShouldFailWithBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true, isAdmin: false);
        const int greaterThanUpperBound = 67;
        var request = Mother.GenerateCreateGolferRequest(handicap: greaterThanUpperBound);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Handicap", [$"'Handicap' must be between 0 and 54. You entered {greaterThanUpperBound}."] }
        });

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateGolfer_WhenClientIsUnauthorized_ShouldFailWithUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Mother.GenerateCreateGolferRequest();

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateGolfer_WhenClientDoesNotHaveProperPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: false, isAdmin: false);
        var request = Mother.GenerateCreateGolferRequest();

        // Act
        var response = await client.PostAsJsonAsync(Mother.GolfersApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllGolfers_WhenGolfersExist_ShouldReturnGolfers()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(createdGolfer.GolferId);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);
        var createdTournamentParticipation = await Mother.CreateGolferTournamentParticipationAsync(
            client,
            createdGolfer.GolferId,
            createdTournament.TournamentId);
        _createdParticipationIds.Add((createdTournamentParticipation.GolferId,
            createdTournamentParticipation.TournamentId, createdTournamentParticipation.Year));

        var expected = new GolfersResponse
        {
            Golfers = new[]
            {
                new GolferResponse
                {
                    GolferId = createdGolfer.GolferId,
                    FirstName = createdGolfer.FirstName,
                    LastName = createdGolfer.LastName,
                    Email = createdGolfer.Email,
                    Handicap = createdGolfer.Handicap,
                    JoinDate = createdGolfer.JoinDate,
                    Tournaments = new[]
                    {
                        new TournamentDetailResponse
                        {
                            TournamentId = createdTournament.TournamentId,
                            Name = createdTournament.Name,
                            Format = createdTournament.Format,
                            Year = createdTournamentParticipation.Year,
                            Score = createdTournamentParticipation.Score
                        }
                    }
                }
            }
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync(Mother.GolfersApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var golfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();
        golfers.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllGolfers_WhenGolfersWithNoTournamentsExist_ShouldReturnGolfersWithoutTournaments()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(createdGolfer.GolferId);

        var expected = new GolfersResponse
        {
            Golfers = new[]
            {
                new GolferResponse
                {
                    GolferId = createdGolfer.GolferId,
                    FirstName = createdGolfer.FirstName,
                    LastName = createdGolfer.LastName,
                    Email = createdGolfer.Email,
                    Handicap = createdGolfer.Handicap,
                    JoinDate = createdGolfer.JoinDate,
                    Tournaments = Array.Empty<TournamentDetailResponse>()
                }
            }
        };

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync(Mother.GolfersApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var golfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();
        golfers.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllGolfers_WhenNoGolfersExist_ShouldReturnNoGolfers()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);

        // Act
        var response = await client.GetAsync(Mother.GolfersApiBasePath);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var returnedGolfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();
        returnedGolfers!.Golfers.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllGolfers_WhenClientIsUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        // Act
        var response = await client.GetAsync(Mother.GolfersApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetGolferById_WhenGolferExists_ShouldReturnGolfer()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(createdGolfer.GolferId);

        var expected = new GolferResponse
        {
            GolferId = createdGolfer.GolferId,
            FirstName = createdGolfer.FirstName,
            LastName = createdGolfer.LastName,
            Email = createdGolfer.Email,
            Handicap = createdGolfer.Handicap,
            JoinDate = createdGolfer.JoinDate,
            Tournaments = Array.Empty<TournamentDetailResponse>()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync($"{Mother.GolfersApiBasePath}/{createdGolfer.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        golfer.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetGolferById_WhenGolferDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var id = Mother.GeneratePositiveInteger();

        // Act
        var result = await client.GetAsync($"{Mother.GolfersApiBasePath}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGolferById_WhenGolferWithTournamentsExists_ShouldReturnGolferWithTournaments()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var createdGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(createdGolfer.GolferId);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        _createdTournamentIds.Add(createdTournament.TournamentId);
        var createdTournamentParticipation = await Mother.CreateGolferTournamentParticipationAsync(
            client,
            createdGolfer.GolferId,
            createdTournament.TournamentId);
        _createdParticipationIds.Add((
            createdTournamentParticipation.GolferId,
            createdTournamentParticipation.TournamentId,
            createdTournamentParticipation.Year));

        var expectedGolfer = new GolferResponse
        {
            GolferId = createdGolfer.GolferId,
            FirstName = createdGolfer.FirstName,
            LastName = createdGolfer.LastName,
            Email = createdGolfer.Email,
            Handicap = createdGolfer.Handicap,
            JoinDate = createdGolfer.JoinDate,
            Tournaments = new[]
            {
                new TournamentDetailResponse
                {
                    TournamentId = createdTournament.TournamentId,
                    Name = createdTournament.Name,
                    Format = createdTournament.Format,
                    Year = createdTournamentParticipation.Year,
                    Score = createdTournamentParticipation.Score
                }
            }
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync($"{Mother.GolfersApiBasePath}/{createdGolfer.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        golfer.Should().BeEquivalentTo(expectedGolfer);
    }

    [Fact]
    public async Task GetGolferById_WhenUnAuthenticated_ShouldReturnUnAuthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var id = Mother.GeneratePositiveInteger();

        // Act
        var result = await client.GetAsync($"{Mother.GolfersApiBasePath}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HealthCheck_WhenHealthy_ShouldReturnHealthy()
    {
        using var client = golfApiFactory.CreateClient();

        var response = await client.GetAsync("/_health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var message = await response.Content.ReadAsStringAsync();
        message.Should().Be("Healthy");
    }

    [Fact]
    public async Task UpdateGolfer_WhenDataIsCorrect_ShouldUpdateGolfer()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(createdGolfer.GolferId);

        const int changedHandicap = 34;
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(
            createdGolfer.FirstName,
            createdGolfer.LastName,
            createdGolfer.Email,
            createdGolfer.JoinDate,
            changedHandicap);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{createdGolfer.GolferId}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedGolfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        updatedGolfer!.GolferId.Should().Be(createdGolfer.GolferId);
        updatedGolfer.FirstName.Should().Be(createdGolfer.FirstName);
        updatedGolfer.LastName.Should().Be(createdGolfer.LastName);
        updatedGolfer.Email.Should().Be(createdGolfer.Email);
        updatedGolfer.JoinDate.Should().Be(createdGolfer.JoinDate);
        updatedGolfer.Handicap.Should().Be(changedHandicap);
    }

    [Fact]
    public async Task UpdateGolfer_WhenGolferDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var updateRequest = Mother.GenerateUpdateGolferRequest();
        var badId = Mother.GeneratePositiveInteger(999_999);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{badId}",
            updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateGolfer_WhenFirstNameIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "FirstName", ["'First Name' must not be empty."] }
        });

        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(firstName: "");

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenLastNameIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "LastName", ["'Last Name' must not be empty."] }
        });

        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(lastName: "");

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenEmailIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Email", ["'Email' is not in the correct format."] }
        });

        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(email: "something");

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenEmailAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var firstGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(firstGolfer.GolferId);
        var secondGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(secondGolfer.GolferId);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Email", ["This Email already exists in the system."] }
        });

        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(
            secondGolfer.FirstName,
            secondGolfer.LastName,
            firstGolfer.Email,
            secondGolfer.JoinDate,
            secondGolfer.Handicap);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{secondGolfer.GolferId}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenJoinDateIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "JoinDate", ["'Join Date' must not be empty."] }
        });
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(joinDate: default(DateOnly));

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenJoinDateIsInTheFuture_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "JoinDate", [$"'Join Date' must be less than or equal to '{currentDate.Year}'."] }
        });

        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(joinDate: currentDate.AddYears(2));

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenHandicapIsLessThanZero_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        const int lessThanLowerBound = -4;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Handicap", [$"'Handicap' must be between 0 and 54. You entered {lessThanLowerBound}."] }
        });

        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(handicap: lessThanLowerBound);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenHandicapIsGreaterThanFiftyFour_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        const int greaterThanUpperBound = 55;
        var expected = Mother.CreateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Handicap", [$"'Handicap' must be between 0 and 54. You entered {greaterThanUpperBound}."] }
        });

        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(handicap: greaterThanUpperBound);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest();

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Act
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateGolfer_WhenDoesNotHaveProperPermissions_ShouldReturnForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest();

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Act
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteGolfer_WhenGolferIsDeleted_ShouldReturnNoContent()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true, isTrusted: false);
        var createdGolfer = await Mother.CreateGolferAsync(client);
        _createdGolferIds.Add(createdGolfer.GolferId);

        // Act
        var response = await client.DeleteAsync($"{Mother.GolfersApiBasePath}/{createdGolfer.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteGolfer_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int golferId = Mother.GeneratePositiveInteger();

        // Act
        var response = await client.DeleteAsync($"{Mother.GolfersApiBasePath}/{golferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound, true, false)]
    [InlineData(HttpStatusCode.Forbidden, false, false)]
    [InlineData(HttpStatusCode.Forbidden, false, true)]
    public async Task DeleteGolfer_WhenGolferDoesNotExistOrUnauthorized_ShouldReturnExpectedStatusCode(
        HttpStatusCode expectedStatusCode,
        bool isAdmin,
        bool isTrusted)
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: isTrusted, isAdmin: isAdmin);
        int golferId = Mother.GeneratePositiveInteger();

        // Act
        var response = await client.DeleteAsync($"{Mother.GolfersApiBasePath}/{golferId}");

        // Assert
        response.StatusCode.Should().Be(expectedStatusCode);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        foreach (var createdParticipationId in _createdParticipationIds)
        {
            _ = await client.DeleteAsync(
                $"{Mother.TournamentParticipationsApiBasePath}"
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