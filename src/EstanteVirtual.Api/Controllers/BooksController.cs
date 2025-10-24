using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstanteVirtual.Data.Data;
using EstanteVirtual.Data.Models;
using EstanteVirtual.Api.DTOs;

namespace EstanteVirtual.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de livros da Estante Virtual.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<BooksController> _logger;

    public BooksController(AppDbContext context, ILogger<BooksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Adiciona um novo livro à estante.
    /// </summary>
    /// <param name="createBookDto">Dados do livro a ser criado.</param>
    /// <returns>O livro criado com status 201 Created.</returns>
    /// <response code="201">Livro criado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto createBookDto)
    {
        _logger.LogInformation("Criando novo livro: {Title} por {Author}", createBookDto.Title, createBookDto.Author);

        // Model validation é automática com [ApiController]
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Validação falhou ao criar livro: {Errors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        // Cria a entidade Book
        var book = new Book
        {
            Title = createBookDto.Title.Trim(),
            Author = createBookDto.Author.Trim(),
            CoverImageUrl = string.IsNullOrWhiteSpace(createBookDto.CoverImageUrl) 
                ? null 
                : createBookDto.CoverImageUrl.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        // Adiciona ao contexto e salva
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Livro criado com sucesso. ID: {BookId}", book.Id);

        // Mapeia para DTO de resposta
        var bookDto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CoverImageUrl = book.CoverImageUrl,
            CreatedAt = book.CreatedAt,
            Review = null // Novo livro não tem review
        };

        // Retorna 201 Created com Location header
        return CreatedAtAction(
            nameof(GetBook), 
            new { id = book.Id }, 
            bookDto);
    }

    /// <summary>
    /// Obtém um livro específico por ID.
    /// </summary>
    /// <param name="id">ID do livro.</param>
    /// <returns>O livro solicitado.</returns>
    /// <response code="200">Livro encontrado.</response>
    /// <response code="404">Livro não encontrado.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        _logger.LogInformation("Buscando livro com ID: {BookId}", id);

        var book = await _context.Books
            .Include(b => b.Review)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            _logger.LogWarning("Livro não encontrado: ID {BookId}", id);
            return NotFound(new { message = $"Livro com ID {id} não encontrado." });
        }

        var bookDto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CoverImageUrl = book.CoverImageUrl,
            CreatedAt = book.CreatedAt,
            Review = book.Review == null ? null : new ReviewDto
            {
                Id = book.Review.Id,
                Rating = book.Review.Rating,
                ReviewText = book.Review.ReviewText,
                CreatedAt = book.Review.CreatedAt,
                UpdatedAt = book.Review.UpdatedAt
            }
        };

        return Ok(bookDto);
    }

    /// <summary>
    /// Lista todos os livros da estante.
    /// </summary>
    /// <returns>Lista de todos os livros.</returns>
    /// <response code="200">Lista de livros retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
    {
        _logger.LogInformation("Listando todos os livros");

        var books = await _context.Books
            .Include(b => b.Review)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var bookDtos = books.Select(book => new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CoverImageUrl = book.CoverImageUrl,
            CreatedAt = book.CreatedAt,
            Review = book.Review == null ? null : new ReviewDto
            {
                Id = book.Review.Id,
                Rating = book.Review.Rating,
                ReviewText = book.Review.ReviewText,
                CreatedAt = book.Review.CreatedAt,
                UpdatedAt = book.Review.UpdatedAt
            }
        }).ToList();

        return Ok(bookDtos);
    }
}
