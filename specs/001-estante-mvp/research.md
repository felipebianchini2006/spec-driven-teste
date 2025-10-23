# Research: Estante Virtual MVP

**Feature**: 001-estante-mvp  
**Phase**: 0 (Outline & Research)  
**Date**: 2025-10-23

## Overview

Este documento consolida as decisões técnicas e pesquisa de melhores práticas para implementação do MVP da Estante Virtual usando .NET 8, ASP.NET Core Web API, Blazor Server, Entity Framework Core e PostgreSQL.

---

## Research Tasks

### 1. Multi-Project Solution Structure (.NET)

**Task**: "Research best practices for organizing multi-project .NET solutions with separated API, Data layer, and Blazor frontend"

**Decision**: Estrutura de 3 projetos (EstanteVirtual.Data, EstanteVirtual.Api, EstanteVirtual.Web)

**Rationale**:
- **Separation of Concerns**: Cada projeto tem responsabilidade única e bem definida
- **Data Layer Isolation**: EstanteVirtual.Data contém apenas modelos EF Core e DbContext, pode ser compartilhado se necessário
- **API Independence**: EstanteVirtual.Api expõe contratos HTTP, desacoplado do frontend
- **Frontend Flexibility**: EstanteVirtual.Web consome API via HTTP, permite migração futura para Blazor WebAssembly
- **Testability**: Cada camada pode ser testada independentemente

**Alternatives Considered**:
- **Single Project**: Rejeitado - viola princípio de separação API/Frontend da constituição
- **Two Projects (API + Blazor)**: Rejeitado - mistura entidades com API, dificulta reuso do modelo
- **Domain + Infrastructure Layers**: Rejeitado - complexidade desnecessária para MVP de 2 entidades (YAGNI)

**References**:
- Microsoft Docs: [ASP.NET Core project structure](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/)
- Clean Architecture patterns adapted for .NET simplicity

---

### 2. Entity Framework Core Configuration

**Task**: "Research EF Core best practices for entity configuration and migrations with PostgreSQL"

**Decision**: Usar Fluent API em arquivos de configuração separados + EF Migrations

**Rationale**:
- **Fluent API**: Configuração explícita e type-safe (vs Data Annotations)
- **Separate Configuration Files**: BookConfiguration.cs e ReviewConfiguration.cs mantêm modelos limpos (POCOs)
- **Migrations**: Controle de versão do schema, permite rollback, trabalha bem em equipe
- **PostgreSQL Provider**: Npgsql.EntityFrameworkCore.PostgreSQL é o provider oficial e maduro

**Implementation Details**:
```csharp
// BookConfiguration.cs example
public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(100);
        builder.Property(b => b.CoverImageUrl).HasMaxLength(500);
        builder.HasOne(b => b.Review).WithOne(r => r.Book).HasForeignKey<Review>(r => r.BookId);
    }
}
```

**Alternatives Considered**:
- **Data Annotations**: Rejeitado - polui modelos, menos flexível que Fluent API
- **DbContext.OnModelCreating**: Rejeitado - arquivo único grande, difícil manutenção
- **Code-First vs Database-First**: Code-First escolhido para controle de versão e YAGNI

**References**:
- [EF Core Fluent API](https://learn.microsoft.com/en-us/ef/core/modeling/)
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

---

### 3. RESTful API Design for Books & Reviews

**Task**: "Research RESTful API conventions and endpoint design for nested resources (books with reviews)"

**Decision**: Resource-based endpoints com relacionamento parent/child

**Rationale**:
- **RESTful Conventions**: Usar HTTP verbs apropriados (GET, POST, PUT), plural nouns, status codes corretos
- **Nested Resource Pattern**: Reviews são sub-recursos de Books (`/api/books/{id}/review`)
- **DTOs**: Usar DTOs separados para input (CreateBookDto) e output (BookDto) para controle de API contract
- **Stateless**: API não mantém sessão, cada request é independente

**Endpoint Design**:
```
GET    /api/books              → List all books (BookDto[])
POST   /api/books              → Create new book (CreateBookDto) → 201 Created
GET    /api/books/{id}         → Get book details (BookDto with ReviewDto if exists)
POST   /api/books/{id}/review  → Add/Update review (CreateReviewDto) → 200 OK or 201 Created
```

**Status Codes**:
- 200 OK: Successful GET, PUT
- 201 Created: Successful POST with new resource
- 400 Bad Request: Validation errors
- 404 Not Found: Resource doesn't exist
- 500 Internal Server Error: Unexpected errors

**Alternatives Considered**:
- **Separate /api/reviews**: Rejeitado - reviews não fazem sentido fora do contexto de um livro
- **PUT vs POST for review**: POST escolhido pois suporta create + update (idempotência não crítica para MVP)
- **GraphQL**: Rejeitado - overhead desnecessário para MVP simples

**References**:
- [REST API Best Practices](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [ASP.NET Core Web API conventions](https://learn.microsoft.com/en-us/aspnet/core/web-api/)

---

### 4. Blazor Server HttpClient Configuration

**Task**: "Research best practices for consuming REST APIs in Blazor Server using HttpClient"

**Decision**: IHttpClientFactory com serviço wrapper (BookApiService)

**Rationale**:
- **IHttpClientFactory**: Gerencia lifecycle de HttpClient, evita socket exhaustion, permite configuração centralizada
- **Service Wrapper**: BookApiService encapsula lógica de chamadas HTTP, facilita mock em testes, centraliza tratamento de erros
- **Typed Client**: Configurar HttpClient como typed client com base URL da API
- **JSON Serialization**: System.Text.Json (padrão .NET) para serializar/deserializar DTOs

**Implementation Pattern**:
```csharp
// Program.cs configuration
builder.Services.AddHttpClient<BookApiService>(client => 
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});

// BookApiService.cs
public class BookApiService
{
    private readonly HttpClient _httpClient;
    
    public async Task<List<BookDto>> GetAllBooksAsync() { ... }
    public async Task<BookDto> GetBookByIdAsync(int id) { ... }
    public async Task<BookDto> CreateBookAsync(CreateBookDto dto) { ... }
}
```

**Alternatives Considered**:
- **Direct HttpClient injection**: Rejeitado - viola Single Responsibility, dificulta testes
- **Refit**: Rejeitado - dependência extra desnecessária para MVP simples
- **SignalR**: Rejeitado - real-time não é requisito, REST suficiente

**References**:
- [IHttpClientFactory in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests)
- [Blazor Server HTTP requests](https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api)

---

### 5. Testing Strategy (TDD with xUnit + WebApplicationFactory)

**Task**: "Research TDD best practices for .NET API testing with xUnit and WebApplicationFactory"

**Decision**: xUnit para unit tests + WebApplicationFactory para integration tests da API

**Rationale**:
- **xUnit**: Framework de testes padrão .NET, suporte nativo, extensível
- **WebApplicationFactory**: In-memory test server, testa toda stack HTTP sem deploy
- **Test Database**: Usar PostgreSQL em container (Testcontainers.PostgreSQL) ou SQLite in-memory para testes
- **TDD Workflow**: Red (write failing test) → Green (implement) → Refactor

**Test Structure**:
```csharp
// BooksControllerTests.cs (Integration)
public class BooksControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAllBooks_ReturnsEmptyList_WhenNoBooksExist() { ... }
    
    [Fact]
    public async Task CreateBook_Returns201Created_WhenValidBook() { ... }
}

// BookTests.cs (Unit)
public class BookTests
{
    [Fact]
    public void Book_Title_CannotExceed200Characters() { ... }
}
```

**Alternatives Considered**:
- **MSTest**: Rejeitado - xUnit tem melhor suporte comunidade e features modernas
- **NUnit**: Rejeitado - xUnit é padrão Microsoft atual
- **Manual API testing only**: Rejeitado - viola princípio TDD da constituição

**References**:
- [xUnit documentation](https://xunit.net/)
- [Integration tests with WebApplicationFactory](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)

---

### 6. Database Connection String Management

**Task**: "Research secure configuration management for PostgreSQL connection strings in .NET"

**Decision**: appsettings.json + User Secrets (development) + Environment Variables (production)

**Rationale**:
- **appsettings.json**: Configuração base (não contém secrets em produção)
- **User Secrets**: Desenvolvimento local, não commitado no Git
- **Environment Variables**: Produção/CI/CD, injetadas pelo host (Docker, Azure, etc.)
- **Configuration Binding**: Usar IConfiguration do .NET para acesso type-safe

**Implementation**:
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=estantevirtual;Username=postgres;Password=<from-secrets>"
  }
}

// User secrets (development)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=estantevirtual;Username=dev;Password=devpass"
```

**Alternatives Considered**:
- **Hardcoded strings**: Rejeitado - risco de segurança
- **Azure Key Vault**: Rejeitado - overhead para MVP, pode ser adicionado depois
- **.env files**: Rejeitado - não é padrão .NET, User Secrets é nativo

**References**:
- [Safe storage of app secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)

---

## Summary of Key Decisions

| Área | Decisão | Justificativa |
|------|---------|---------------|
| **Arquitetura** | 3-project solution | Separação clara, testabilidade, alinhado com constituição |
| **ORM** | EF Core + Fluent API | Type-safety, migrations, configuração limpa |
| **API** | RESTful com DTOs | Padrão da indústria, desacoplamento, versionamento |
| **Frontend** | Blazor Server + HttpClient | Simplicidade MVP, preparado para WebAssembly futuro |
| **Testes** | xUnit + WebApplicationFactory | TDD compliant, testes rápidos e confiáveis |
| **Configuração** | appsettings + User Secrets | Seguro, padrão .NET, flexível por ambiente |

---

## Next Steps

Com este research completo, estamos prontos para:

1. **Phase 1**: Gerar data-model.md detalhando entidades Book e Review
2. **Phase 1**: Criar contracts/ com especificação OpenAPI dos 4 endpoints
3. **Phase 1**: Gerar quickstart.md com instruções de setup do ambiente
4. **Phase 2**: Converter tudo isso em tasks executáveis (via /speckit.tasks)

Todos os "NEEDS CLARIFICATION" do Technical Context foram resolvidos. ✅
