using System.ComponentModel.DataAnnotations;
using EstanteVirtual.Api.DTOs;

namespace EstanteVirtual.Api.Tests.DTOs;

public class CreateReviewDtoTests
{
    [Fact]
    public void CreateReviewDto_ValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = "Excelente livro!"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void CreateReviewDto_RatingOutOfRange_FailsValidation(int rating)
    {
        // Arrange
        var dto = new CreateReviewDto
        {
            Rating = rating,
            ReviewText = "Texto da avaliação"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateReviewDto.Rating)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void CreateReviewDto_RatingInRange_PassesValidation(int rating)
    {
        // Arrange
        var dto = new CreateReviewDto
        {
            Rating = rating,
            ReviewText = "Texto da avaliação"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void CreateReviewDto_ReviewTextTooLong_FailsValidation()
    {
        // Arrange
        var dto = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = new string('A', 2001) // Excede 2000 caracteres
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateReviewDto.ReviewText)));
    }

    [Fact]
    public void CreateReviewDto_ReviewTextAt2000Chars_PassesValidation()
    {
        // Arrange
        var dto = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = new string('A', 2000) // Exatamente 2000 caracteres
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void CreateReviewDto_NullReviewText_PassesValidation()
    {
        // Arrange
        var dto = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = null
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void CreateReviewDto_EmptyReviewText_PassesValidation()
    {
        // Arrange
        var dto = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = ""
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.Empty(validationResults);
    }

    private static List<ValidationResult> ValidateDto(object dto)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto, null, null);
        Validator.TryValidateObject(dto, validationContext, validationResults, true);
        return validationResults;
    }
}
