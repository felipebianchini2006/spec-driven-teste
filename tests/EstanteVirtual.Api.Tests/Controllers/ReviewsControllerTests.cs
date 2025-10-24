using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EstanteVirtual.Api.Tests.Helpers;

namespace EstanteVirtual.Api.Tests.Controllers;

public class ReviewsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ReviewsControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task POST_Review_CreatesNewReview_Returns201Created()
    {
        // Arrange
        _factory.ResetDatabase();
        
        // Cria um livro primeiro
        var newBook = new { Title = "The Pragmatic Programmer", Author = "Andrew Hunt" };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var book = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var bookId = book.GetProperty("id").GetInt32();

        var newReview = new
        {
            Rating = 5,
            ReviewText = "Um dos melhores livros de programação que já li!"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/books/{bookId}/review", newReview);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var review = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(review.TryGetProperty("id", out var id));
        Assert.True(id.GetInt32() > 0);
        Assert.Equal(5, review.GetProperty("rating").GetInt32());
        Assert.Equal("Um dos melhores livros de programação que já li!", review.GetProperty("reviewText").GetString());
    }

    [Fact]
    public async Task POST_Review_UpdatesExistingReview_Returns200OK()
    {
        // Arrange
        _factory.ResetDatabase();
        
        // Cria um livro
        var newBook = new { Title = "Design Patterns", Author = "Gang of Four" };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var book = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var bookId = book.GetProperty("id").GetInt32();

        // Cria uma review inicial
        var initialReview = new { Rating = 3, ReviewText = "Review inicial" };
        await _client.PostAsJsonAsync($"/api/books/{bookId}/review", initialReview);

        // Act - Atualiza a review
        var updatedReview = new { Rating = 5, ReviewText = "Review atualizada após releitura" };
        var response = await _client.PostAsJsonAsync($"/api/books/{bookId}/review", updatedReview);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var review = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(5, review.GetProperty("rating").GetInt32());
        Assert.Equal("Review atualizada após releitura", review.GetProperty("reviewText").GetString());
    }

    [Fact]
    public async Task POST_Review_WithoutRating_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        
        // Cria um livro
        var newBook = new { Title = "Refactoring", Author = "Martin Fowler" };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var book = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var bookId = book.GetProperty("id").GetInt32();

        var invalidReview = new
        {
            // Rating ausente
            ReviewText = "Texto sem rating"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/books/{bookId}/review", invalidReview);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Review_WithRatingBelowRange_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        
        // Cria um livro
        var newBook = new { Title = "Test Book", Author = "Test Author" };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var book = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var bookId = book.GetProperty("id").GetInt32();

        var invalidReview = new
        {
            Rating = 0, // Abaixo do range (1-5)
            ReviewText = "Rating inválido"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/books/{bookId}/review", invalidReview);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Review_WithRatingAboveRange_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        
        // Cria um livro
        var newBook = new { Title = "Test Book", Author = "Test Author" };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var book = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var bookId = book.GetProperty("id").GetInt32();

        var invalidReview = new
        {
            Rating = 6, // Acima do range (1-5)
            ReviewText = "Rating inválido"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/books/{bookId}/review", invalidReview);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Review_WithoutReviewText_Returns201Created()
    {
        // Arrange
        _factory.ResetDatabase();
        
        // Cria um livro
        var newBook = new { Title = "Test Book", Author = "Test Author" };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var book = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var bookId = book.GetProperty("id").GetInt32();

        var reviewWithoutText = new
        {
            Rating = 4
            // ReviewText ausente (opcional)
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/books/{bookId}/review", reviewWithoutText);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var review = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(4, review.GetProperty("rating").GetInt32());
    }

    [Fact]
    public async Task POST_Review_WithReviewTextExceeding2000Chars_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        
        // Cria um livro
        var newBook = new { Title = "Test Book", Author = "Test Author" };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var book = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var bookId = book.GetProperty("id").GetInt32();

        var longText = new string('A', 2001); // 2001 caracteres - excede o limite
        var invalidReview = new
        {
            Rating = 5,
            ReviewText = longText
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/books/{bookId}/review", invalidReview);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Review_ForNonExistentBook_Returns404NotFound()
    {
        // Arrange
        _factory.ResetDatabase();
        var nonExistentBookId = 9999;

        var validReview = new
        {
            Rating = 5,
            ReviewText = "Review para livro inexistente"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/books/{nonExistentBookId}/review", validReview);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
