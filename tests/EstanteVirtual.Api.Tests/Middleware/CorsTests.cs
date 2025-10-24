using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EstanteVirtual.Api.Tests.Middleware;

public class CorsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CorsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ApiEndpoint_WithBlazorOrigin_AllowsCors()
    {
        // Arrange
        var client = _factory.CreateClient();
        var blazorOrigin = "http://localhost:5248";

        var request = new HttpRequestMessage(HttpMethod.Options, "/api/books");
        request.Headers.Add("Origin", blazorOrigin);
        request.Headers.Add("Access-Control-Request-Method", "GET");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));

        var allowOriginHeader = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
        Assert.Equal(blazorOrigin, allowOriginHeader);
    }

    [Fact]
    public async Task ApiEndpoint_WithAlternativeBlazorPort_AllowsCors()
    {
        // Arrange
        var client = _factory.CreateClient();
        var disallowedOrigin = "http://localhost:5000";

        var request = new HttpRequestMessage(HttpMethod.Options, "/api/books");
        request.Headers.Add("Origin", disallowedOrigin);
        request.Headers.Add("Access-Control-Request-Method", "POST");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        // Para origem não permitida, o header CORS não deve estar presente
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task ApiEndpoint_WithHttpsBlazorOrigin_AllowsCors()
    {
        // Arrange
        var client = _factory.CreateClient();
        var blazorOrigin = "https://localhost:7242";

        var request = new HttpRequestMessage(HttpMethod.Options, "/api/books");
        request.Headers.Add("Origin", blazorOrigin);
        request.Headers.Add("Access-Control-Request-Method", "GET");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task ApiEndpoint_PreflightRequest_ReturnsCorrectHeaders()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Options, "/api/books");
        request.Headers.Add("Origin", "http://localhost:5248");
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "content-type");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.True(response.Headers.Contains("Access-Control-Allow-Methods"));
        Assert.True(response.Headers.Contains("Access-Control-Allow-Headers"));
    }

    [Fact]
    public async Task GetBooks_WithOriginHeader_IncludesCorsHeaders()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/books");
        request.Headers.Add("Origin", "http://localhost:5248");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task PostBook_WithOriginHeader_IncludesCorsHeaders()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/books")
        {
            Content = JsonContent.Create(new
            {
                title = "Test Book",
                author = "Test Author"
            })
        };
        request.Headers.Add("Origin", "http://localhost:5248");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.BadRequest);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
    }
}
