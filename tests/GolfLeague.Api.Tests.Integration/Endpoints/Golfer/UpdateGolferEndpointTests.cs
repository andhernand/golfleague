using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class UpdateGolferEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    private const string GolfersApiBasePath = "/api/golfers";

    [Fact]
    public async Task UpdateGolfer_UpdatesGolfer_WhenDataIsCorrect()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync(GolfersApiBasePath, createGolferRequest);
        var createdGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        const int changedHandicap = 34;
        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            createdGolfer!.FirstName,
            createdGolfer.LastName,
            createdGolfer.Email,
            createdGolfer.JoinDate,
            changedHandicap);

        // Act
        var response = await client
            .PutAsJsonAsync($"{GolfersApiBasePath}/{createdGolfer.GolferId}", updateGolferRequest);

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
    public async Task UpdateGolfer_ReturnsNotFound_WhenGolferDoesNotExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var updateRequest = Fakers.GenerateUpdateGolferRequest();
        var badId = Fakers.GeneratePositiveInteger(999_999);

        // Act
        var response = await client.PutAsJsonAsync($"{GolfersApiBasePath}/{badId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateGolfer_Fails_WhenFirstNameIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(firstName: "");

        // Act
        var response = await client
            .PutAsJsonAsync($"{GolfersApiBasePath}/{Fakers.GeneratePositiveInteger()}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("FirstName");
        error.ErrorMessage.Should().Be("'First Name' must not be empty.");
    }

    [Fact]
    public async Task UpdateGolfer_Fails_WhenLastNameIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(lastName: "");

        // Act
        var response = await client
            .PutAsJsonAsync($"{GolfersApiBasePath}/{Fakers.GeneratePositiveInteger()}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("LastName");
        error.ErrorMessage.Should().Be("'Last Name' must not be empty.");
    }

    [Fact]
    public async Task UpdateGolfer_Fails_WhenEmailIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(email: "something");

        // Act
        var response = await client
            .PutAsJsonAsync($"{GolfersApiBasePath}/{Fakers.GeneratePositiveInteger()}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Email");
        error.ErrorMessage.Should().Be("'Email' is not in the correct format.");
    }

    [Fact]
    public async Task UpdateGolfer_Fails_WhenEmailAlreadyExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var firstGolferRequest = Fakers.GenerateCreateGolferRequest();
        var firstGolferResponse = await client.PostAsJsonAsync(GolfersApiBasePath, firstGolferRequest);
        var firstGolfer = await firstGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        var secondGolferRequest = Fakers.GenerateCreateGolferRequest();
        var secondGolferResponse = await client.PostAsJsonAsync(GolfersApiBasePath, secondGolferRequest);
        var secondGolfer = await secondGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            secondGolfer!.FirstName,
            secondGolfer.LastName,
            firstGolfer!.Email,
            secondGolfer.JoinDate,
            secondGolfer.Handicap);

        // Act
        var response = await client
            .PutAsJsonAsync($"{GolfersApiBasePath}/{secondGolfer.GolferId}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        error.PropertyName.Should().Be("Email");
        error.ErrorMessage.Should().Be("This Email already exists in the system.");
    }

    [Fact]
    public async Task UpdateGolfer_Fails_WhenJoinDateIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(joinDate: default(DateOnly));

        // Act
        var response = await client
            .PutAsJsonAsync($"{GolfersApiBasePath}/{Fakers.GeneratePositiveInteger()}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("JoinDate");
        error.ErrorMessage.Should().Be("'Join Date' must not be empty.");
    }

    [Fact]
    public async Task UpdateGolfer_Fails_WhenJoinDateIsInTheFuture()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(joinDate: currentDate.AddYears(2));

        // Act
        var response = await client
            .PutAsJsonAsync($"{GolfersApiBasePath}/{Fakers.GeneratePositiveInteger()}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("JoinDate");
        error.ErrorMessage.Should().Be($"'Join Date' must be less than or equal to '{currentDate.Year}'.");
    }

    [Fact]
    public async Task UpdateGolfer_Fails_WhenHandicapIsLessThanZero()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        const int lessThanLowerBound = -4;
        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(handicap: lessThanLowerBound);

        // Act
        var response = await client
            .PutAsJsonAsync($"{GolfersApiBasePath}/{Fakers.GeneratePositiveInteger()}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Handicap");
        error.ErrorMessage.Should().Be($"'Handicap' must be between 0 and 54. You entered {lessThanLowerBound}.");
    }

    [Fact]
    public async Task UpdateGolfer_Fails_WhenHandicapIsGreaterThanFiftyFour()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        const int greaterThanUpperBound = 55;
        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(handicap: greaterThanUpperBound);

        // Act
        var response = await client
            .PutAsJsonAsync($"{GolfersApiBasePath}/{Fakers.GeneratePositiveInteger()}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Handicap");
        error.ErrorMessage.Should().Be($"'Handicap' must be between 0 and 54. You entered {greaterThanUpperBound}.");
    }
}