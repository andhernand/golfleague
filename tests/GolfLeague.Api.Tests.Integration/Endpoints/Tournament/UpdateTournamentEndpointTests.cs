using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Application.Models;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class UpdateTournamentEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task UpdateTournament_WhenDataIsCorrect_ShouldUpdateTournament()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);

        const string changedFormat = "Match Play";
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest(
            createdTournament!.Name,
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
        var expected = new ValidationFailureResponse
        {
            Errors = new[]
            {
                new ValidationResponse { PropertyName = "Name", ErrorMessage = "'Name' must not be empty." }
            }
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournament_WhenFormatIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest(format: "");
        var expected = new ValidationFailureResponse
        {
            Errors = new[]
            {
                new ValidationResponse { PropertyName = "Format", ErrorMessage = "'Format' must not be empty." }
            }
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournament_WhenTournamentWithNameAndFormatAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var firstCreatedTournament = await Mother.CreateTournamentAsync(client);
        var secondCreatedTournament = await Mother.CreateTournamentAsync(client);
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest(
            firstCreatedTournament!.Name,
            firstCreatedTournament.Format);
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

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(isTrusted: true));

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{secondCreatedTournament!.TournamentId}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTournament_WhenTournamentFormatIsNotAnAcceptableValue_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var updateTournamentRequest = Mother.GenerateUpdateTournamentRequest(format: "Yo Momma");
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

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.TournamentsApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateTournamentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
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
}