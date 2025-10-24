using EstanteVirtual.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstanteVirtual.Data.Data.EntityConfigurations;

/// <summary>
/// Configuração EF Core para a entidade Review usando Fluent API.
/// </summary>
public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        // Configuração da tabela
        builder.ToTable("Reviews");

        // Chave primária
        builder.HasKey(r => r.Id);

        // Propriedade Rating (obrigatória, range 1-5)
        builder.Property(r => r.Rating)
            .IsRequired();

        // Propriedade ReviewText (opcional, máximo 2000 caracteres)
        builder.Property(r => r.ReviewText)
            .HasMaxLength(2000);

        // Propriedade CreatedAt com valor padrão
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()"); // PostgreSQL function

        // Propriedade UpdatedAt com valor padrão
        builder.Property(r => r.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()"); // PostgreSQL function

        // Foreign key para Book (obrigatória)
        builder.Property(r => r.BookId)
            .IsRequired();

        // Relacionamento 1:0..1 com Book
        builder.HasOne(r => r.Book)
            .WithOne(b => b.Review)
            .HasForeignKey<Review>(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade); // Ao deletar Book, deleta Review também

        // Unique constraint: cada livro pode ter no máximo uma review
        builder.HasIndex(r => r.BookId)
            .IsUnique();

        // Índice para melhorar consultas por rating
        builder.HasIndex(r => r.Rating);
    }
}
