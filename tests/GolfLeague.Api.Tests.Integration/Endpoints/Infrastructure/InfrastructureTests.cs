using System.Net;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Infrastructure;

public class DatabaseNotNeededFactory : WebApplicationFactory<IGolfApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:GolfLeagueDb",
            "Server=localhost,9433;Connect Timeout=1;TrustServerCertificate=True;");
        base.ConfigureWebHost(builder);
    }
}

public class InfrastructureTests(DatabaseNotNeededFactory factory) : IClassFixture<DatabaseNotNeededFactory>
{
    [Fact]
    public async Task Authentication_WhenJwtTokenIsExpired_ShouldFailWithUnauthenticated()
    {
        // arrange
        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(customTimeSpan: TimeSpan.FromSeconds(1)));

        await Task.Delay(TimeSpan.FromSeconds(7)); // make sure the ClockSkew (5 seconds) has passed.

        // act
        var response = await client.GetAsync(Mother.GolfersApiBasePath);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HealthCheck_WhenUnhealthy_ShouldReturnUnhealthy()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        // var response = await client.GetAsync("/swagger/v1/swagger.json");
        var response = await client.GetAsync("/_health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

        var message = await response.Content.ReadAsStringAsync();
        message.Should().Be("Unhealthy");
    }

    [Fact]
    public async Task Swagger_WhenCalled_ShouldReturnOK()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}