using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Members;

public class MemberEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdMemberIds = [];

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var httpClient = golfApiFactory.CreateClient();
        foreach (var createdMemberId in _createdMemberIds)
        {
            await httpClient.DeleteAsync($"/api/members/{createdMemberId}");
        }
    }

    [Fact]
    public async Task GetMemberById_ReturnsMember_WhenMemberExists()
    {
        using var httpClient = golfApiFactory.CreateClient();
        var memberId = 2;

        var response = await httpClient.GetAsync($"/api/members/{memberId}");
        var existingMember = await response.Content.ReadFromJsonAsync<MemberResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        existingMember!.MemberId.Should().Be(memberId);
    }

    [Fact]
    public async Task GetMemberById_ReturnsNotFound_WhenMemberDoesNotExists()
    {
        using var httpClient = golfApiFactory.CreateClient();

        var response = await httpClient.GetAsync("/api/members/200");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllMembers_ReturnsAllMembers_WhenMembersExist()
    {
        using var httpClient = golfApiFactory.CreateClient();

        var response = await httpClient.GetAsync("/api/members");
        var members = await response.Content.ReadFromJsonAsync<MembersResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        members?.Members.Count().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAllMembers_ReturnsNoMembers_WhenNoMembersExist()
    {
        using var httpClient = golfApiFactory.CreateClient();

        var response = await httpClient.GetAsync("/api/members");
        var members = await response.Content.ReadFromJsonAsync<MembersResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        members?.Members.Count().Should().Be(0);
    }
}