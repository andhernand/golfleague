using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class CreateGolferEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];

    [Fact]
    public async Task CreateGolfer_CreatesGolfer_WhenDataIsCorrect()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Fakers.GenerateCreateGolferRequest();

        // Act
        var result = await client.PostAsJsonAsync("/api/golfers", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var golfer = await result.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(golfer!.GolferId);

        result.Headers.Location.Should().Be($"http://localhost/api/golfers/{golfer.GolferId}");
        golfer.GolferId.Should().NotBe(default);
        golfer.FirstName.Should().Be(request.FirstName);
        golfer.LastName.Should().Be(request.LastName);
        golfer.Email.Should().Be(request.Email);
        golfer.JoinDate.Should().Be(request.JoinDate);
        golfer.Handicap.Should().Be(request.Handicap);
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_FirstNameIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Fakers.GenerateCreateGolferRequest(firstName: "");

        // Act
        var response = await client.PostAsJsonAsync("/api/golfers", request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("FirstName");
        error.ErrorMessage.Should().Be("'First Name' must not be empty.");
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_LastNameIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Fakers.GenerateCreateGolferRequest(lastName: "");

        // Act
        var response = await client.PostAsJsonAsync("/api/golfers", request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("LastName");
        error.ErrorMessage.Should().Be("'Last Name' must not be empty.");
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_EmailIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Fakers.GenerateCreateGolferRequest(email: "badEmail");

        // Act
        var response = await client.PostAsJsonAsync("/api/golfers", request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Email");
        error.ErrorMessage.Should().Be("'Email' is not in the correct format.");
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_EmailAlreadyExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var existingGolferRequest = Fakers.GenerateCreateGolferRequest();
        var existingGolferResponse = await client.PostAsJsonAsync("/api/golfers", existingGolferRequest);
        var existingGolfer = await existingGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(existingGolfer!.GolferId);

        var request = Fakers.GenerateCreateGolferRequest(email: existingGolfer.Email);

        // Act
        var response = await client.PostAsJsonAsync("/api/golfers", request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        error.PropertyName.Should().Be("Email");
        error.ErrorMessage.Should().Be("This Email already exists in the system.");
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_JoinDateIsInvalid()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var defaultDate = default(DateOnly);
        var request = Fakers.GenerateCreateGolferRequest(joinDate: defaultDate);

        // Act
        var response = await client.PostAsJsonAsync("/api/golfers", request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("JoinDate");
        error.ErrorMessage.Should().Be("'Join Date' must not be empty.");
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_JoinDateIsInTheFuture()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = Fakers.GenerateCreateGolferRequest(joinDate: currentDate.AddYears(1));

        // Act
        var response = await client.PostAsJsonAsync("/api/golfers", request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("JoinDate");
        error.ErrorMessage.Should().Be($"'Join Date' must be less than or equal to '{currentDate.Year}'.");
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_HandicapIsLessThanZero()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        const int lessThanLowerBound = -2;
        var request = Fakers.GenerateCreateGolferRequest(handicap: lessThanLowerBound);

        // Act
        var response = await client.PostAsJsonAsync("/api/golfers", request);

        // Assert
        var errors = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Handicap");
        error.ErrorMessage.Should().Be($"'Handicap' must be between 0 and 54. You entered {lessThanLowerBound}.");
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_HandicapIsGreaterThanFiftyFour()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        const int greaterThanUpperBound = 67;
        var request = Fakers.GenerateCreateGolferRequest(handicap: greaterThanUpperBound);

        // Act
        var response = await client.PostAsJsonAsync("/api/golfers", request);

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
        using var httpClient = golfApiFactory.CreateClient();
        foreach (var golferId in _createdGolferIds)
        {
            await httpClient.DeleteAsync($"/api/golfers/{golferId}");
        }
    }
}