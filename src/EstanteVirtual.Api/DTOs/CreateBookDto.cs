using System.ComponentModel.DataAnnotations;

namespace EstanteVirtual.Api.DTOs;

/// <summary>
/// DTO para criação de um novo livro (input).
/// Usado no endpoint POST /api/books.
/// </summary>
public class CreateBookDto
{
    /// <summary>
    /// Título do livro (obrigatório, máximo 200 caracteres).
    /// </summary>
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, ErrorMessage = "O título não pode exceder 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Autor do livro (obrigatório, máximo 100 caracteres).
    /// </summary>
    [Required(ErrorMessage = "O autor é obrigatório.")]
    [StringLength(100, ErrorMessage = "O autor não pode exceder 100 caracteres.")]
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// URL da imagem de capa (opcional, máximo 500 caracteres).
    /// </summary>
    [StringLength(500, ErrorMessage = "A URL da capa não pode exceder 500 caracteres.")]
    public string? CoverImageUrl { get; set; }
}
