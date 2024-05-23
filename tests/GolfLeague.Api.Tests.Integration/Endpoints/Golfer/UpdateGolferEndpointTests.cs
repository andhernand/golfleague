using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class UpdateGolferEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];

    [Fact]
    public async Task UpdateGolfer_UpdatesGolfer_WhenDataIsCorrect()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        const int changedHandicap = 34;
        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            createdGolfer.FirstName,
            createdGolfer.LastName,
            createdGolfer.Email,
            createdGolfer.JoinDate,
            changedHandicap);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{createdGolfer.GolferId}", updateGolferRequest);

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
        var badId = new Faker().Random.Int(min: 999999);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{badId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateGolfer_Fails_WhenFirstNameIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            "",
            createdGolfer.LastName,
            createdGolfer.Email,
            createdGolfer.JoinDate,
            createdGolfer.Handicap);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{createdGolfer.GolferId}", updateGolferRequest);

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

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            createdGolfer.FirstName,
            "",
            createdGolfer.Email,
            createdGolfer.JoinDate,
            createdGolfer.Handicap);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{createdGolfer.GolferId}", updateGolferRequest);

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

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            createdGolfer.FirstName,
            createdGolfer.LastName,
            "something",
            createdGolfer.JoinDate,
            createdGolfer.Handicap);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{createdGolfer.GolferId}", updateGolferRequest);

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
        var firstGolferResponse = await client.PostAsJsonAsync("/api/golfers", firstGolferRequest);
        var firstGolfer = await firstGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(firstGolfer!.GolferId);

        var secondGolferRequest = Fakers.GenerateCreateGolferRequest();
        var secondGolferResponse = await client.PostAsJsonAsync("/api/golfers", secondGolferRequest);
        var secondGolfer = await secondGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(secondGolfer!.GolferId);

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            secondGolfer.FirstName,
            secondGolfer.LastName,
            firstGolfer.Email,
            secondGolfer.JoinDate,
            secondGolfer.Handicap);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{secondGolfer.GolferId}", updateGolferRequest);

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

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            createdGolfer.FirstName,
            createdGolfer.LastName,
            createdGolfer.Email,
            default(DateOnly),
            createdGolfer.Handicap);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{createdGolfer.GolferId}", updateGolferRequest);

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

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            createdGolfer.FirstName,
            createdGolfer.LastName,
            createdGolfer.Email,
            currentDate.AddYears(2),
            createdGolfer.Handicap);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{createdGolfer.GolferId}", updateGolferRequest);

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

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        const int lessThanLowerBound = -4;
        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            createdGolfer.FirstName,
            createdGolfer.LastName,
            createdGolfer.Email,
            createdGolfer.JoinDate,
            lessThanLowerBound);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{createdGolfer.GolferId}", updateGolferRequest);

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

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        const int greaterThanUpperBound = 55;
        var updateGolferRequest = Fakers.GenerateUpdateGolferRequest(
            createdGolfer.FirstName,
            createdGolfer.LastName,
            createdGolfer.Email,
            createdGolfer.JoinDate,
            greaterThanUpperBound);

        // Act
        var response = await client.PutAsJsonAsync($"/api/golfers/{createdGolfer.GolferId}", updateGolferRequest);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Handicap");
        error.ErrorMessage.Should().Be($"'Handicap' must be between 0 and 54. You entered {greaterThanUpperBound}.");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using HttpClient httpClient = golfApiFactory.CreateClient();
        foreach (int golferId in _createdGolferIds)
        {
            await httpClient.DeleteAsync($"/api/golfers/{golferId}");
        }
    }
}