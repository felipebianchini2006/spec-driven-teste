using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EstanteVirtual.Api.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace EstanteVirtual.Api.Tests.Integration
{
    public class ErrorHandlingTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ErrorHandlingTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Request_To_NonExistent_Endpoint_Should_Return_404NotFound()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/non-existent-route");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Este teste assume que há um endpoint que pode lançar uma exceção não tratada.
        // Como nosso middleware global deve capturar tudo, podemos simular isso
        // ou testar um cenário que sabemos que causa um erro interno.
        // No entanto, um teste mais robusto exigiria a injeção de um serviço mock
        // que lança uma exceção. Para este escopo, vamos nos concentrar no que já temos.

        // O teste para um 500 Internal Server Error é mais complexo de simular de forma confiável
        // sem mocks. O middleware de exceção já está configurado em Program.cs.
        // Um teste manual ou um teste com um mock que lança exceção seria o ideal.
        // Por exemplo, se o banco de dados estivesse offline, teríamos um 500.

        // Vamos testar o comportamento de 404 para um livro que não existe,
        // que é um erro tratado, mas mostra o pipeline de resposta de erro.
        [Fact]
        public async Task Get_NonExistent_Book_Should_Return_404NotFound_With_ProblemDetails()
        {
            // Arrange
            var nonExistentId = 99999;

            // Act
            var response = await _client.GetAsync($"/api/books/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problemDetails);
            Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
        }
    }
}
