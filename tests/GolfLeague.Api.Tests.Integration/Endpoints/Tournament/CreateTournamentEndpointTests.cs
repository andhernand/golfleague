using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Application.Models;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class CreateTournamentEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
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
        response.Headers.Location.Should()
            .Be($"http://localhost{Mother.TournamentsApiBasePath}/{tournament!.TournamentId}");
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

        var expected = new ValidationFailureResponse
        {
            Errors = new[]
            {
                new ValidationResponse { PropertyName = "Name", ErrorMessage = "'Name' must not be empty." }
            }
        };

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournament_WhenFormatIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var request = Mother.GenerateCreateTournamentRequest(format: "");

        var expected = new ValidationFailureResponse
        {
            Errors = new[]
            {
                new ValidationResponse { PropertyName = "Format", ErrorMessage = "'Format' must not be empty." }
            }
        };

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournament_WhenTournamentWithNameAndFormatAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);

        var expected = new ValidationFailureResponse
        {
            Errors = new[]
            {
                new ValidationResponse
                {
                    PropertyName = "Tournament",
                    ErrorMessage =
                        "A Tournament with the Name and Format combination already exists in the system."
                }
            }
        };

        var request = Mother.GenerateCreateTournamentRequest(createdTournament!.Name, createdTournament.Format);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateTournament_WhenTournamentFormatIsNotAnAcceptableValue_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var expected = new ValidationFailureResponse
        {
            Errors = new[]
            {
                new ValidationResponse
                {
                    PropertyName = "Format",
                    ErrorMessage =
                        $"'Format' must be one of any: {string.Join(", ", TournamentFormat.Values)}"
                }
            }
        };

        var request = Mother.GenerateCreateTournamentRequest(format: "Yo Momma");

        // Act
        var response = await client.PostAsJsonAsync(Mother.TournamentsApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
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
}