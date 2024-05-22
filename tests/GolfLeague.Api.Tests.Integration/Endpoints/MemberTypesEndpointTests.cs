using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints;

public class MemberTypesEndpointTests(GolfApiFactory factory) : IClassFixture<GolfApiFactory>, IAsyncLifetime
{
    private readonly List<int> _createdMemberTypeIds = [];

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task CreateMemberType_CreatesMemberType_WhenDataIsCorrect()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = Fakers.GenerateCreateMemberTypeRequest();

        // Act
        var result = await client.PostAsJsonAsync("/api/membertypes", request);
        var createdMemberType = await result.Content.ReadFromJsonAsync<MemberTypeResponse>();
        _createdMemberTypeIds.Add(createdMemberType!.MemberTypeId);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Headers.Location.Should().Be($"http://localhost/api/membertypes/{createdMemberType.MemberTypeId}");
        createdMemberType.Name.Should().BeEquivalentTo(request.Name);
        createdMemberType.Fee.Should().Be(request.Fee);
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task CreateMemberType_Fails_When_NameIsInvalid()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = Fakers.GenerateCreateMemberTypeRequest(name: "");

        // Act
        var result = await client.PostAsJsonAsync("/api/membertypes", request);
        var errors = await result.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("'Name' must not be empty.");
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task CreateMemberType_Fails_WhenNameAlreadyExists()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = Fakers.GenerateCreateMemberTypeRequest();
        var response = await client.PostAsJsonAsync("/api/membertypes", request);
        var createdMemberType = await response.Content.ReadFromJsonAsync<MemberTypeResponse>();
        _createdMemberTypeIds.Add(createdMemberType!.MemberTypeId);

        // Act
        var result = await client.PostAsJsonAsync("/api/membertypes", request);
        var errors = await result.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("This Member Type already exists in the system");
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task GetMemberType_ReturnsMemberType_WhenMemberTypeExists()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = Fakers.GenerateCreateMemberTypeRequest();
        var response = await client.PostAsJsonAsync("/api/membertypes", request);
        var createdMemberType = await response.Content.ReadFromJsonAsync<MemberTypeResponse>();
        _createdMemberTypeIds.Add(createdMemberType!.MemberTypeId);

        // Act
        var result = await client.GetAsync($"/api/membertypes/{createdMemberType.MemberTypeId}");
        var existingMemberType = await result.Content.ReadFromJsonAsync<MemberTypeResponse>();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        existingMemberType.Should().BeEquivalentTo(createdMemberType);
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task GetMemberType_ReturnsNotFound_WhenMemberTypeDoesNotExists()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var result = await client.GetAsync($"/api/membertypes/{900}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task GetAllMemberTypes_ReturnAllMemberTypes_WhenMemberTypesExist()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = Fakers.GenerateCreateMemberTypeRequest();
        var response = await client.PostAsJsonAsync("/api/membertypes", request);
        var createdMemberType = await response.Content.ReadFromJsonAsync<MemberTypeResponse>();
        _createdMemberTypeIds.Add(createdMemberType!.MemberTypeId);
        var createdMemberTypes = new MemberTypesResponse { MemberTypes = new[] { createdMemberType } };

        // Act
        var result = await client.GetAsync("/api/membertypes");
        var existingMemberTypes = await result.Content.ReadFromJsonAsync<MemberTypesResponse>();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        existingMemberTypes.Should().BeEquivalentTo(createdMemberTypes);
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task GetAllMemberTypes_ReturnsNoMemberTypes_WhenNoMemberTypesExist()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var result = await client.GetAsync("/api/membertypes");
        var returnedMemberTypes = await result.Content.ReadFromJsonAsync<MemberTypesResponse>();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedMemberTypes!.MemberTypes.Should().BeEmpty();
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task UpdateMemberType_UpdatesMemberType_WhenDataIsCorrect()
    {
        // Arrange
        using var client = factory.CreateClient();
        var createdRequest = Fakers.GenerateCreateMemberTypeRequest();
        var createdResponse = await client.PostAsJsonAsync("/api/membertypes", createdRequest);
        var createdMemberType = await createdResponse.Content.ReadFromJsonAsync<MemberTypeResponse>();
        var createdId = createdMemberType!.MemberTypeId;
        _createdMemberTypeIds.Add(createdId);

        const string? newName = "NewName";
        var updateRequest = Fakers.GenerateUpdateMemberTypeRequest(name: newName, fee: createdRequest.Fee);

        // Act
        var result = await client.PutAsJsonAsync($"/api/membertypes/{createdId}", updateRequest);
        var updatedMemberType = await result.Content.ReadFromJsonAsync<MemberTypeResponse>();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedMemberType!.MemberTypeId.Should().Be(createdId);
        updatedMemberType.Name.Should().Be(newName);
        updatedMemberType.Fee.Should().Be(updateRequest.Fee);
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task UpdateMemberType_DoesNotUpdateMemberType_WhenDataIsInCorrect()
    {
        // Arrange
        using var client = factory.CreateClient();
        var createdRequest = Fakers.GenerateCreateMemberTypeRequest();
        var createdResponse = await client.PostAsJsonAsync("/api/membertypes", createdRequest);
        var createdMemberType = await createdResponse.Content.ReadFromJsonAsync<MemberTypeResponse>();
        var createdId = createdMemberType!.MemberTypeId;
        _createdMemberTypeIds.Add(createdId);

        var updateRequest = Fakers.GenerateUpdateMemberTypeRequest(name: "", fee: createdRequest.Fee);

        // Act
        var result = await client.PutAsJsonAsync($"/api/membertypes/{createdId}", updateRequest);
        var errors = await result.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("'Name' must not be empty.");
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task UpdateMemberType_ReturnsNotFound_WhenMemberTypeDoesNotExist()
    {
        // Arrange
        using var client = factory.CreateClient();
        var updateRequest = Fakers.GenerateUpdateMemberTypeRequest();
        var badId = new Faker().Random.Int(999999);

        // Act
        var result = await client.PutAsJsonAsync($"/api/membertypes/{badId}", updateRequest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task DeleteMemberType_ReturnsNoContent_WhenMemberTypeIsDeleted()
    {
        // Arrange
        using var client = factory.CreateClient();
        var createdRequest = Fakers.GenerateCreateMemberTypeRequest();
        var createdResponse = await client.PostAsJsonAsync("/api/membertypes", createdRequest);
        var createdMemberType = await createdResponse.Content.ReadFromJsonAsync<MemberTypeResponse>();
        var createdId = createdMemberType!.MemberTypeId;
        _createdMemberTypeIds.Add(createdId);

        // Act
        var result = await client.DeleteAsync($"/api/membertypes/{createdId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(Skip = "MemberType Endpoints Removed.")]
    public async Task DeleteMemberType_ReturnsNotFound_WhenMemberTypeDoesNotExist()
    {
        // Arrange
        using var client = factory.CreateClient();
        var badId = new Faker().Random.Int(999999);

        // Act
        // var result = await httpClient.DeleteAsync($"/books/{isbn}");
        var result = await client.DeleteAsync($"/api/membertypes/{badId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using HttpClient httpClient = factory.CreateClient();
        foreach (int createdId in _createdMemberTypeIds)
        {
            await httpClient.DeleteAsync($"/api/membertypes/{createdId}");
        }
    }
}