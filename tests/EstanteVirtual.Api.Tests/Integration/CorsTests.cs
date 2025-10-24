using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EstanteVirtual.Api.Tests.Helpers;
using Xunit;

namespace EstanteVirtual.Api.Tests.Integration
{
    public class CorsTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CorsTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Options_Request_From_Allowed_Origin_Should_Succeed()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/books");
            request.Headers.Add("Origin", "http://localhost:5248");
            request.Headers.Add("Access-Control-Request-Method", "GET");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
            Assert.Contains("http://localhost:5248", response.Headers.GetValues("Access-Control-Allow-Origin"));
        }

        [Fact]
        public async Task Get_Request_From_Allowed_Origin_Should_Have_Cors_Header()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/books");
            request.Headers.Add("Origin", "http://localhost:5248");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
            Assert.Contains("http://localhost:5248", response.Headers.GetValues("Access-Control-Allow-Origin"));
        }

        [Fact]
        public async Task Options_Request_From_Disallowed_Origin_Should_Not_Have_Cors_Header()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/books");
            request.Headers.Add("Origin", "http://not-allowed.com");
            request.Headers.Add("Access-Control-Request-Method", "GET");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            // A resposta pode ou não ser bem-sucedida dependendo da configuração do servidor,
            // mas o header 'Access-Control-Allow-Origin' NÃO deve estar presente para essa origem.
            Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
        }
    }
}
