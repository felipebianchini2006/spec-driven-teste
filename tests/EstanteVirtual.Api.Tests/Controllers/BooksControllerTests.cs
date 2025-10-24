using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EstanteVirtual.Api.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using EstanteVirtual.Data.Data;

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
}
