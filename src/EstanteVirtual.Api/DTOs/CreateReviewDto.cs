using System.ComponentModel.DataAnnotations;

namespace EstanteVirtual.Api.DTOs;

/// <summary>
/// DTO para criação/atualização de review (input).
/// Usado no endpoint POST /api/books/{id}/review.
/// </summary>
public class CreateReviewDto
{
    /// <summary>
    /// Avaliação de 1 a 5 estrelas (obrigatório).
    /// </summary>
    [Required(ErrorMessage = "A avaliação é obrigatória.")]
    [Range(1, 5, ErrorMessage = "A avaliação deve estar entre 1 e 5 estrelas.")]
    public int Rating { get; set; }

    /// <summary>
    /// Texto da resenha (opcional, máximo 2000 caracteres).
    /// </summary>
    [StringLength(2000, ErrorMessage = "O texto da resenha não pode exceder 2000 caracteres.")]
    public string? ReviewText { get; set; }
}
