using System.ComponentModel.DataAnnotations;

namespace EstanteVirtual.Web.DTOs;

/// <summary>
/// DTO para criação/atualização de uma avaliação (review).
/// </summary>
public class CreateReviewDto
{
    [Required(ErrorMessage = "A nota é obrigatória.")]
    [Range(1, 5, ErrorMessage = "A nota deve estar entre 1 e 5.")]
    public int Rating { get; set; }

    [StringLength(2000, ErrorMessage = "A resenha não pode exceder 2000 caracteres.")]
    public string? ReviewText { get; set; }
}
