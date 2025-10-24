using EstanteVirtual.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstanteVirtual.Data.Data.EntityConfigurations;

/// <summary>
/// Configuração EF Core para a entidade Book usando Fluent API.
/// </summary>
public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        // Configuração da tabela
        builder.ToTable("Books");

        // Chave primária
        builder.HasKey(b => b.Id);

        // Propriedade Title
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Propriedade Author
        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);

        // Propriedade CoverImageUrl (opcional)
        builder.Property(b => b.CoverImageUrl)
            .HasMaxLength(500);

        // Propriedade CreatedAt com valor padrão
        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()"); // PostgreSQL function para timestamp atual

        // Relacionamento 1:0..1 com Review
        builder.HasOne(b => b.Review)
            .WithOne(r => r.Book)
            .HasForeignKey<Review>(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade); // Ao deletar Book, deleta Review também

        // Índice para melhorar consultas por título ou autor
        builder.HasIndex(b => b.Title);
        builder.HasIndex(b => b.Author);
    }
}
