# Quickstart Guide: Estante Virtual MVP

**Feature**: 001-estante-mvp  
**Last Updated**: 2025-10-23  
**Target Audience**: Desenvolvedores configurando ambiente local

---

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- **.NET 8.0 SDK** ou superior
  - Download: https://dotnet.microsoft.com/download/dotnet/8.0
  - Verifique: `dotnet --version` (deve mostrar 8.0.x)

- **PostgreSQL 14+**
  - Download: https://www.postgresql.org/download/
  - Ou use Docker: `docker run --name postgres-estante -e POSTGRES_PASSWORD=devpass -p 5432:5432 -d postgres:14`
  - Verifique: `psql --version`

- **IDE/Editor**:
  - Visual Studio 2022 (recomendado para Windows)
  - Visual Studio Code com extensões C# + .NET
  - JetBrains Rider

- **Git** (para clonar repositório)

---

## 🚀 Setup Rápido (5 minutos)

### 1. Clonar o Repositório e Checkout da Branch

```bash
git clone <repository-url>
cd spec-driven-teste
git checkout 001-estante-mvp
```

### 2. Criar a Solução e Projetos

```bash
# Criar solução na raiz
dotnet new sln -n EstanteVirtual

# Criar projeto Data (biblioteca de classes)
dotnet new classlib -n EstanteVirtual.Data -o src/EstanteVirtual.Data -f net8.0
dotnet sln add src/EstanteVirtual.Data/EstanteVirtual.Data.csproj

# Criar projeto API (Web API)
dotnet new webapi -n EstanteVirtual.Api -o src/EstanteVirtual.Api -f net8.0
dotnet sln add src/EstanteVirtual.Api/EstanteVirtual.Api.csproj

# Criar projeto Web (Blazor Server)
dotnet new blazorserver -n EstanteVirtual.Web -o src/EstanteVirtual.Web -f net8.0
dotnet sln add src/EstanteVirtual.Web/EstanteVirtual.Web.csproj

# Criar projetos de testes
dotnet new xunit -n EstanteVirtual.Api.Tests -o tests/EstanteVirtual.Api.Tests -f net8.0
dotnet sln add tests/EstanteVirtual.Api.Tests/EstanteVirtual.Api.Tests.csproj

dotnet new xunit -n EstanteVirtual.Data.Tests -o tests/EstanteVirtual.Data.Tests -f net8.0
dotnet sln add tests/EstanteVirtual.Data.Tests/EstanteVirtual.Data.Tests.csproj
```

### 3. Adicionar Dependências NuGet

```bash
# EstanteVirtual.Data - EF Core + PostgreSQL
cd src/EstanteVirtual.Data
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
cd ../..

# EstanteVirtual.Api - Referência ao Data + Swagger
cd src/EstanteVirtual.Api
dotnet add reference ../EstanteVirtual.Data/EstanteVirtual.Data.csproj
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
cd ../..

# EstanteVirtual.Web - HttpClient (já incluído no template Blazor)
cd src/EstanteVirtual.Web
# Nenhuma dependência extra necessária
cd ../..

# EstanteVirtual.Api.Tests - WebApplicationFactory + Moq
cd tests/EstanteVirtual.Api.Tests
dotnet add reference ../../src/EstanteVirtual.Api/EstanteVirtual.Api.csproj
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.0
dotnet add package Moq --version 4.20.70
cd ../..

# EstanteVirtual.Data.Tests - Referência ao Data
cd tests/EstanteVirtual.Data.Tests
dotnet add reference ../../src/EstanteVirtual.Data/EstanteVirtual.Data.csproj
cd ../..
```

### 4. Configurar String de Conexão PostgreSQL

```bash
# Opção A: Usar User Secrets (desenvolvimento - RECOMENDADO)
cd src/EstanteVirtual.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=estantevirtual;Username=postgres;Password=devpass"
cd ../..

# Opção B: Editar appsettings.Development.json (NÃO COMMITAR SENHA)
# src/EstanteVirtual.Api/appsettings.Development.json:
# {
#   "ConnectionStrings": {
#     "DefaultConnection": "Host=localhost;Port=5432;Database=estantevirtual;Username=postgres;Password=devpass"
#   }
# }
```

### 5. Criar Banco de Dados e Aplicar Migrations

```bash
# Criar database no PostgreSQL
psql -U postgres -c "CREATE DATABASE estantevirtual;"

# Criar migration inicial (após criar modelos Book e Review)
dotnet ef migrations add InitialCreate \
  --project src/EstanteVirtual.Data \
  --startup-project src/EstanteVirtual.Api

# Aplicar migration ao banco
dotnet ef database update \
  --project src/EstanteVirtual.Data \
  --startup-project src/EstanteVirtual.Api
```

### 6. Executar API e Verificar Swagger

```bash
cd src/EstanteVirtual.Api
dotnet run

# Abrir navegador em: https://localhost:7001/swagger
# Testar endpoints:
# - GET /api/books (deve retornar array vazio inicialmente)
# - POST /api/books com body { "title": "1984", "author": "George Orwell" }
```

### 7. Executar Frontend Blazor

```bash
# Em outro terminal
cd src/EstanteVirtual.Web
dotnet run

# Abrir navegador em: https://localhost:7002
# Deve ver página inicial do Blazor
```

---

## 🏗️ Estrutura do Projeto (Após Setup)

```
EstanteVirtual/
├── EstanteVirtual.sln
├── src/
│   ├── EstanteVirtual.Data/          # Modelos EF Core + DbContext
│   │   ├── Models/
│   │   │   ├── Book.cs
│   │   │   └── Review.cs
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   └── EntityConfigurations/
│   │   │       ├── BookConfiguration.cs
│   │   │       └── ReviewConfiguration.cs
│   │   └── Migrations/               # Auto-gerado por EF
│   │
│   ├── EstanteVirtual.Api/           # Backend Web API
│   │   ├── Controllers/
│   │   │   ├── BooksController.cs
│   │   │   └── ReviewsController.cs
│   │   ├── DTOs/
│   │   │   ├── BookDto.cs
│   │   │   ├── CreateBookDto.cs
│   │   │   ├── ReviewDto.cs
│   │   │   └── CreateReviewDto.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   └── EstanteVirtual.Web/           # Frontend Blazor Server
│       ├── Pages/
│       │   ├── Index.razor           # Galeria de livros
│       │   └── BookDetails.razor     # Detalhes + avaliação
│       ├── Components/
│       │   ├── AddBookForm.razor
│       │   └── BookCard.razor
│       ├── Services/
│       │   └── BookApiService.cs
│       └── Program.cs
│
└── tests/
    ├── EstanteVirtual.Api.Tests/     # Testes de integração API
    │   ├── BooksControllerTests.cs
    │   └── ReviewsControllerTests.cs
    └── EstanteVirtual.Data.Tests/    # Testes unitários modelos
        └── Models/
            ├── BookTests.cs
            └── ReviewTests.cs
```

---

## 🧪 Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar apenas testes de API
dotnet test tests/EstanteVirtual.Api.Tests

# Executar com verbosidade detalhada
dotnet test --logger "console;verbosity=detailed"

# Executar com cobertura de código (se configurado)
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## 📝 Workflow de Desenvolvimento (TDD)

### Exemplo: Adicionar endpoint GET /api/books

#### 1. **RED**: Escrever teste que falha

```csharp
// tests/EstanteVirtual.Api.Tests/BooksControllerTests.cs
[Fact]
public async Task GetAllBooks_ReturnsEmptyList_WhenNoBooksExist()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/api/books");
    
    // Assert
    response.EnsureSuccessStatusCode();
    var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
    Assert.NotNull(books);
    Assert.Empty(books);
}
```

```bash
dotnet test  # DEVE FALHAR (endpoint não existe ainda)
```

#### 2. **GREEN**: Implementar código mínimo

```csharp
// src/EstanteVirtual.Api/Controllers/BooksController.cs
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;
    
    [HttpGet]
    public async Task<ActionResult<List<BookDto>>> GetAllBooks()
    {
        var books = await _context.Books
            .Include(b => b.Review)
            .Select(b => new BookDto { /* ... */ })
            .ToListAsync();
        return Ok(books);
    }
}
```

```bash
dotnet test  # DEVE PASSAR
```

#### 3. **REFACTOR**: Melhorar código mantendo testes verdes

```csharp
// Extrair lógica para método helper, adicionar logging, etc.
// Executar testes novamente para garantir que não quebrou
dotnet test
```

---

## 🔧 Comandos Úteis

### EF Core Migrations

```bash
# Adicionar nova migration
dotnet ef migrations add <MigrationName> \
  --project src/EstanteVirtual.Data \
  --startup-project src/EstanteVirtual.Api

# Aplicar migrations
dotnet ef database update \
  --project src/EstanteVirtual.Data \
  --startup-project src/EstanteVirtual.Api

# Reverter última migration
dotnet ef database update <PreviousMigration> \
  --project src/EstanteVirtual.Data \
  --startup-project src/EstanteVirtual.Api

# Remover última migration (se não aplicada)
dotnet ef migrations remove \
  --project src/EstanteVirtual.Data \
  --startup-project src/EstanteVirtual.Api

# Gerar SQL script
dotnet ef migrations script \
  --project src/EstanteVirtual.Data \
  --startup-project src/EstanteVirtual.Api \
  --output migration.sql
```

### Build e Run

```bash
# Build toda solução
dotnet build

# Restaurar dependências
dotnet restore

# Limpar build
dotnet clean

# Run API com hot reload
cd src/EstanteVirtual.Api
dotnet watch run

# Run Blazor com hot reload
cd src/EstanteVirtual.Web
dotnet watch run
```

### Docker (Alternativa para PostgreSQL)

```bash
# Iniciar PostgreSQL em container
docker run --name postgres-estante \
  -e POSTGRES_PASSWORD=devpass \
  -e POSTGRES_DB=estantevirtual \
  -p 5432:5432 \
  -d postgres:14

# Parar container
docker stop postgres-estante

# Reiniciar container
docker start postgres-estante

# Remover container
docker rm -f postgres-estante
```

---

## 🐛 Troubleshooting

### Erro: "Connection refused" ao acessar banco

**Solução**: Verificar se PostgreSQL está rodando
```bash
# Linux/Mac
sudo systemctl status postgresql

# Windows (Services)
services.msc -> PostgreSQL service

# Docker
docker ps | grep postgres
```

### Erro: "A network-related or instance-specific error occurred"

**Solução**: Verificar connection string e credenciais
```bash
# Testar conexão manual
psql -h localhost -p 5432 -U postgres -d estantevirtual

# Verificar user secrets
dotnet user-secrets list --project src/EstanteVirtual.Api
```

### Erro: "The type or namespace name 'Book' could not be found"

**Solução**: Verificar referências entre projetos
```bash
# API deve referenciar Data
dotnet list src/EstanteVirtual.Api/EstanteVirtual.Api.csproj reference

# Adicionar referência se faltando
dotnet add src/EstanteVirtual.Api/EstanteVirtual.Api.csproj reference src/EstanteVirtual.Data/EstanteVirtual.Data.csproj
```

### Testes falhando: "Database does not exist"

**Solução**: Usar banco in-memory para testes ou criar test database
```csharp
// tests/EstanteVirtual.Api.Tests/Helpers/TestWebApplicationFactory.cs
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.ConfigureServices(services =>
    {
        // Substituir DbContext real por in-memory
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
    });
}
```

---

## 📚 Próximos Passos

Após setup completo:

1. ✅ Implementar modelos `Book` e `Review` (TDD primeiro - testes de validação)
2. ✅ Configurar `AppDbContext` e entity configurations
3. ✅ Criar migration inicial e aplicar ao banco
4. ✅ Implementar `BooksController` (TDD - um endpoint por vez)
5. ✅ Implementar `ReviewsController` (TDD)
6. ✅ Criar componentes Blazor (`AddBookForm`, `BookCard`)
7. ✅ Implementar `BookApiService` no Blazor
8. ✅ Criar páginas Blazor (`Index.razor`, `BookDetails.razor`)
9. ✅ Testar fluxo completo end-to-end
10. ✅ Verificar Constitution Check (todos os gates devem passar)

---

## 📖 Referências

- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [xUnit Testing](https://xunit.net/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [OpenAPI Specification](https://swagger.io/specification/)

---

## ✅ Checklist de Setup Completo

- [ ] .NET 8 SDK instalado e verificado
- [ ] PostgreSQL rodando (local ou Docker)
- [ ] Solução criada com 3 projetos (Data, Api, Web)
- [ ] Projetos de teste criados (Api.Tests, Data.Tests)
- [ ] Dependências NuGet instaladas
- [ ] Connection string configurada (User Secrets)
- [ ] Database criada no PostgreSQL
- [ ] Migration inicial aplicada
- [ ] API rodando e Swagger acessível
- [ ] Blazor rodando
- [ ] Testes executando (mesmo que vazios inicialmente)
- [ ] Constitution principles revisados

**Quando todos checados**: Pronto para começar desenvolvimento TDD! 🎉
