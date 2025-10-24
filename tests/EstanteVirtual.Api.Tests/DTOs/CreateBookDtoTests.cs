using System.ComponentModel.DataAnnotations;
using EstanteVirtual.Api.DTOs;

namespace EstanteVirtual.Api.Tests.DTOs;

public class CreateBookDtoTests
{
    [Fact]
    public void CreateBookDto_ValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            CoverImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void CreateBookDto_MissingTitle_FailsValidation()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = null!,
            Author = "Robert C. Martin"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateBookDto.Title)));
    }

    [Fact]
    public void CreateBookDto_EmptyTitle_FailsValidation()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "",
            Author = "Robert C. Martin"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateBookDto.Title)));
    }

    [Fact]
    public void CreateBookDto_TitleTooLong_FailsValidation()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = new string('A', 201), // Excede 200 caracteres
            Author = "Robert C. Martin"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateBookDto.Title)));
    }

    [Fact]
    public void CreateBookDto_MissingAuthor_FailsValidation()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Clean Code",
            Author = null!
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateBookDto.Author)));
    }

    [Fact]
    public void CreateBookDto_EmptyAuthor_FailsValidation()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Clean Code",
            Author = ""
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateBookDto.Author)));
    }

    [Fact]
    public void CreateBookDto_AuthorTooLong_FailsValidation()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Clean Code",
            Author = new string('A', 101) // Excede 100 caracteres
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateBookDto.Author)));
    }

    [Fact]
    public void CreateBookDto_CoverImageUrlTooLong_FailsValidation()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            CoverImageUrl = new string('A', 501) // Excede 500 caracteres
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateBookDto.CoverImageUrl)));
    }

    [Fact]
    public void CreateBookDto_NullCoverImageUrl_PassesValidation()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            CoverImageUrl = null
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
