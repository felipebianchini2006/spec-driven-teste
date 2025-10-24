# Implementation Plan: Estante Virtual MVP

**Branch**: `001-estante-mvp` | **Date**: 2025-10-23 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-estante-mvp/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Implementação de um sistema web de catalogação pessoal de livros com funcionalidades de adicionar livros (título, autor, capa), visualizar galeria e avaliar com notas e resenhas. A arquitetura seguirá o padrão multi-projeto com separação clara entre camada de dados (EstanteVirtual.Data), API backend (EstanteVirtual.Api) e frontend Blazor Server (EstanteVirtual.Web). O frontend consumirá exclusivamente a API via HTTP, sem acesso direto ao banco PostgreSQL. Desenvolvimento orientado por TDD com xUnit para testes unitários e WebApplicationFactory para testes de integração da API.

## Technical Context

**Language/Version**: .NET 8.0 | ASP.NET Core 8.0  
**Primary Dependencies**: Entity Framework Core 8.x, Npgsql.EntityFrameworkCore.PostgreSQL, xUnit, Microsoft.AspNetCore.Mvc.Testing, Moq  
**Storage**: PostgreSQL 14+ with EF Core migrations  
**Testing**: xUnit (unit tests for business logic), WebApplicationFactory (API integration tests), Moq (mocking dependencies)  
**Target Platform**: Cross-platform (.NET 8 runtime) - Windows/Linux/macOS  
**Project Type**: multi-project-solution (EstanteVirtual.Data, EstanteVirtual.Api, EstanteVirtual.Web)  
**Performance Goals**: API response time <100ms p95 for CRUD operations, gallery load <2 seconds for 100 books  
**Constraints**: Blazor frontend MUST NOT reference Data project directly, all data access via API; EF Core only (no raw SQL unless justified); single-user MVP (no authentication/authorization)  
**Scale/Scope**: Support up to 100 books and 100 reviews without performance degradation; 2 main entities (Book, Review); ~6 API endpoints

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Stack Tecnológica (.NET 8+)**:
- ✅ Backend uses ASP.NET Core Web API (.NET 8.0)
- ✅ Frontend uses Blazor Server (API-independent via HttpClient)
- ✅ No usage of .NET Framework (legacy)

**Persistência (EF Core + PostgreSQL)**:
- ✅ Entity Framework Core 8.x is the ORM
- ✅ PostgreSQL 14+ is the target database
- ✅ No direct database access from Blazor frontend (EstanteVirtual.Web will NOT reference EstanteVirtual.Data)

**TDD (NON-NEGOTIABLE)**:
- ✅ Tests (xUnit) will be written BEFORE implementation for all business logic and API endpoints
- ✅ Tests will be verified to FAIL before writing implementation code (Red-Green-Refactor)
- ✅ Integration tests for API endpoints using WebApplicationFactory

**API Limpa (RESTful)**:
- ✅ API will follow RESTful conventions (proper HTTP verbs, status codes, resource naming)
- ✅ Frontend ONLY accesses data through Web API (no direct DbContext usage in Blazor)
- ✅ API is stateless with proper HTTP status codes (200, 201, 400, 404, 500)

**Simplicidade (YAGNI)**:
- ✅ No features/abstractions beyond specification (no search, delete, multi-user in MVP)
- ✅ Direct EF Core DbContext usage in API (no Repository/UoW patterns - keeping it simple)
- ✅ Simple controller → service → DbContext flow without unnecessary layers

**All gates PASSED** ✅ - No violations, no complexity justifications needed.

### Re-evaluation After Phase 1 Design

**Date**: 2025-10-23  
**Status**: ✅ **ALL GATES STILL PASSING**

After completing Phase 0 (Research) and Phase 1 (Design - data-model.md, contracts/, quickstart.md):

**Stack Tecnológica (.NET 8+)**:
- ✅ research.md confirms ASP.NET Core 8.0 Web API architecture
- ✅ quickstart.md documents Blazor Server setup with HttpClient consumption
- ✅ No .NET Framework usage planned

**Persistência (EF Core + PostgreSQL)**:
- ✅ data-model.md specifies EF Core 8.x with Fluent API configuration
- ✅ quickstart.md includes PostgreSQL setup and migration commands
- ✅ Architecture diagram confirms EstanteVirtual.Web does NOT reference Data project

**TDD (NON-NEGOTIABLE)**:
- ✅ quickstart.md includes TDD workflow (Red-Green-Refactor) with examples
- ✅ Test project structure defined (Api.Tests, Data.Tests)
- ✅ Integration testing strategy documented (WebApplicationFactory)

**API Limpa (RESTful)**:
- ✅ contracts/api-spec.yaml follows RESTful conventions (GET, POST, proper status codes)
- ✅ 4 endpoints defined: GET/POST /books, GET /books/{id}, POST /books/{id}/review
- ✅ DTOs documented in data-model.md (input/output separation)

**Simplicidade (YAGNI)**:
- ✅ research.md explicitly rejects Repository/UoW patterns for MVP
- ✅ Direct DbContext usage in controllers (no unnecessary service layer)
- ✅ No features beyond spec (no search, delete, multi-user)

**Design Validation**:
- ✅ 2 entities only (Book, Review) - minimal for MVP
- ✅ 4 API endpoints - covers all 3 user stories
- ✅ No over-engineering detected
- ✅ All design documents (research, data-model, contracts, quickstart) align with constitution

**Conclusion**: Design phase complete. Ready for Phase 2 (/speckit.tasks to generate implementation tasks).

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
EstanteVirtual.sln                                    # Solution file

src/
├── EstanteVirtual.Data/                             # Data layer (class library)
│   ├── EstanteVirtual.Data.csproj
│   ├── Models/
│   │   ├── Book.cs                                  # Book entity
│   │   └── Review.cs                                # Review entity
│   ├── Data/
│   │   ├── AppDbContext.cs                          # EF Core DbContext
│   │   └── EntityConfigurations/
│   │       ├── BookConfiguration.cs                 # Fluent API config for Book
│   │       └── ReviewConfiguration.cs               # Fluent API config for Review
│   └── Migrations/                                  # EF Core migrations (auto-generated)
│
├── EstanteVirtual.Api/                              # Backend Web API
│   ├── EstanteVirtual.Api.csproj
│   ├── Program.cs                                   # Application entry point
│   ├── appsettings.json                             # Configuration (connection strings)
│   ├── Controllers/
│   │   ├── BooksController.cs                       # Books CRUD endpoints
│   │   └── ReviewsController.cs                     # Reviews CRUD endpoints
│   ├── DTOs/                                        # Data Transfer Objects
│   │   ├── BookDto.cs
│   │   ├── CreateBookDto.cs
│   │   ├── ReviewDto.cs
│   │   └── CreateReviewDto.cs
│   └── Services/                                    # Business logic (optional for simple MVP)
│       └── (empty - direct DbContext usage for MVP simplicity)
│
└── EstanteVirtual.Web/                              # Frontend Blazor Server
    ├── EstanteVirtual.Web.csproj
    ├── Program.cs                                   # Blazor application entry point
    ├── appsettings.json                             # Configuration (API base URL)
    ├── Pages/
    │   ├── Index.razor                              # Gallery page (home)
    │   ├── BookDetails.razor                        # Book details + review page
    │   └── _Host.cshtml
    ├── Components/
    │   ├── AddBookForm.razor                        # Form component to add books
    │   └── BookCard.razor                           # Book card for gallery
    ├── Services/
    │   └── BookApiService.cs                        # HttpClient wrapper for API calls
    └── wwwroot/
        ├── css/
        └── images/
            └── book-placeholder.png                 # Default book cover image

tests/
├── EstanteVirtual.Api.Tests/                        # API integration tests
│   ├── EstanteVirtual.Api.Tests.csproj
│   ├── BooksControllerTests.cs                      # Tests for Books API
│   ├── ReviewsControllerTests.cs                    # Tests for Reviews API
│   └── Helpers/
│       └── WebApplicationFactoryHelper.cs           # Custom WebApplicationFactory setup
│
└── EstanteVirtual.Data.Tests/                       # Unit tests for data models (if needed)
    ├── EstanteVirtual.Data.Tests.csproj
    └── Models/
        ├── BookTests.cs                             # Book entity validation tests
        └── ReviewTests.cs                           # Review entity validation tests
```

**Structure Decision**: 

Selecionada **Option 3: Multi-project solution** conforme especificado pelo usuário. Esta estrutura oferece separação clara de responsabilidades:

- **EstanteVirtual.Data**: Contém apenas entidades EF Core e DbContext. É uma biblioteca de classe reutilizável.
- **EstanteVirtual.Api**: API REST que referencia Data. Expõe endpoints HTTP e gerencia lógica de negócio simples.
- **EstanteVirtual.Web**: Frontend Blazor Server que consome a API via HttpClient. **NÃO** referencia Data diretamente, cumprindo o princípio constitucional IV (API Limpa).

Esta arquitetura permite:
- Evolução independente de frontend e backend
- Futuro suporte a Blazor WebAssembly sem mudanças na API
- Testes isolados de cada camada
- Clara separação de boundaries (Blazor → API → Data → PostgreSQL)

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
