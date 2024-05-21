using System.Net;

using FluentAssertions;

using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints;

public class MemberTypesEndpointTests(GolfApiFactory factory) : IClassFixture<GolfApiFactory>, IAsyncLifetime
{
    private readonly List<int> _createdMemberTypeIds = [];

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using HttpClient httpClient = factory.CreateClient();
        foreach (int createdId in _createdMemberTypeIds)
        {
            await httpClient.DeleteAsync($"/api/membertypes/{createdId}");
        }
    }

    [Fact]
    public async Task CreateMemberType_CreatesMemberType_WhenDataIsCorrect()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = new CreateMemberTypeRequest { Name = "Senior", Fee = 25.00m };

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

    [Fact]
    public async Task CreateMemberType_Fails_When_NameIsInvalid()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = new CreateMemberTypeRequest { Name = "", Fee = 0m };

        // Act
        var result = await client.PostAsJsonAsync("/api/membertypes", request);
        var errors = await result.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        var error = errors!.Errors.Single();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("'Name' must not be empty.");
    }

    [Fact]
    public async Task CreateMemberType_Fails_WhenNameAlreadyExists()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = new CreateMemberTypeRequest { Name = "Social", Fee = 200m };
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
}