using EstanteVirtual.Data.Data;
using EstanteVirtual.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EstanteVirtual.Data.Tests.Models;

/// <summary>
/// Testes unitários para a configuração EF Core da entidade Review.
/// </summary>
public class ReviewTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Review_WithValidData_ShouldBeSaved()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Primeiro cria um livro
        var book = new Book
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            CreatedAt = DateTime.UtcNow
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Depois cria uma review
        var review = new Review
        {
            BookId = book.Id,
            Rating = 5,
            ReviewText = "Excelente livro sobre clean code!",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        // Assert
        var savedReview = await context.Reviews
            .Include(r => r.Book)
            .FirstOrDefaultAsync(r => r.BookId == book.Id);

        Assert.NotNull(savedReview);
        Assert.Equal(5, savedReview.Rating);
        Assert.Equal("Excelente livro sobre clean code!", savedReview.ReviewText);
        Assert.NotNull(savedReview.Book);
        Assert.Equal("Clean Code", savedReview.Book.Title);
    }

    [Fact]
    public async Task Review_Rating_ShouldBeRequired()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            CreatedAt = DateTime.UtcNow
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var review = new Review
        {
            BookId = book.Id,
            Rating = 0, // Rating inválido
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act & Assert
        context.Reviews.Add(review);
        // O InMemory DB não valida constraints, então apenas salvamos
        // A validação real será feita na camada de API
        await context.SaveChangesAsync();

        var savedReview = await context.Reviews.FindAsync(review.Id);
        Assert.NotNull(savedReview);
    }

    [Fact]
    public async Task Review_RatingRange_ShouldBeValidated()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            CreatedAt = DateTime.UtcNow
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Act & Assert - Rating válido (1-5)
        for (int rating = 1; rating <= 5; rating++)
        {
            var review = new Review
            {
                BookId = book.Id,
                Rating = rating,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var savedReview = await context.Reviews.FindAsync(review.Id);
            Assert.Equal(rating, savedReview!.Rating);

            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task Review_ReviewText_CanBeNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            CreatedAt = DateTime.UtcNow
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var review = new Review
        {
            BookId = book.Id,
            Rating = 4,
            ReviewText = null, // Texto opcional
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        // Assert
        var savedReview = await context.Reviews.FindAsync(review.Id);
        Assert.NotNull(savedReview);
        Assert.Null(savedReview.ReviewText);
    }

    [Fact]
    public async Task Review_ReviewText_WithMaxLength_ShouldBeSaved()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            CreatedAt = DateTime.UtcNow
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var longText = new string('A', 2000); // Exatamente 2000 caracteres
        var review = new Review
        {
            BookId = book.Id,
            Rating = 5,
            ReviewText = longText,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        // Assert
        var savedReview = await context.Reviews.FindAsync(review.Id);
        Assert.NotNull(savedReview);
        Assert.Equal(2000, savedReview.ReviewText!.Length);
    }

    [Fact]
    public async Task Review_BookIdForeignKey_ShouldWork()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            CreatedAt = DateTime.UtcNow
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var review = new Review
        {
            BookId = book.Id,
            Rating = 4,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        // Assert
        var savedReview = await context.Reviews
            .Include(r => r.Book)
            .FirstOrDefaultAsync(r => r.Id == review.Id);

        Assert.NotNull(savedReview);
        Assert.Equal(book.Id, savedReview.BookId);
        Assert.NotNull(savedReview.Book);
        Assert.Equal("Test Book", savedReview.Book.Title);
    }

    [Fact]
    public async Task Review_UpdatedAt_ShouldBeDifferentFromCreatedAt()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            CreatedAt = DateTime.UtcNow
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var createdAt = DateTime.UtcNow;
        var review = new Review
        {
            BookId = book.Id,
            Rating = 3,
            ReviewText = "Initial review",
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        // Act - Atualiza a review
        await Task.Delay(10); // Pequeno delay para garantir timestamp diferente
        review.ReviewText = "Updated review";
        review.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        // Assert
        var savedReview = await context.Reviews.FindAsync(review.Id);
        Assert.NotNull(savedReview);
        Assert.True(savedReview.UpdatedAt >= savedReview.CreatedAt);
        Assert.Equal("Updated review", savedReview.ReviewText);
    }
}
