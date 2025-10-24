using System.ComponentModel.DataAnnotations;

namespace EstanteVirtual.Web.DTOs;

/// <summary>
/// DTO para criação de um novo livro.
/// </summary>
public class CreateBookDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, ErrorMessage = "O título não pode exceder 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "O autor é obrigatório.")]
    [StringLength(100, ErrorMessage = "O autor não pode exceder 100 caracteres.")]
    public string Author { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A URL da imagem não pode exceder 500 caracteres.")]
    public string? CoverImageUrl { get; set; }
}
