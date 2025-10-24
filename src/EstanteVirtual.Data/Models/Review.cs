namespace EstanteVirtual.Data.Models;

/// <summary>
/// Represents a user review for a book (rating + optional text).
/// Each book can have at most one review (1:1 relationship).
/// </summary>
public class Review
{
    /// <summary>
    /// Unique identifier for the review.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the book being reviewed.
    /// Each book can have at most one review (unique constraint).
    /// </summary>
    public int BookId { get; set; }

    /// <summary>
    /// Rating from 1 to 5 stars (required).
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Optional review text (max 2000 characters).
    /// </summary>
    public string? ReviewText { get; set; }

    /// <summary>
    /// Timestamp when review was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when review was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to the associated book.
    /// </summary>
    public Book Book { get; set; } = null!;
}
