# Data Model: Estante Virtual MVP

**Feature**: 001-estante-mvp  
**Phase**: 1 (Design & Contracts)  
**Date**: 2025-10-23

## Overview

Este documento descreve o modelo de dados para o MVP da Estante Virtual, incluindo entidades, relacionamentos, validações e schema de banco de dados PostgreSQL gerenciado via Entity Framework Core migrations.

---

## Entity Relationship Diagram

```
┌─────────────────────────────────┐
│           Book                  │
│─────────────────────────────────│
│ Id (PK)            : int        │
│ Title              : string(200)│ *required
│ Author             : string(100)│ *required
│ CoverImageUrl      : string(500)│ *optional
│ CreatedAt          : DateTime   │
│─────────────────────────────────│
│ Review (1:0..1)                 │
└─────────────────────────────────┘
              │
              │ 1:0..1
              │ (One book can have zero or one review)
              ▼
┌─────────────────────────────────┐
│          Review                 │
│─────────────────────────────────│
│ Id (PK)            : int        │
│ BookId (FK)        : int        │ *required
│ Rating             : int        │ *required (1-5)
│ ReviewText         : string(2000)│ *optional
│ CreatedAt          : DateTime   │
│ UpdatedAt          : DateTime   │
└─────────────────────────────────┘
```

---

## Entities

### Book

**Purpose**: Representa um livro catalogado na estante do usuário.

**Properties**:

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| `Id` | `int` | PK, Identity | Identificador único auto-incrementado |
| `Title` | `string` | Required, MaxLength(200) | Título do livro |
| `Author` | `string` | Required, MaxLength(100) | Nome do autor |
| `CoverImageUrl` | `string?` | Optional, MaxLength(500) | URL pública da imagem da capa |
| `CreatedAt` | `DateTime` | Required, Default(UTC Now) | Data/hora de adição do livro |

**Relationships**:
- `Review` (navigation property): Um livro pode ter **zero ou uma** resenha associada (1:0..1)

**Validation Rules** (enforced at API level):
- `Title`: Não pode ser null ou vazio, máximo 200 caracteres
- `Author`: Não pode ser null ou vazio, máximo 100 caracteres
- `CoverImageUrl`: Opcional, se fornecido deve ser URL válida, máximo 500 caracteres

**Business Rules**:
- Livros duplicados (mesmo título/autor) são permitidos (usuário pode ter múltiplas edições ou releituras)
- Não há funcionalidade de delete no MVP - livros são permanentes
- Criação requer apenas Title e Author; CoverImageUrl é opcional

**C# Entity (POCO)**:

```csharp
public class Book
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Author { get; set; } = string.Empty;
    
    public string? CoverImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public Review? Review { get; set; }
}
```

---

### Review

**Purpose**: Representa a avaliação (nota e resenha) de um livro pelo usuário.

**Properties**:

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| `Id` | `int` | PK, Identity | Identificador único auto-incrementado |
| `BookId` | `int` | FK, Required, Unique | Referência ao livro avaliado (um livro = uma review) |
| `Rating` | `int` | Required, Range(1-5) | Nota de 1 a 5 estrelas |
| `ReviewText` | `string?` | Optional, MaxLength(2000) | Texto da resenha |
| `CreatedAt` | `DateTime` | Required, Default(UTC Now) | Data/hora de criação da avaliação |
| `UpdatedAt` | `DateTime` | Required, Default(UTC Now) | Data/hora da última edição |

**Relationships**:
- `Book` (navigation property): Cada review pertence a **exatamente um** livro (1:1)

**Validation Rules** (enforced at API level):
- `BookId`: Deve existir na tabela Books (foreign key constraint)
- `Rating`: Obrigatório, inteiro entre 1 e 5 inclusive
- `ReviewText`: Opcional, se fornecido máximo 2000 caracteres

**Business Rules**:
- Cada livro pode ter no máximo UMA review (constraint de uniqueness no BookId)
- Review pode ser editada (UpdatedAt rastreia última modificação)
- Rating é obrigatório, ReviewText é opcional (usuário pode dar nota sem escrever)
- Não há funcionalidade de delete no MVP - reviews são permanentes (podem ser editadas)

**C# Entity (POCO)**:

```csharp
public class Review
{
    public int Id { get; set; }
    
    public int BookId { get; set; }
    
    public int Rating { get; set; }
    
    public string? ReviewText { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property
    public Book Book { get; set; } = null!;
}
```

---

## Entity Framework Core Configuration

### AppDbContext

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());
    }
}
```

### BookConfiguration (Fluent API)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Id)
            .UseIdentityColumn();
        
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(b => b.CoverImageUrl)
            .HasMaxLength(500);
        
        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // One-to-zero-or-one relationship
        builder.HasOne(b => b.Review)
            .WithOne(r => r.Book)
            .HasForeignKey<Review>(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### ReviewConfiguration (Fluent API)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .UseIdentityColumn();
        
        builder.Property(r => r.BookId)
            .IsRequired();
        
        // Unique constraint: one review per book
        builder.HasIndex(r => r.BookId)
            .IsUnique();
        
        builder.Property(r => r.Rating)
            .IsRequired();
        
        builder.Property(r => r.ReviewText)
            .HasMaxLength(2000);
        
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(r => r.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
```

---

## PostgreSQL Schema (Generated by EF Migration)

```sql
-- Books table
CREATE TABLE "Books" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(200) NOT NULL,
    "Author" VARCHAR(100) NOT NULL,
    "CoverImageUrl" VARCHAR(500),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Reviews table
CREATE TABLE "Reviews" (
    "Id" SERIAL PRIMARY KEY,
    "BookId" INT NOT NULL,
    "Rating" INT NOT NULL,
    "ReviewText" VARCHAR(2000),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT "FK_Reviews_Books_BookId" 
        FOREIGN KEY ("BookId") 
        REFERENCES "Books" ("Id") 
        ON DELETE CASCADE,
    
    CONSTRAINT "UQ_Reviews_BookId" 
        UNIQUE ("BookId")
);

-- Index for foreign key (auto-created by EF Core)
CREATE INDEX "IX_Reviews_BookId" ON "Reviews" ("BookId");

-- Check constraint for Rating (enforced at application level)
-- ALTER TABLE "Reviews" ADD CONSTRAINT "CK_Reviews_Rating" CHECK ("Rating" >= 1 AND "Rating" <= 5);
```

**Notes**:
- `SERIAL` = auto-increment integer (PostgreSQL equivalent to SQL Server `IDENTITY`)
- `CURRENT_TIMESTAMP` = UTC timestamp padrão
- `ON DELETE CASCADE` = se livro deletado, review associada também é deletada (não usado no MVP mas importante para futuro)
- Unique constraint em `BookId` garante 1:1 relationship Book-Review

---

## Data Transfer Objects (DTOs)

### API Input/Output DTOs

```csharp
// Output DTO - retornado pela API
public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public ReviewDto? Review { get; set; }
}

// Input DTO - enviado pelo cliente para criar livro
public class CreateBookDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;
    
    [MaxLength(500)]
    [Url] // Validates URL format
    public string? CoverImageUrl { get; set; }
}

// Output DTO - review dentro de BookDto
public class ReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Input DTO - enviado pelo cliente para criar/atualizar review
public class CreateReviewDto
{
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [MaxLength(2000)]
    public string? ReviewText { get; set; }
}
```

---

## Sample Data

### Example Book Records

```json
[
  {
    "Id": 1,
    "Title": "1984",
    "Author": "George Orwell",
    "CoverImageUrl": "https://covers.openlibrary.org/b/id/7222246-L.jpg",
    "CreatedAt": "2025-10-23T10:00:00Z"
  },
  {
    "Id": 2,
    "Title": "O Senhor dos Anéis",
    "Author": "J.R.R. Tolkien",
    "CoverImageUrl": null,
    "CreatedAt": "2025-10-23T11:30:00Z"
  }
]
```

### Example Review Records

```json
[
  {
    "Id": 1,
    "BookId": 1,
    "Rating": 5,
    "ReviewText": "Obra-prima distópica que permanece relevante até hoje. A vigilância do Big Brother é assustadoramente profética.",
    "CreatedAt": "2025-10-23T15:00:00Z",
    "UpdatedAt": "2025-10-23T15:00:00Z"
  },
  {
    "Id": 2,
    "BookId": 2,
    "Rating": 4,
    "ReviewText": null,
    "CreatedAt": "2025-10-23T16:00:00Z",
    "UpdatedAt": "2025-10-23T16:00:00Z"
  }
]
```

---

## Migration Commands

```bash
# Create initial migration
dotnet ef migrations add InitialCreate --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api

# Apply migration to database
dotnet ef database update --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api

# Remove last migration (if needed)
dotnet ef migrations remove --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api

# Generate SQL script (for production deployment)
dotnet ef migrations script --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api
```

---

## Summary

✅ **2 Entities**: Book, Review  
✅ **1:0..1 Relationship**: Um livro pode ter zero ou uma review  
✅ **Validation**: Constraints de tamanho, obrigatoriedade e range  
✅ **EF Core**: Fluent API configuration, migrations ready  
✅ **PostgreSQL**: Schema compatível, auto-increment, timestamps  
✅ **DTOs**: Input/output separation, validation attributes  

Este modelo está alinhado com os requisitos funcionais FR-001 a FR-016 da especificação e segue os princípios constitucionais de simplicidade (YAGNI) e persistência (EF Core + PostgreSQL).
