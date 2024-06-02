using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class CreateGolferEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
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
        response.Headers.Location.Should()
            .Be($"http://localhost{Mother.GolfersApiBasePath}/{golfer!.GolferId}");
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
}