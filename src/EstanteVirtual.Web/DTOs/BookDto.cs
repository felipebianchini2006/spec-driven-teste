namespace EstanteVirtual.Web.DTOs;

/// <summary>
/// DTO para representação de um livro (output).
/// </summary>
public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public ReviewDto? Review { get; set; }
}

/// <summary>
/// DTO para representação de uma review (output).
/// </summary>
public class ReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
