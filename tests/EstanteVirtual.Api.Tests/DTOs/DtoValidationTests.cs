using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EstanteVirtual.Api.DTOs;
using Xunit;

namespace EstanteVirtual.Api.Tests.DTOs
{
    public class DtoValidationTests
    {
        [Theory]
        [InlineData("Livro Válido", "Autor Válido", null, true)]
        [InlineData("Livro Válido", "Autor Válido", "http://example.com/cover.jpg", true)]
        [InlineData("", "Autor Válido", null, false)] // Título vazio
        [InlineData(" ", "Autor Válido", null, false)] // Título com espaço
        [InlineData(null, "Autor Válido", null, false)] // Título nulo
        [InlineData("Livro Válido", "", null, false)] // Autor vazio
        [InlineData("Livro Válido", " ", null, false)] // Autor com espaço
        [InlineData("Livro Válido", null, null, false)] // Autor nulo
        [InlineData("a", "a", null, true)] // Mínimo
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "a", null, false)] // Título muito longo (201+)
        [InlineData("a", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", null, false)] // Autor muito longo (101+)
        public void CreateBookDto_Validation(string title, string author, string coverImageUrl, bool expected)
        {
            // Arrange
            var dto = new CreateBookDto
            {
                Title = title,
                Author = author,
                CoverImageUrl = coverImageUrl
            };

            // Act
            var context = new ValidationContext(dto, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(1, "Ótimo livro!", true)]
        [InlineData(5, "Leitura recomendada.", true)]
        [InlineData(3, null, true)] // ReviewText é opcional
        [InlineData(3, "", true)] // ReviewText pode ser vazio
        [InlineData(0, "Rating abaixo", false)] // Rating < 1
        [InlineData(6, "Rating acima", false)] // Rating > 5
        [InlineData(3, "a", true)]
        public void CreateReviewDto_Validation(int rating, string reviewText, bool expected)
        {
            // Arrange
            var dto = new CreateReviewDto
            {
                Rating = rating,
                ReviewText = reviewText
            };

            // Act
            var validationContext = new ValidationContext(dto, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Assert
            Assert.Equal(expected, isValid);
        }

        [Fact]
        public void CreateReviewDto_ReviewText_ExceedsMax_ShouldBeInvalid()
        {
            // Arrange
            var tooLong = new string('a', 2001);
            var dto = new CreateReviewDto { Rating = 3, ReviewText = tooLong };

            // Act
            var ctx = new ValidationContext(dto, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, ctx, results, true);

            // Assert
            Assert.False(isValid);
        }
    }
}
