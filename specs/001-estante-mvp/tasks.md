---
description: "Task list for Estante Virtual MVP implementation"
---

# Tasks: Estante Virtual MVP

**Input**: Design documents from `/specs/001-estante-mvp/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests (TDD MANDATORY)**: Per constitution, ALL logic and API endpoints MUST have tests written FIRST. Tests must FAIL before implementation begins. Use xUnit for unit tests and WebApplicationFactory for API integration tests.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Multi-project solution**: `src/EstanteVirtual.Data/`, `src/EstanteVirtual.Api/`, `src/EstanteVirtual.Web/`, `tests/`
- Paths shown below follow the multi-project structure defined in plan.md

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create .NET solution file EstanteVirtual.sln in repository root
- [X] T002 [P] Create EstanteVirtual.Data class library project in src/EstanteVirtual.Data/ targeting .NET 8.0
- [X] T003 [P] Create EstanteVirtual.Api Web API project in src/EstanteVirtual.Api/ targeting .NET 8.0
- [X] T004 [P] Create EstanteVirtual.Web Blazor Server project in src/EstanteVirtual.Web/ targeting .NET 8.0
- [X] T005 [P] Create EstanteVirtual.Api.Tests xUnit project in tests/EstanteVirtual.Api.Tests/ targeting .NET 8.0
- [X] T006 [P] Create EstanteVirtual.Data.Tests xUnit project in tests/EstanteVirtual.Data.Tests/ targeting .NET 8.0
- [X] T007 Add all projects to EstanteVirtual.sln solution file
- [X] T008 [P] Add NuGet packages to EstanteVirtual.Data: Microsoft.EntityFrameworkCore (8.0.0), Microsoft.EntityFrameworkCore.Design (8.0.0), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.0)
- [X] T009 [P] Add project reference from EstanteVirtual.Api to EstanteVirtual.Data
- [X] T010 [P] Add NuGet package Swashbuckle.AspNetCore (6.5.0) to EstanteVirtual.Api for Swagger documentation
- [X] T011 [P] Add project reference from EstanteVirtual.Api.Tests to EstanteVirtual.Api
- [X] T012 [P] Add NuGet packages to EstanteVirtual.Api.Tests: Microsoft.AspNetCore.Mvc.Testing (8.0.0), Moq (4.20.70)
- [X] T013 [P] Add project reference from EstanteVirtual.Data.Tests to EstanteVirtual.Data
- [X] T014 [P] Configure .editorconfig for code style consistency in repository root

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T015 Create PostgreSQL database 'estantevirtual' (manual or via script)
- [X] T016 Configure connection string in EstanteVirtual.Api using User Secrets (dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=estantevirtual;Username=postgres;Password=<password>")
- [X] T017 Create AppDbContext class in src/EstanteVirtual.Data/Data/AppDbContext.cs inheriting from DbContext
- [X] T018 [P] Configure AppDbContext with DbSet<Book> Books and DbSet<Review> Reviews properties
- [X] T019 [P] Register AppDbContext in EstanteVirtual.Api/Program.cs with PostgreSQL provider
- [X] T020 [P] Configure Swagger/OpenAPI in EstanteVirtual.Api/Program.cs
- [X] T021 [P] Configure CORS in EstanteVirtual.Api/Program.cs to allow Blazor frontend
- [X] T022 [P] Configure global exception handling middleware in EstanteVirtual.Api/Program.cs
- [X] T023 [P] Configure IHttpClientFactory in EstanteVirtual.Web/Program.cs with API base URL
- [X] T024 Create WebApplicationFactory helper in tests/EstanteVirtual.Api.Tests/Helpers/TestWebApplicationFactory.cs for integration tests
- [X] T025 Configure test database (in-memory or test PostgreSQL instance) in TestWebApplicationFactory

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Adicionar Livro à Estante (Priority: P1) 🎯 MVP

**Goal**: Permitir usuário adicionar livro com título, autor e URL da capa opcional

**Independent Test**: Pode ser completamente testada preenchendo formulário, submetendo e verificando persistência após reload

### Tests for User Story 1 (TDD MANDATORY - Write FIRST) ✅

> **CRITICAL**: Per constitution Principle III, write these tests FIRST and ensure they FAIL before implementation

- [ ] T026 [P] [US1] Unit test for Book entity validation (title required, max 200 chars) in tests/EstanteVirtual.Data.Tests/Models/BookTests.cs
- [ ] T027 [P] [US1] Unit test for Book entity validation (author required, max 100 chars) in tests/EstanteVirtual.Data.Tests/Models/BookTests.cs
- [ ] T028 [P] [US1] Unit test for Book entity validation (coverImageUrl optional, max 500 chars) in tests/EstanteVirtual.Data.Tests/Models/BookTests.cs
- [ ] T029 [P] [US1] Integration test for POST /api/books with valid data (returns 201 Created) in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs
- [ ] T030 [P] [US1] Integration test for POST /api/books without title (returns 400 Bad Request) in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs
- [ ] T031 [P] [US1] Integration test for POST /api/books without author (returns 400 Bad Request) in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs
- [ ] T032 [P] [US1] Integration test for POST /api/books with cover URL (returns 201 and includes URL) in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs
- [ ] T033 [P] [US1] Integration test for data persistence (add book, reload, verify exists) in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs

### Implementation for User Story 1

- [ ] T034 [P] [US1] Create Book entity class in src/EstanteVirtual.Data/Models/Book.cs with Id, Title, Author, CoverImageUrl, CreatedAt properties
- [ ] T035 [US1] Create BookConfiguration class in src/EstanteVirtual.Data/Data/EntityConfigurations/BookConfiguration.cs implementing IEntityTypeConfiguration<Book>
- [ ] T036 [US1] Configure Book entity using Fluent API in BookConfiguration (primary key, required fields, max lengths, CreatedAt default)
- [ ] T037 [US1] Apply BookConfiguration in AppDbContext.OnModelCreating method
- [ ] T038 [US1] Create EF Core migration for Book entity (dotnet ef migrations add AddBookEntity --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api)
- [ ] T039 [US1] Apply migration to database (dotnet ef database update --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api)
- [ ] T040 [P] [US1] Create BookDto class in src/EstanteVirtual.Api/DTOs/BookDto.cs with all Book properties plus Review property
- [ ] T041 [P] [US1] Create CreateBookDto class in src/EstanteVirtual.Api/DTOs/CreateBookDto.cs with Title, Author, CoverImageUrl and validation attributes
- [ ] T042 [US1] Create BooksController class in src/EstanteVirtual.Api/Controllers/BooksController.cs with AppDbContext injection
- [ ] T043 [US1] Implement POST /api/books endpoint in BooksController accepting CreateBookDto and returning BookDto with 201 Created status
- [ ] T044 [US1] Add model validation and error handling in POST /api/books endpoint (return 400 for invalid input)
- [ ] T045 [US1] Add logging using ILogger<BooksController> in POST /api/books endpoint
- [ ] T046 [US1] Create BookApiService class in src/EstanteVirtual.Web/Services/BookApiService.cs with HttpClient injection
- [ ] T047 [US1] Implement CreateBookAsync method in BookApiService calling POST /api/books endpoint
- [ ] T048 [US1] Create AddBookForm component in src/EstanteVirtual.Web/Components/AddBookForm.razor with title, author, coverUrl inputs
- [ ] T049 [US1] Add validation to AddBookForm component (required fields, max lengths)
- [ ] T050 [US1] Wire AddBookForm submit button to call BookApiService.CreateBookAsync
- [ ] T051 [US1] Display success/error messages in AddBookForm after submission
- [ ] T052 [US1] Add AddBookForm component to Index.razor page in src/EstanteVirtual.Web/Pages/Index.razor

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently - Users can add books to the shelf

---

## Phase 4: User Story 2 - Visualizar Galeria de Livros (Priority: P2)

**Goal**: Exibir todos os livros cadastrados em formato de galeria na página inicial

**Independent Test**: Pode ser testada adicionando múltiplos livros (User Story 1) e verificando exibição em galeria

### Tests for User Story 2 (TDD MANDATORY - Write FIRST) ✅

- [ ] T053 [P] [US2] Integration test for GET /api/books returns empty array when no books exist in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs
- [ ] T054 [P] [US2] Integration test for GET /api/books returns all books after adding multiple in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs
- [ ] T055 [P] [US2] Integration test for GET /api/books includes book with cover URL in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs
- [ ] T056 [P] [US2] Integration test for GET /api/books includes book without cover URL (null) in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs

### Implementation for User Story 2

- [ ] T057 [US2] Implement GET /api/books endpoint in BooksController returning List<BookDto> with 200 OK status
- [ ] T058 [US2] Query all books from database using EF Core in GET /api/books endpoint (Include Review navigation property)
- [ ] T059 [US2] Add logging for GET /api/books endpoint
- [ ] T060 [US2] Implement GetAllBooksAsync method in BookApiService calling GET /api/books endpoint
- [ ] T061 [US2] Create BookCard component in src/EstanteVirtual.Web/Components/BookCard.razor accepting Book parameter
- [ ] T062 [US2] Display book cover image in BookCard component (use img tag with src from coverImageUrl)
- [ ] T063 [US2] Display placeholder image in BookCard when coverImageUrl is null in src/EstanteVirtual.Web/wwwroot/images/book-placeholder.png
- [ ] T064 [US2] Display book title below/over cover image in BookCard component
- [ ] T065 [US2] Add CSS styling to BookCard component for grid layout and visual appeal
- [ ] T066 [US2] Update Index.razor to fetch books on load using BookApiService.GetAllBooksAsync
- [ ] T067 [US2] Display empty state message in Index.razor when no books exist ("Sua estante está vazia. Adicione seu primeiro livro!")
- [ ] T068 [US2] Display grid of BookCard components in Index.razor when books exist
- [ ] T069 [US2] Add CSS for responsive grid layout in Index.razor (grid-template-columns, gap)

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently - Users can add books and see them in gallery

---

## Phase 5: User Story 3 - Avaliar Livro com Nota e Resenha (Priority: P3)

**Goal**: Permitir usuário clicar em livro, ver detalhes e adicionar/editar avaliação com nota (1-5) e resenha

**Independent Test**: Pode ser testada adicionando livro, clicando nele, adicionando avaliação e verificando persistência

### Tests for User Story 3 (TDD MANDATORY - Write FIRST) ✅

- [ ] T070 [P] [US3] Unit test for Review entity validation (rating required, range 1-5) in tests/EstanteVirtual.Data.Tests/Models/ReviewTests.cs
- [ ] T071 [P] [US3] Unit test for Review entity validation (reviewText optional, max 2000 chars) in tests/EstanteVirtual.Data.Tests/Models/ReviewTests.cs
- [ ] T072 [P] [US3] Unit test for Review entity BookId foreign key constraint in tests/EstanteVirtual.Data.Tests/Models/ReviewTests.cs
- [ ] T073 [P] [US3] Integration test for GET /api/books/{id} returns book with review if exists in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs
- [ ] T074 [P] [US3] Integration test for GET /api/books/{id} returns 404 when book doesn't exist in tests/EstanteVirtual.Api.Tests/Controllers/BooksControllerTests.cs
- [ ] T075 [P] [US3] Integration test for POST /api/books/{id}/review creates new review (returns 201) in tests/EstanteVirtual.Api.Tests/Controllers/ReviewsControllerTests.cs
- [ ] T076 [P] [US3] Integration test for POST /api/books/{id}/review updates existing review (returns 200) in tests/EstanteVirtual.Api.Tests/Controllers/ReviewsControllerTests.cs
- [ ] T077 [P] [US3] Integration test for POST /api/books/{id}/review without rating (returns 400) in tests/EstanteVirtual.Api.Tests/Controllers/ReviewsControllerTests.cs
- [ ] T078 [P] [US3] Integration test for POST /api/books/{id}/review with rating out of range (returns 400) in tests/EstanteVirtual.Api.Tests/Controllers/ReviewsControllerTests.cs

### Implementation for User Story 3

- [ ] T079 [P] [US3] Create Review entity class in src/EstanteVirtual.Data/Models/Review.cs with Id, BookId, Rating, ReviewText, CreatedAt, UpdatedAt properties
- [ ] T080 [US3] Add Review navigation property to Book entity in src/EstanteVirtual.Data/Models/Book.cs
- [ ] T081 [US3] Create ReviewConfiguration class in src/EstanteVirtual.Data/Data/EntityConfigurations/ReviewConfiguration.cs implementing IEntityTypeConfiguration<Review>
- [ ] T082 [US3] Configure Review entity using Fluent API in ReviewConfiguration (primary key, foreign key to Book, unique constraint on BookId, required rating, max lengths)
- [ ] T083 [US3] Configure one-to-zero-or-one relationship between Book and Review in BookConfiguration
- [ ] T084 [US3] Apply ReviewConfiguration in AppDbContext.OnModelCreating method
- [ ] T085 [US3] Create EF Core migration for Review entity (dotnet ef migrations add AddReviewEntity --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api)
- [ ] T086 [US3] Apply migration to database (dotnet ef database update --project src/EstanteVirtual.Data --startup-project src/EstanteVirtual.Api)
- [ ] T087 [P] [US3] Create ReviewDto class in src/EstanteVirtual.Api/DTOs/ReviewDto.cs with all Review properties
- [ ] T088 [P] [US3] Create CreateReviewDto class in src/EstanteVirtual.Api/DTOs/CreateReviewDto.cs with Rating and ReviewText with validation attributes
- [ ] T089 [US3] Implement GET /api/books/{id} endpoint in BooksController returning BookDto with 200 OK or 404 Not Found
- [ ] T090 [US3] Include Review navigation property in GET /api/books/{id} query using EF Core
- [ ] T091 [US3] Add logging for GET /api/books/{id} endpoint
- [ ] T092 [US3] Create ReviewsController class in src/EstanteVirtual.Api/Controllers/ReviewsController.cs with AppDbContext injection
- [ ] T093 [US3] Implement POST /api/books/{id}/review endpoint in ReviewsController accepting CreateReviewDto
- [ ] T094 [US3] Check if book exists in POST /api/books/{id}/review endpoint (return 404 if not)
- [ ] T095 [US3] Check if review already exists for book in POST /api/books/{id}/review endpoint
- [ ] T096 [US3] Create new review if doesn't exist (return 201 Created) in POST /api/books/{id}/review endpoint
- [ ] T097 [US3] Update existing review if exists (return 200 OK, update UpdatedAt) in POST /api/books/{id}/review endpoint
- [ ] T098 [US3] Add model validation and error handling in POST /api/books/{id}/review endpoint
- [ ] T099 [US3] Add logging using ILogger<ReviewsController> in POST /api/books/{id}/review endpoint
- [ ] T100 [US3] Implement GetBookByIdAsync method in BookApiService calling GET /api/books/{id} endpoint
- [ ] T101 [US3] Implement AddOrUpdateReviewAsync method in BookApiService calling POST /api/books/{id}/review endpoint
- [ ] T102 [US3] Make BookCard component clickable (navigate to book details page) using NavigationManager
- [ ] T103 [US3] Create BookDetails.razor page in src/EstanteVirtual.Web/Pages/BookDetails.razor with route @page "/books/{id:int}"
- [ ] T104 [US3] Fetch book details on load in BookDetails.razor using BookApiService.GetBookByIdAsync
- [ ] T105 [US3] Display book title, author and cover image in BookDetails.razor
- [ ] T106 [US3] Display existing review (rating and text) in BookDetails.razor if book has review
- [ ] T107 [US3] Create rating selector component (1-5 stars) in BookDetails.razor using radio buttons or star icons
- [ ] T108 [US3] Create review text area in BookDetails.razor with max 2000 characters
- [ ] T109 [US3] Add save button in BookDetails.razor wired to call BookApiService.AddOrUpdateReviewAsync
- [ ] T110 [US3] Display success/error messages in BookDetails.razor after saving review
- [ ] T111 [US3] Refresh book details after saving review to show updated data in BookDetails.razor
- [ ] T112 [US3] Add validation to review form in BookDetails.razor (rating required, text optional)

**Checkpoint**: All user stories should now be independently functional - Users can add books, view gallery, and add/edit reviews

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T113 [P] Add XML documentation comments to all public API methods in BooksController and ReviewsController
- [ ] T114 [P] Configure Swagger to include XML documentation in EstanteVirtual.Api/Program.cs
- [ ] T115 [P] Add error boundary in Blazor app (src/EstanteVirtual.Web/App.razor) to handle unhandled exceptions
- [ ] T116 [P] Add loading spinners/indicators in Blazor components during API calls
- [ ] T117 [P] Optimize EF Core queries with AsNoTracking() for read-only operations
- [ ] T118 [P] Add unit tests for DTOs validation attributes in tests/EstanteVirtual.Api.Tests/DTOs/
- [ ] T119 [P] Add integration tests for CORS policy in tests/EstanteVirtual.Api.Tests/
- [ ] T120 [P] Add integration tests for error handling middleware in tests/EstanteVirtual.Api.Tests/
- [ ] T121 Code cleanup and remove unused using statements across all projects
- [ ] T122 Performance testing with 100 books in database (verify <2s load time per success criteria)
- [ ] T123 Run quickstart.md validation (follow setup guide end-to-end and verify all steps work)
- [ ] T124 Update README.md with project overview, setup instructions and architecture diagram

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational (Phase 2) - No dependencies on other stories ✅ **MVP STARTS HERE**
- **User Story 2 (Phase 4)**: Depends on Foundational (Phase 2) - Can integrate with US1 but independently testable
- **User Story 3 (Phase 5)**: Depends on Foundational (Phase 2) - Integrates with US1 and US2 but independently testable
- **Polish (Phase 6)**: Depends on desired user stories being complete

### User Story Dependencies

**US1 (P1 - Add Book)**: 
- Foundation ready ✓
- No dependencies on other stories
- **Delivers MVP**: Basic functionality to add books

**US2 (P2 - Gallery)**:
- Foundation ready ✓
- Uses books from US1 for display
- Independently testable (can add books via API directly for tests)

**US3 (P3 - Review)**:
- Foundation ready ✓
- Uses books from US1
- Uses gallery from US2 for navigation
- Independently testable (can add book and navigate via URL for tests)

### Within Each User Story

**TDD Workflow (MANDATORY)**:
1. Write tests FIRST (T026-T033 for US1, T053-T056 for US2, T070-T078 for US3)
2. Run tests and verify they FAIL (Red phase)
3. Implement code to make tests pass (Green phase)
4. Refactor while keeping tests green (Refactor phase)

**Implementation Order Within Story**:
1. Tests (write all tests for story, verify failures)
2. Entities (models with navigation properties)
3. EF Core configuration (Fluent API)
4. Migrations (create and apply)
5. DTOs (input and output)
6. Controllers (API endpoints with validation)
7. Services (Blazor HTTP clients)
8. Components (Blazor UI)
9. Integration (wire components to services)

### Parallel Opportunities

**Setup Phase (Phase 1)**:
- All project creation tasks (T002-T006) can run in parallel
- All NuGet package installation tasks (T008, T010, T012) can run in parallel

**Foundational Phase (Phase 2)**:
- Configuration tasks (T019-T023) can run in parallel after T017-T018 complete

**Within Each User Story**:
- All tests for a story can be written in parallel (e.g., T026-T033 for US1)
- Entity and DTO creation can happen in parallel (e.g., T034 and T040-T041 for US1)
- Independent components can be built in parallel

**Cross-Story Parallelization** (if team capacity allows):
- Once Foundation complete, US1, US2, US3 can be worked on simultaneously by different developers
- Each story is independently testable and deployable

---

## Parallel Example: User Story 1

```bash
# Phase 1: Write all tests in parallel (Red phase)
Task: "Unit test for Book entity validation (title)" → T026
Task: "Unit test for Book entity validation (author)" → T027
Task: "Integration test POST /api/books valid" → T029
Task: "Integration test POST /api/books no title" → T030
# ... all tests T026-T033 can run together

# Phase 2: After tests written, create entities and DTOs in parallel
Task: "Create Book entity" → T034
Task: "Create BookDto" → T040
Task: "Create CreateBookDto" → T041

# Phase 3: After migrations applied, create controller and service in parallel
Task: "Create BooksController" → T042
Task: "Create BookApiService" → T046
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T014)
2. Complete Phase 2: Foundational (T015-T025) - CRITICAL, blocks all stories
3. Complete Phase 3: User Story 1 (T026-T052)
4. **STOP and VALIDATE**: 
   - Run all US1 tests (should pass ✅)
   - Manually test: Add book via Blazor form
   - Verify book persists after page reload
5. **Deploy/Demo MVP** - Basic "Add Book" functionality working

### Incremental Delivery

**Sprint 1 - MVP** (US1 only):
- Setup + Foundational + US1 = ~52 tasks
- Delivers: Users can add books
- Timeline: ~3-5 days (with TDD)

**Sprint 2 - Gallery** (add US2):
- US2 = ~17 tasks
- Delivers: Users can add and VIEW books in gallery
- Timeline: ~1-2 days

**Sprint 3 - Reviews** (add US3):
- US3 = ~43 tasks
- Delivers: Complete MVP with reviews
- Timeline: ~3-4 days

**Sprint 4 - Polish**:
- Phase 6 = ~12 tasks
- Delivers: Production-ready application
- Timeline: ~1-2 days

**Total**: ~124 tasks, 10-13 days with 1 developer following TDD

### Parallel Team Strategy

With 3 developers after Foundation complete:
- **Developer A**: User Story 1 (T026-T052) → MVP delivery
- **Developer B**: User Story 2 (T053-T069) → Gallery feature
- **Developer C**: User Story 3 (T070-T112) → Reviews feature

Stories integrate and test independently. Final integration testing when all complete.

---

## Notes

- **[P] marker**: Task uses different file than previous task, no blocking dependencies
- **[US#] label**: Maps to user story in spec.md (required for story phase tasks)
- **TDD Required**: Constitution Principle III is non-negotiable - tests FIRST, verify failures, then implement
- **Independent Stories**: Each story delivers value on its own and can be deployed independently
- **Checkpoints**: Stop after each story completion to validate independently before moving to next
- **Constitution Compliance**: All gates from plan.md are baked into this task structure
  - ✅ .NET 8 + ASP.NET Core + Blazor Server
  - ✅ EF Core + PostgreSQL
  - ✅ TDD with xUnit + WebApplicationFactory
  - ✅ RESTful API, Blazor consumes via HTTP (no direct Data reference)
  - ✅ YAGNI - no Repository pattern, direct DbContext usage

---

## Validation Checklist

Before marking feature complete:

- [ ] All 124 tasks completed
- [ ] All unit tests passing (xUnit)
- [ ] All integration tests passing (WebApplicationFactory)
- [ ] Manual testing of all 3 user stories successful
- [ ] Each user story independently testable and working
- [ ] Constitution Check gates still passing (no violations introduced)
- [ ] Performance criteria met (<100ms API p95, <2s gallery load for 100 books)
- [ ] Success criteria from spec.md verified
- [ ] quickstart.md validated end-to-end
- [ ] Code follows .NET conventions (PascalCase, async/await, etc.)
- [ ] No direct Data reference from Blazor (architecture validated)
- [ ] API Swagger documentation complete and accurate

**When all checked**: Feature ready for deployment 🚀
