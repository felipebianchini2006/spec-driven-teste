using EstanteVirtual.Data.Data;
using EstanteVirtual.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EstanteVirtual.Data.Tests.Models;

/// <summary>
/// Testes unitários para a configuração EF Core da entidade Book.
/// Nota: Validações de negócio (required, max length) são testadas na camada API via DTOs.
/// Aqui testamos apenas a estrutura da entidade e configuração do DbContext.
/// </summary>
public class BookTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Book_WithValidData_ShouldBeSaved()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var book = new Book
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            CoverImageUrl = "https://example.com/cover.jpg",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Assert
        var savedBook = await context.Books.FirstOrDefaultAsync(b => b.Title == "Clean Code");
        Assert.NotNull(savedBook);
        Assert.Equal("Clean Code", savedBook.Title);
        Assert.Equal("Robert C. Martin", savedBook.Author);
        Assert.Equal("https://example.com/cover.jpg", savedBook.CoverImageUrl);
    }

    [Fact]
    public async Task Book_CoverImageUrl_CanBeNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var book = new Book
        {
            Title = "Refactoring",
            Author = "Martin Fowler",
            CoverImageUrl = null,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Assert
        var savedBook = await context.Books.FirstOrDefaultAsync(b => b.Title == "Refactoring");
        Assert.NotNull(savedBook);
        Assert.Null(savedBook.CoverImageUrl);
    }

    [Fact]
    public async Task Book_CreatedAt_ShouldBePersisted()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var now = DateTime.UtcNow;
        var book = new Book
        {
            Title = "Design Patterns",
            Author = "Gang of Four",
            CreatedAt = now
        };

        // Act
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Assert
        var savedBook = await context.Books.FirstOrDefaultAsync(b => b.Title == "Design Patterns");
        Assert.NotNull(savedBook);
        Assert.Equal(now.Date, savedBook.CreatedAt.Date);
    }

    [Fact]
    public async Task Book_Review_NavigationProperty_ShouldWork()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var book = new Book
        {
            Title = "The Pragmatic Programmer",
            Author = "Andrew Hunt",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Assert
        var savedBook = await context.Books
            .Include(b => b.Review)
            .FirstOrDefaultAsync(b => b.Title == "The Pragmatic Programmer");

        Assert.NotNull(savedBook);
        Assert.Null(savedBook.Review); // Novo livro não tem review
    }

    [Fact]
    public async Task Book_MultipleBooks_CanBeSaved()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var books = new List<Book>
        {
            new Book { Title = "Book 1", Author = "Author 1", CreatedAt = DateTime.UtcNow },
            new Book { Title = "Book 2", Author = "Author 2", CreatedAt = DateTime.UtcNow },
            new Book { Title = "Book 3", Author = "Author 3", CreatedAt = DateTime.UtcNow }
        };

        // Act
        context.Books.AddRange(books);
        await context.SaveChangesAsync();

        // Assert
        var count = await context.Books.CountAsync();
        Assert.Equal(3, count);
    }
}
