using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstanteVirtual.Data.Data;
using EstanteVirtual.Data.Models;
using EstanteVirtual.Api.DTOs;

namespace EstanteVirtual.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de avaliações (reviews) de livros.
/// </summary>
[ApiController]
[Route("api/books/{bookId}/review")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(AppDbContext context, ILogger<ReviewsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Cria ou atualiza a avaliação de um livro.
    /// Cada livro pode ter no máximo uma avaliação (1:1 relationship).
    /// </summary>
    /// <param name="bookId">ID do livro.</param>
    /// <param name="createReviewDto">Dados da avaliação.</param>
    /// <returns>A avaliação criada ou atualizada.</returns>
    /// <response code="201">Avaliação criada com sucesso.</response>
    /// <response code="200">Avaliação atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos.</response>
    /// <response code="404">Livro não encontrado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> CreateOrUpdateReview(
        int bookId, 
        [FromBody] CreateReviewDto createReviewDto)
    {
        _logger.LogInformation("Criando/atualizando review para livro ID: {BookId}", bookId);

        // Model validation é automática com [ApiController]
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Validação falhou ao criar/atualizar review: {Errors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        // Verifica se o livro existe
        var book = await _context.Books
            .Include(b => b.Review)
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
        {
            _logger.LogWarning("Livro não encontrado: ID {BookId}", bookId);
            return NotFound(new { message = $"Livro com ID {bookId} não encontrado." });
        }

        Review review;
        bool isUpdate = false;

        // Se já existe uma review, atualiza
        if (book.Review != null)
        {
            _logger.LogInformation("Atualizando review existente. ID: {ReviewId}", book.Review.Id);
            review = book.Review;
            review.Rating = createReviewDto.Rating;
            review.ReviewText = createReviewDto.ReviewText;
            review.UpdatedAt = DateTime.UtcNow;
            isUpdate = true;
        }
        else
        {
            // Cria nova review
            _logger.LogInformation("Criando nova review para livro ID: {BookId}", bookId);
            review = new Review
            {
                BookId = bookId,
                Rating = createReviewDto.Rating,
                ReviewText = createReviewDto.ReviewText,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Reviews.Add(review);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Review {Action} com sucesso. ID: {ReviewId}", 
            isUpdate ? "atualizada" : "criada", review.Id);

        // Mapeia para DTO de resposta
        var reviewDto = new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            ReviewText = review.ReviewText,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };

        if (isUpdate)
        {
            return Ok(reviewDto);
        }
        else
        {
            return CreatedAtAction(
                nameof(GetReview), 
                new { bookId = bookId }, 
                reviewDto);
        }
    }

    /// <summary>
    /// Obtém a avaliação de um livro específico.
    /// </summary>
    /// <param name="bookId">ID do livro.</param>
    /// <returns>A avaliação do livro.</returns>
    /// <response code="200">Avaliação encontrada.</response>
    /// <response code="404">Livro ou avaliação não encontrados.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> GetReview(int bookId)
    {
        _logger.LogInformation("Buscando review do livro ID: {BookId}", bookId);

        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.BookId == bookId);

        if (review == null)
        {
            _logger.LogWarning("Review não encontrada para livro ID: {BookId}", bookId);
            return NotFound(new { message = $"Avaliação não encontrada para o livro com ID {bookId}." });
        }

        var reviewDto = new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            ReviewText = review.ReviewText,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };

        return Ok(reviewDto);
    }
}
