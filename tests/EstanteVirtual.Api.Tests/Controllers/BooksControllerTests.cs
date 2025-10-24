using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EstanteVirtual.Api.Tests.Helpers;
using EstanteVirtual.Data.Data;
using Microsoft.Extensions.DependencyInjection;

namespace EstanteVirtual.Api.Tests.Controllers;

public class BooksControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public BooksControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task POST_Books_WithValidData_Returns201Created()
    {
        // Arrange
        _factory.ResetDatabase();
        var newBook = new
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            CoverImageUrl = "https://example.com/cleancode.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", newBook);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var createdBook = JsonSerializer.Deserialize<JsonElement>(responseContent, options);

        Assert.True(createdBook.TryGetProperty("id", out var id));
        Assert.True(id.GetInt32() > 0);
        Assert.Equal("Clean Code", createdBook.GetProperty("title").GetString());
        Assert.Equal("Robert C. Martin", createdBook.GetProperty("author").GetString());
        Assert.Equal("https://example.com/cleancode.jpg", createdBook.GetProperty("coverImageUrl").GetString());

        // Verifica Location header
        Assert.NotNull(response.Headers.Location);
        var locationHeader = response.Headers.Location.ToString();
        Assert.Contains($"/api/Books/{id.GetInt32()}", locationHeader, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task POST_Books_WithoutTitle_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        var invalidBook = new
        {
            Author = "Robert C. Martin",
            CoverImageUrl = "https://example.com/cleancode.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("title", responseContent.ToLower());
    }

    [Fact]
    public async Task POST_Books_WithEmptyTitle_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        var invalidBook = new
        {
            Title = "",
            Author = "Robert C. Martin"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Books_WithoutAuthor_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        var invalidBook = new
        {
            Title = "Clean Code",
            CoverImageUrl = "https://example.com/cleancode.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("author", responseContent.ToLower());
    }

    [Fact]
    public async Task POST_Books_WithEmptyAuthor_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        var invalidBook = new
        {
            Title = "Clean Code",
            Author = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Books_WithCoverUrl_Returns201AndIncludesUrl()
    {
        // Arrange
        _factory.ResetDatabase();
        var newBook = new
        {
            Title = "Design Patterns",
            Author = "Gang of Four",
            CoverImageUrl = "https://example.com/designpatterns.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", newBook);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var createdBook = JsonSerializer.Deserialize<JsonElement>(responseContent, options);

        Assert.Equal("https://example.com/designpatterns.jpg", createdBook.GetProperty("coverImageUrl").GetString());
    }

    [Fact]
    public async Task POST_Books_WithoutCoverUrl_Returns201WithNullCoverUrl()
    {
        // Arrange
        _factory.ResetDatabase();
        var newBook = new
        {
            Title = "Refactoring",
            Author = "Martin Fowler"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", newBook);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var createdBook = JsonSerializer.Deserialize<JsonElement>(responseContent, options);

        // Verifica se coverImageUrl é null ou não está presente
        if (createdBook.TryGetProperty("coverImageUrl", out var coverUrl))
        {
            Assert.Equal(JsonValueKind.Null, coverUrl.ValueKind);
        }
    }

    [Fact]
    public async Task POST_Books_DataPersistence_BookCanBeRetrieved()
    {
        // Arrange
        _factory.ResetDatabase();
        var newBook = new
        {
            Title = "The Pragmatic Programmer",
            Author = "Andrew Hunt and David Thomas",
            CoverImageUrl = "https://example.com/pragmatic.jpg"
        };

        // Act - Adiciona o livro
        var postResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var postContent = await postResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var createdBook = JsonSerializer.Deserialize<JsonElement>(postContent, options);
        var bookId = createdBook.GetProperty("id").GetInt32();

        // Act - Recupera o livro
        var getResponse = await _client.GetAsync($"/api/books/{bookId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getContent = await getResponse.Content.ReadAsStringAsync();
        var retrievedBook = JsonSerializer.Deserialize<JsonElement>(getContent, options);

        Assert.Equal(bookId, retrievedBook.GetProperty("id").GetInt32());
        Assert.Equal("The Pragmatic Programmer", retrievedBook.GetProperty("title").GetString());
        Assert.Equal("Andrew Hunt and David Thomas", retrievedBook.GetProperty("author").GetString());
        Assert.Equal("https://example.com/pragmatic.jpg", retrievedBook.GetProperty("coverImageUrl").GetString());
    }

    [Fact]
    public async Task POST_Books_WithTitleExceeding200Chars_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        var longTitle = new string('A', 201);
        var invalidBook = new
        {
            Title = longTitle,
            Author = "Valid Author"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Books_WithAuthorExceeding100Chars_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        var longAuthor = new string('B', 101);
        var invalidBook = new
        {
            Title = "Valid Title",
            Author = longAuthor
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Books_WithCoverUrlExceeding500Chars_Returns400BadRequest()
    {
        // Arrange
        _factory.ResetDatabase();
        var longUrl = "https://example.com/" + new string('C', 500);
        var invalidBook = new
        {
            Title = "Valid Title",
            Author = "Valid Author",
            CoverImageUrl = longUrl
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ===== User Story 2 Tests - Visualizar Galeria de Livros =====

    [Fact]
    public async Task GET_Books_ReturnsEmptyArray_WhenNoBooksExist()
    {
        // Arrange
        _factory.ResetDatabase();

        // Act
        var response = await _client.GetAsync("/api/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var books = JsonSerializer.Deserialize<JsonElement[]>(responseContent, options);

        Assert.NotNull(books);
        Assert.Empty(books);
    }

    [Fact]
    public async Task GET_Books_ReturnsAllBooks_AfterAddingMultiple()
    {
        // Arrange
        _factory.ResetDatabase();

        // Adiciona 3 livros
        var book1 = new { Title = "Book 1", Author = "Author 1", CoverImageUrl = "https://example.com/1.jpg" };
        var book2 = new { Title = "Book 2", Author = "Author 2", CoverImageUrl = "https://example.com/2.jpg" };
        var book3 = new { Title = "Book 3", Author = "Author 3" }; // Sem capa

        await _client.PostAsJsonAsync("/api/books", book1);
        await _client.PostAsJsonAsync("/api/books", book2);
        await _client.PostAsJsonAsync("/api/books", book3);

        // Act
        var response = await _client.GetAsync("/api/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var books = JsonSerializer.Deserialize<JsonElement[]>(responseContent, options);

        Assert.NotNull(books);
        Assert.Equal(3, books.Length);

        // Verifica que todos os livros estão presentes
        var titles = books.Select(b => b.GetProperty("title").GetString()).ToList();
        Assert.Contains("Book 1", titles);
        Assert.Contains("Book 2", titles);
        Assert.Contains("Book 3", titles);
    }

    [Fact]
    public async Task GET_Books_IncludesBookWithCoverUrl()
    {
        // Arrange
        _factory.ResetDatabase();
        var bookWithCover = new
        {
            Title = "Design Patterns",
            Author = "Gang of Four",
            CoverImageUrl = "https://example.com/designpatterns.jpg"
        };

        await _client.PostAsJsonAsync("/api/books", bookWithCover);

        // Act
        var response = await _client.GetAsync("/api/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var books = JsonSerializer.Deserialize<JsonElement[]>(responseContent, options);

        Assert.NotNull(books);
        Assert.Single(books);

        var book = books[0];
        Assert.Equal("Design Patterns", book.GetProperty("title").GetString());
        Assert.Equal("Gang of Four", book.GetProperty("author").GetString());
        Assert.Equal("https://example.com/designpatterns.jpg", book.GetProperty("coverImageUrl").GetString());
    }

    [Fact]
    public async Task GET_Books_IncludesBookWithoutCoverUrl()
    {
        // Arrange
        _factory.ResetDatabase();
        var bookWithoutCover = new
        {
            Title = "Refactoring",
            Author = "Martin Fowler"
            // Sem CoverImageUrl
        };

        await _client.PostAsJsonAsync("/api/books", bookWithoutCover);

        // Act
        var response = await _client.GetAsync("/api/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var books = JsonSerializer.Deserialize<JsonElement[]>(responseContent, options);

        Assert.NotNull(books);
        Assert.Single(books);

        var book = books[0];
        Assert.Equal("Refactoring", book.GetProperty("title").GetString());
        Assert.Equal("Martin Fowler", book.GetProperty("author").GetString());

        // Verifica que coverImageUrl é null
        if (book.TryGetProperty("coverImageUrl", out var coverUrl))
        {
            Assert.Equal(JsonValueKind.Null, coverUrl.ValueKind);
        }
    }

    // ===== User Story 3 Tests - Avaliar Livro =====

    [Fact]
    public async Task GET_BooksById_ReturnsBookWithReview_WhenReviewExists()
    {
        // Arrange
        _factory.ResetDatabase();

        // Cria um livro
        var newBook = new { Title = "Clean Architecture", Author = "Robert C. Martin" };
        var createResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var createdBook = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var bookId = createdBook.GetProperty("id").GetInt32();

        // Adiciona uma review
        var review = new { Rating = 5, ReviewText = "Excelente livro!" };
        await _client.PostAsJsonAsync($"/api/books/{bookId}/review", review);

        // Act
        var response = await _client.GetAsync($"/api/books/{bookId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var book = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(book.TryGetProperty("review", out var reviewProp));
        Assert.NotEqual(JsonValueKind.Null, reviewProp.ValueKind);
        Assert.Equal(5, reviewProp.GetProperty("rating").GetInt32());
        Assert.Equal("Excelente livro!", reviewProp.GetProperty("reviewText").GetString());
    }

    [Fact]
    public async Task GET_BooksById_Returns404_WhenBookDoesNotExist()
    {
        // Arrange
        _factory.ResetDatabase();
        var nonExistentId = 9999;

        // Act
        var response = await _client.GetAsync($"/api/books/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
