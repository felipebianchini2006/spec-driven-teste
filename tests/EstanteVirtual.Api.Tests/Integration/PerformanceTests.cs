using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EstanteVirtual.Api.DTOs;
using EstanteVirtual.Api.Tests.Helpers;
using EstanteVirtual.Data.Data;
using EstanteVirtual.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace EstanteVirtual.Api.Tests.Integration
{
    public class PerformanceTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public PerformanceTests(TestWebApplicationFactory factory, ITestOutputHelper output)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task GetAllBooks_With_100_Books_Should_Load_Under_2_Seconds()
        {
            // Arrange
            const int bookCount = 100;
            const int maxLoadTimeMs = 2000;

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Garante que o banco de dados está limpo antes de popular
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var books = new List<Book>();
                for (int i = 0; i < bookCount; i++)
                {
                    books.Add(new Book { Title = $"Livro de Teste {i}", Author = $"Autor {i}" });
                }
                await context.Books.AddRangeAsync(books);
                await context.SaveChangesAsync();
            }

            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            var response = await _client.GetAsync("/api/books");
            stopwatch.Stop();

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedBooks = await response.Content.ReadFromJsonAsync<List<BookDto>>();

            Assert.NotNull(returnedBooks);
            Assert.Equal(bookCount, returnedBooks.Count);

            _output.WriteLine($"Tempo de carregamento para {bookCount} livros: {stopwatch.ElapsedMilliseconds} ms");
            Assert.True(stopwatch.ElapsedMilliseconds < maxLoadTimeMs, $"A requisição demorou {stopwatch.ElapsedMilliseconds} ms, o que é mais do que o limite de {maxLoadTimeMs} ms.");
        }
    }
}
