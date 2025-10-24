namespace EstanteVirtual.Api.DTOs;

/// <summary>
/// DTO para representação de um livro (output).
/// Usado nas respostas da API.
/// </summary>
public class BookDto
{
    /// <summary>
    /// Identificador único do livro.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Título do livro.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Autor do livro.
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// URL da imagem de capa (pode ser nula).
    /// </summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>
    /// Data e hora de criação do livro.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Review associada ao livro (pode ser nula).
    /// </summary>
    public ReviewDto? Review { get; set; }
}

/// <summary>
/// DTO para representação de uma review (output).
/// </summary>
public class ReviewDto
{
    /// <summary>
    /// Identificador único da review.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Avaliação de 1 a 5 estrelas.
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Texto da review (opcional).
    /// </summary>
    public string? ReviewText { get; set; }

    /// <summary>
    /// Data e hora de criação da review.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
