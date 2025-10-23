namespace EstanteVirtual.Data.Models;

/// <summary>
/// Represents a book in the user's virtual bookshelf.
/// </summary>
public class Book
{
    /// <summary>
    /// Unique identifier for the book.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Book title (required, max 200 characters).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Author name (required, max 100 characters).
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// URL to book cover image (optional, max 500 characters).
    /// </summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>
    /// Timestamp when book was added to shelf.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Optional review for this book (one-to-zero-or-one relationship).
    /// </summary>
    public Review? Review { get; set; }
}
