using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CodeCareer.Tests.Integration;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("environment", "Testing");
            builder.UseSetting("ConnectionStrings:DefaultConnection",
                "Server=127.0.0.1;Port=3306;Database=codecareer_test;User=root;Password=root;");
        });
    }

    [Fact]
    public async Task Health_ReturnsOkOrServiceUnavailableWithoutDb()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/health");
        Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.ServiceUnavailable);
    }
}

public class PublicPagesTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PublicPagesTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("environment", "Testing");
            builder.UseSetting("ConnectionStrings:DefaultConnection",
                "Server=127.0.0.1;Port=3306;Database=codecareer_test;User=root;Password=root;");
        });
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
    }

    private readonly WebApplicationFactory<Program> _factory;

    [Fact]
    public async Task HomePage_IsAccessible()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task LearningIndex_ReturnsResponseWithout404()
    {
        var response = await _client.GetAsync("/User/Learning/Index");
        // Without MySQL in test host, page may return 500; ensure routing exists (not 404).
        Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
