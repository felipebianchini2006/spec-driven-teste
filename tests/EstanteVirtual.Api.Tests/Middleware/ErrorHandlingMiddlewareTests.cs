using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EstanteVirtual.Api.Tests.Middleware;

public class ErrorHandlingMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ErrorHandlingMiddlewareTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetBook_WithInvalidId_ReturnsNotFoundWithErrorMessage()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/books/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        // Resposta segue ProblemDetails padrão
        var pd = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(pd);
        Assert.Equal((int)HttpStatusCode.NotFound, pd!.Status);
    }

    [Fact]
    public async Task PostBook_WithInvalidData_ReturnsBadRequestWithValidationErrors()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidBook = new { title = "", author = "" }; // Campos vazios

        // Act
        var response = await client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task PostReview_ForNonExistentBook_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var review = new { rating = 5, reviewText = "Great book!" };

        // Act
        var response = await client.PostAsJsonAsync("/api/books/99999/review", review);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var pd = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(pd);
        Assert.Equal((int)HttpStatusCode.NotFound, pd!.Status);
    }

    [Fact]
    public async Task GetReview_ForNonExistentReview_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Primeiro cria um livro
        var book = new { title = "Test Book", author = "Test Author" };
        var bookResponse = await client.PostAsJsonAsync("/api/books", book);
        var bookContent = await bookResponse.Content.ReadAsStringAsync();
        var bookDto = JsonSerializer.Deserialize<JsonElement>(bookContent);
        var bookId = bookDto.GetProperty("id").GetInt32();

        // Act - Tenta buscar review que não existe
        var response = await client.GetAsync($"/api/books/{bookId}/review");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var pd = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(pd);
        Assert.Equal((int)HttpStatusCode.NotFound, pd!.Status);
    }

    [Fact]
    public async Task PostBook_WithExcessivelyLongTitle_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidBook = new
        {
            title = new string('A', 201), // Excede o limite de 200 caracteres
            author = "Valid Author"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostReview_WithInvalidRating_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Primeiro cria um livro
        var book = new { title = "Test Book", author = "Test Author" };
        var bookResponse = await client.PostAsJsonAsync("/api/books", book);
        var bookContent = await bookResponse.Content.ReadAsStringAsync();
        var bookDto = JsonSerializer.Deserialize<JsonElement>(bookContent);
        var bookId = bookDto.GetProperty("id").GetInt32();

        // Tenta criar review com rating inválido
        var invalidReview = new { rating = 10, reviewText = "Invalid rating" };

        // Act
        var response = await client.PostAsJsonAsync($"/api/books/{bookId}/review", invalidReview);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBooks_Always_ReturnsSuccessStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Api_WithInvalidRoute_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/nonexistent");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
