using System.Net;

using FluentAssertions;

namespace GolfLeague.Api.Tests.Integration.Endpoints.HealthCheck;

public class HealthCheckTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task HealthCheck_Returns_Healthy()
    {
        using var client = golfApiFactory.CreateClient();

        var response = await client.GetAsync("/_health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var message = await response.Content.ReadAsStringAsync();
        message.Should().Be("Healthy");
    }
}