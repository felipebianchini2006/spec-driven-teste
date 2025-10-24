using System.Net.Http.Json;
using EstanteVirtual.Web.DTOs;

namespace EstanteVirtual.Web.Services;

/// <summary>
/// Serviço para comunicação com a API de livros.
/// </summary>
public class BookApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BookApiService> _logger;

    public BookApiService(IHttpClientFactory httpClientFactory, ILogger<BookApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo livro na estante.
    /// </summary>
    /// <param name="title">Título do livro.</param>
    /// <param name="author">Autor do livro.</param>
    /// <param name="coverImageUrl">URL da capa (opcional).</param>
    /// <returns>O livro criado ou null se falhar.</returns>
    public async Task<BookDto?> CreateBookAsync(string title, string author, string? coverImageUrl)
    {
        try
        {
            _logger.LogInformation("Criando livro via API: {Title} por {Author}", title, author);

            var httpClient = _httpClientFactory.CreateClient("EstanteVirtualApi");

            var createBookDto = new
            {
                Title = title,
                Author = author,
                CoverImageUrl = coverImageUrl
            };

            var response = await httpClient.PostAsJsonAsync("/api/books", createBookDto);

            if (response.IsSuccessStatusCode)
            {
                var book = await response.Content.ReadFromJsonAsync<BookDto>();
                _logger.LogInformation("Livro criado com sucesso. ID: {BookId}", book?.Id);
                return book;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Falha ao criar livro. Status: {StatusCode}, Erro: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção ao criar livro: {Message}", ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Obtém todos os livros da estante.
    /// </summary>
    /// <returns>Lista de livros ou lista vazia se falhar.</returns>
    public async Task<List<BookDto>> GetAllBooksAsync()
    {
        try
        {
            _logger.LogInformation("Carregando todos os livros via API");

            var httpClient = _httpClientFactory.CreateClient("EstanteVirtualApi");
            var response = await httpClient.GetAsync("/api/books");

            if (response.IsSuccessStatusCode)
            {
                var books = await response.Content.ReadFromJsonAsync<List<BookDto>>() ?? new List<BookDto>();
                _logger.LogInformation("Carregados {Count} livros", books.Count);
                return books;
            }
            else
            {
                _logger.LogWarning("Falha ao carregar livros. Status: {StatusCode}", response.StatusCode);
                return new List<BookDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção ao carregar livros: {Message}", ex.Message);
            return new List<BookDto>();
        }
    }

    /// <summary>
    /// Obtém um livro específico pelo ID.
    /// </summary>
    /// <param name="id">Identificador do livro.</param>
    /// <returns>O livro encontrado ou null quando inexistente ou erro.</returns>
    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Carregando livro {BookId} via API", id);

            var httpClient = _httpClientFactory.CreateClient("EstanteVirtualApi");
            var response = await httpClient.GetAsync($"/api/books/{id}");

            if (response.IsSuccessStatusCode)
            {
                var book = await response.Content.ReadFromJsonAsync<BookDto>();
                if (book != null)
                {
                    _logger.LogInformation("Livro {BookId} carregado com sucesso", id);
                }
                else
                {
                    _logger.LogWarning("Resposta da API não pôde ser desserializada ao buscar livro {BookId}", id);
                }

                return book;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Livro {BookId} não encontrado", id);
                return null;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Falha ao carregar livro {BookId}. Status: {StatusCode}. Corpo: {Body}",
                id, response.StatusCode, errorContent);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção ao carregar livro {BookId}: {Message}", id, ex.Message);
            return null;
        }
    }
}
