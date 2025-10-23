# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: .NET 8.0 (or specify if .NET 9+) | ASP.NET Core
**Primary Dependencies**: Entity Framework Core, xUnit, PostgreSQL (Npgsql provider) or NEEDS CLARIFICATION  
**Storage**: PostgreSQL with EF Core or NEEDS CLARIFICATION  
**Testing**: xUnit (unit), WebApplicationFactory (integration) or NEEDS CLARIFICATION  
**Target Platform**: Cross-platform (.NET runtime) - Linux/Windows/macOS or NEEDS CLARIFICATION
**Project Type**: web-api / web-api-blazor / multi-project-solution  
**Performance Goals**: [domain-specific, e.g., <100ms API response p95, 1000 req/s or NEEDS CLARIFICATION]  
**Constraints**: [domain-specific, e.g., EF Core queries only, no raw SQL without justification or NEEDS CLARIFICATION]  
**Scale/Scope**: [domain-specific, e.g., 1000 concurrent users, 10 entities, 20 endpoints or NEEDS CLARIFICATION]

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Stack Tecnológica (.NET 8+)**:
- ✅ / ⚠️ Backend uses ASP.NET Core Web API (.NET 8+)
- ✅ / ⚠️ Frontend uses Blazor (Server initially, API-independent)
- ✅ / ⚠️ No usage of .NET Framework (legacy)

**Persistência (EF Core + PostgreSQL)**:
- ✅ / ⚠️ Entity Framework Core is the ORM
- ✅ / ⚠️ PostgreSQL is the target database
- ✅ / ⚠️ No direct database access from Blazor frontend

**TDD (NON-NEGOTIABLE)**:
- ✅ / ⚠️ Tests (xUnit) written BEFORE implementation
- ✅ / ⚠️ Tests verified to FAIL before writing implementation code
- ✅ / ⚠️ Integration tests for API endpoints using WebApplicationFactory

**API Limpa (RESTful)**:
- ✅ / ⚠️ API follows RESTful conventions
- ✅ / ⚠️ Frontend ONLY accesses data through Web API
- ✅ / ⚠️ API is stateless with proper HTTP status codes

**Simplicidade (YAGNI)**:
- ✅ / ⚠️ No features/abstractions beyond specification
- ✅ / ⚠️ Complex patterns (Repository, UoW) justified if used
- ✅ / ⚠️ Direct EF Core DbContext usage unless complexity documented

*Mark ⚠️ if violated and document justification in "Complexity Tracking" section below*

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
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., src/Controllers/BooksController.cs). The delivered plan must
  not include Option labels.
-->

```text
# [REMOVE IF UNUSED] Option 1: Single ASP.NET Core project (DEFAULT for API-only or simple apps)
src/
├── Controllers/      # API endpoints (ASP.NET Core Web API)
├── Models/          # EF Core entities
├── Services/        # Business logic layer
├── Data/            # DbContext and configurations
└── Program.cs       # Application entry point

tests/
├── unit/            # xUnit unit tests for services/logic
└── integration/     # WebApplicationFactory API integration tests

# [REMOVE IF UNUSED] Option 2: Separated Backend + Blazor Frontend
backend/
├── src/
│   ├── Controllers/     # ASP.NET Core Web API endpoints
│   ├── Models/          # EF Core entities
│   ├── Services/        # Business logic
│   ├── Data/            # DbContext
│   └── Program.cs
└── tests/
    ├── unit/
    └── integration/

frontend/
├── Components/          # Blazor components
├── Pages/              # Blazor pages
├── Services/           # HTTP clients to consume backend API
└── Program.cs

# [REMOVE IF UNUSED] Option 3: Multi-project solution (when multiple APIs or complex domains)
src/
├── EstanteVirtual.Api/        # Main Web API project
├── EstanteVirtual.Domain/     # Domain models and business logic
├── EstanteVirtual.Data/       # EF Core DbContext and repositories
└── EstanteVirtual.Blazor/     # Blazor frontend project

tests/
├── EstanteVirtual.Api.Tests/
├── EstanteVirtual.Domain.Tests/
└── EstanteVirtual.Integration.Tests/
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
