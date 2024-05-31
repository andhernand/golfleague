using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class UpdateGolferEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task UpdateGolfer_WhenDataIsCorrect_ShouldUpdateGolfer()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);

        const int changedHandicap = 34;
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(
            createdGolfer!.FirstName,
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
        var expected = Mother.CreateValidationFailureResponse("FirstName", "'First Name' must not be empty.");
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(firstName: "");

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenLastNameIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var expected = Mother.CreateValidationFailureResponse("LastName", "'Last Name' must not be empty.");
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(lastName: "");

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenEmailIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var expected = Mother.CreateValidationFailureResponse("Email", "'Email' is not in the correct format.");
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(email: "something");

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenEmailAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var firstGolfer = await Mother.CreateGolferAsync(client);
        var secondGolfer = await Mother.CreateGolferAsync(client);
        var expected = Mother.CreateValidationFailureResponse("Email", "This Email already exists in the system.");

        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(
            secondGolfer!.FirstName,
            secondGolfer.LastName,
            firstGolfer!.Email,
            secondGolfer.JoinDate,
            secondGolfer.Handicap);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{secondGolfer.GolferId}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenJoinDateIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var expected = Mother.CreateValidationFailureResponse("JoinDate", "'Join Date' must not be empty.");
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(joinDate: default(DateOnly));

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenJoinDateIsInTheFuture_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var expected = Mother.CreateValidationFailureResponse(
            "JoinDate", $"'Join Date' must be less than or equal to '{currentDate.Year}'.");
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(joinDate: currentDate.AddYears(2));

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenHandicapIsLessThanZero_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        const int lessThanLowerBound = -4;
        var expected = Mother.CreateValidationFailureResponse(
            "Handicap", $"'Handicap' must be between 0 and 54. You entered {lessThanLowerBound}.");
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(handicap: lessThanLowerBound);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateGolfer_WhenHandicapIsGreaterThanFiftyFour_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        const int greaterThanUpperBound = 55;
        var expected = Mother.CreateValidationFailureResponse(
            "Handicap", $"'Handicap' must be between 0 and 54. You entered {greaterThanUpperBound}.");
        var updateGolferRequest = Mother.GenerateUpdateGolferRequest(handicap: greaterThanUpperBound);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.GolfersApiBasePath}/{Mother.GeneratePositiveInteger()}",
            updateGolferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
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
}