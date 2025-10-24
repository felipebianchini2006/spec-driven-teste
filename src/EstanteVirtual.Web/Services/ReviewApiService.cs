using System.Net.Http.Json;
using System.Text.Json;
using EstanteVirtual.Web.DTOs;

namespace EstanteVirtual.Web.Services;

/// <summary>
/// Serviço para comunicação com a API de avaliações (reviews).
/// </summary>
public class ReviewApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReviewApiService> _logger;

    public ReviewApiService(IHttpClientFactory httpClientFactory, ILogger<ReviewApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("EstanteVirtualApi");
        _logger = logger;
    }

    /// <summary>
    /// Cria ou atualiza uma avaliação para um livro.
    /// </summary>
    /// <param name="bookId">ID do livro.</param>
    /// <param name="createReviewDto">Dados da avaliação.</param>
    /// <returns>A avaliação criada ou atualizada, ou null em caso de erro.</returns>
    public async Task<ReviewDto?> CreateOrUpdateReviewAsync(int bookId, CreateReviewDto createReviewDto)
    {
        try
        {
            _logger.LogInformation("Criando/atualizando review para livro ID: {BookId}", bookId);

            var response = await _httpClient.PostAsJsonAsync($"api/books/{bookId}/review", createReviewDto);

            if (response.IsSuccessStatusCode)
            {
                var review = await response.Content.ReadFromJsonAsync<ReviewDto>();
                _logger.LogInformation("Review criada/atualizada com sucesso. ID: {ReviewId}", review?.Id);
                return review;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Erro ao criar/atualizar review. Status: {Status}, Erro: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede ao criar/atualizar review para livro ID: {BookId}", bookId);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar resposta da API ao criar/atualizar review");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar/atualizar review para livro ID: {BookId}", bookId);
            return null;
        }
    }

    /// <summary>
    /// Obtém a avaliação de um livro específico.
    /// </summary>
    /// <param name="bookId">ID do livro.</param>
    /// <returns>A avaliação do livro, ou null se não existir ou em caso de erro.</returns>
    public async Task<ReviewDto?> GetReviewAsync(int bookId)
    {
        try
        {
            _logger.LogInformation("Buscando review do livro ID: {BookId}", bookId);

            var review = await _httpClient.GetFromJsonAsync<ReviewDto>($"api/books/{bookId}/review");

            if (review != null)
            {
                _logger.LogInformation("Review encontrada. ID: {ReviewId}", review.Id);
            }
            else
            {
                _logger.LogInformation("Nenhuma review encontrada para o livro ID: {BookId}", bookId);
            }

            return review;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogInformation("Review não encontrada para o livro ID: {BookId}", bookId);
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede ao buscar review do livro ID: {BookId}", bookId);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar resposta da API ao buscar review");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar review do livro ID: {BookId}", bookId);
            return null;
        }
    }
}
