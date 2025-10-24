# 📚 Estante Virtual MVP

Sistema completo de gerenciamento de biblioteca pessoal com avaliações, desenvolvido com arquitetura limpa e metodologia TDD (Test-Driven Development).

## 🎯 Funcionalidades

### ✅ US1 - Adicionar Livro
- Cadastro de livros com título, autor e capa opcional
- Validação de campos obrigatórios
- Interface intuitiva com feedback visual

### ✅ US2 - Galeria de Livros
- Visualização em grid responsivo
- Imagens de capa com placeholder automático
- Indicador visual de livros avaliados

### ✅ US3 - Avaliar Livro com Nota e Resenha
- Sistema de avaliação com estrelas (1-5)
- Resenha opcional de até 2000 caracteres
- Funcionalidade de criar e editar avaliações
- Relacionamento 1:1 entre livro e avaliação

## 🏗️ Arquitetura

### Estrutura de Projetos

```
EstanteVirtual/
├── src/
│   ├── EstanteVirtual.Data/          # Camada de Dados
│   │   ├── Models/                   # Entidades (Book, Review)
│   │   ├── Data/                     # DbContext e Configurações
│   │   └── Migrations/               # Migrations EF Core
│   ├── EstanteVirtual.Api/           # API REST
│   │   ├── Controllers/              # Endpoints REST
│   │   └── DTOs/                     # Data Transfer Objects
│   └── EstanteVirtual.Web/           # Interface Blazor
│       ├── Components/               # Componentes Razor
│       ├── Services/                 # Serviços HTTP
│       └── DTOs/                     # DTOs do Frontend
└── tests/
    ├── EstanteVirtual.Data.Tests/    # Testes Unitários
    └── EstanteVirtual.Api.Tests/     # Testes de Integração
```

### Stack Tecnológica

- **.NET 8.0** - Framework principal
- **ASP.NET Core 8.0** - API REST
- **Blazor Server** - Interface do usuário
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL** - Banco de dados
- **xUnit** - Framework de testes
- **Moq** - Mocking para testes

## 🚀 Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- PostgreSQL 12+
- Visual Studio Code ou Visual Studio 2022

### Configuração do Banco de Dados

1. Crie o banco de dados PostgreSQL:
```sql
CREATE DATABASE estantevirtual;
```

2. Configure a connection string em `src/EstanteVirtual.Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=estantevirtual;Username=seu_usuario;Password=sua_senha"
  }
}
```

3. Execute as migrations:
```bash
cd src/EstanteVirtual.Api
dotnet ef database update
```

### Executando a Aplicação

1. **Inicie a API** (Terminal 1):
```bash
dotnet run --project src/EstanteVirtual.Api/EstanteVirtual.Api.csproj
```
API disponível em: http://localhost:5009  
Swagger UI: http://localhost:5009/swagger

2. **Inicie o Blazor** (Terminal 2):
```bash
dotnet run --project src/EstanteVirtual.Web/EstanteVirtual.Web.csproj
```
Aplicação disponível em: http://localhost:5248

### Executando os Testes

```bash
# Todos os testes
dotnet test EstanteVirtual.sln

# Apenas testes unitários
dotnet test tests/EstanteVirtual.Data.Tests

# Apenas testes de integração
dotnet test tests/EstanteVirtual.Api.Tests
```

## 📊 Cobertura de Testes

- **39 testes automatizados** (100% passando)
  - 13 testes unitários
  - 26 testes de integração
- Cobertura TDD completa para todas as user stories
- Testes de validação, regras de negócio e endpoints

## 🎨 Interface do Usuário

### Página Principal
- Formulário para adicionar novos livros
- Galeria responsiva de livros
- Indicadores visuais de avaliação

### Página de Detalhes
- Informações completas do livro
- Visualização de avaliação existente
- Formulário para adicionar/editar avaliação
- Seletor interativo de estrelas

## 🔗 Endpoints da API

### Books
- `POST /api/books` - Criar livro
- `GET /api/books` - Listar todos os livros
- `GET /api/books/{id}` - Obter livro por ID

### Reviews
- `POST /api/books/{id}/review` - Criar/atualizar avaliação
- `GET /api/books/{id}/review` - Obter avaliação do livro

## 📝 Banco de Dados

### Tabela Books
```sql
- Id (PK)
- Title (VARCHAR 200, NOT NULL)
- Author (VARCHAR 100, NOT NULL)
- CoverImageUrl (VARCHAR 500, NULL)
- CreatedAt (TIMESTAMP, NOT NULL)
```

### Tabela Reviews
```sql
- Id (PK)
- BookId (FK UNIQUE, NOT NULL)
- Rating (INT 1-5, NOT NULL)
- ReviewText (VARCHAR 2000, NULL)
- CreatedAt (TIMESTAMP, NOT NULL)
- UpdatedAt (TIMESTAMP, NOT NULL)
```

### Índices
- `IX_Books_Title` - Busca por título
- `IX_Books_Author` - Busca por autor
- `IX_Reviews_BookId` - Busca por livro (UNIQUE)
- `IX_Reviews_Rating` - Filtro por nota

## 🛠️ Desenvolvimento

### Princípios Seguidos
- **TDD (Test-Driven Development)** - Red-Green-Refactor
- **YAGNI** - You Aren't Gonna Need It
- **Clean Architecture** - Separação clara de responsabilidades
- **RESTful API** - Padrões HTTP corretos

### Padrões de Código
- Validação em múltiplas camadas (DTO, Entity, Database)
- Logging abrangente para debugging
- Tratamento de erros centralizado
- Documentação XML para Swagger

## 📦 Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api

# Aplicar migrations
dotnet ef database update --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api

# Reverter migration
dotnet ef database update NomeMigrationAnterior --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api
```

## 🤝 Contribuindo

Este projeto segue metodologia spec-driven com SpecKit. Para contribuir:

1. Verifique as specs em `specs/001-estante-mvp/`
2. Siga o processo TDD (Red-Green-Refactor)
3. Mantenha cobertura de testes
4. Documente mudanças significativas

## 📄 Licença

Este projeto é open source e está disponível sob a licença MIT.

## 🎯 Status do Projeto

✅ **MVP Completo** - Todas as 3 user stories implementadas  
✅ **39 testes passando** - 100% de sucesso  
✅ **Banco de dados configurado** - PostgreSQL com migrations  
✅ **Aplicações rodando** - API e Blazor funcionais

---

**Desenvolvido com ❤️ seguindo metodologia TDD e princípios de Clean Architecture**
