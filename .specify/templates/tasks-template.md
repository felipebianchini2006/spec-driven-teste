---

description: "Task list template for feature implementation"
---

# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests (TDD MANDATORY)**: Per constitution, ALL logic and API endpoints MUST have tests written FIRST. Tests must FAIL before implementation begins. Use xUnit for unit tests and WebApplicationFactory for API integration tests.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Single ASP.NET Core project**: `src/`, `tests/` at repository root
- **Separated backend + Blazor**: `backend/src/`, `frontend/` with Components/Pages
- **Multi-project solution**: `src/ProjectName.Api/`, `src/ProjectName.Domain/`, `tests/ProjectName.Tests/`
- Paths shown below assume single project - adjust based on plan.md structure

<!-- 
  ============================================================================
  IMPORTANT: The tasks below are SAMPLE TASKS for illustration purposes only.
  
  The /speckit.tasks command MUST replace these with actual tasks based on:
  - User stories from spec.md (with their priorities P1, P2, P3...)
  - Feature requirements from plan.md
  - Entities from data-model.md
  - Endpoints from contracts/
  
  Tasks MUST be organized by user story so each story can be:
  - Implemented independently
  - Tested independently
  - Delivered as an MVP increment
  
  DO NOT keep these sample tasks in the generated tasks.md file.
  ============================================================================
-->

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create .NET solution and project structure per implementation plan
- [ ] T002 Initialize ASP.NET Core Web API project with required NuGet packages (EF Core, Npgsql, xUnit)
- [ ] T003 [P] Configure code analysis, formatting (.editorconfig, StyleCop if used)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

Examples of foundational tasks (adjust based on your project):

- [ ] T004 Create ASP.NET Core Web API project structure
- [ ] T005 [P] Setup Entity Framework Core DbContext and initial configuration
- [ ] T006 [P] Configure PostgreSQL connection and connection string management
- [ ] T007 [P] Setup authentication/authorization middleware (if required)
- [ ] T008 [P] Configure global error handling and exception filters
- [ ] T009 [P] Setup structured logging (ILogger, Serilog, etc.)
- [ ] T010 [P] Configure dependency injection container
- [ ] T011 Setup xUnit test projects (unit and integration)
- [ ] T012 Configure WebApplicationFactory for integration testing

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - [Title] (Priority: P1) 🎯 MVP

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 1 (TDD MANDATORY - Write FIRST) ✅

> **CRITICAL**: Per constitution Principle III, write these tests FIRST and ensure they FAIL before implementation

- [ ] T010 [P] [US1] Unit tests for [business logic/service] in tests/unit/[Name]Tests.cs (xUnit)
- [ ] T011 [P] [US1] Integration test for [API endpoint] in tests/integration/[Controller]IntegrationTests.cs (WebApplicationFactory)

### Implementation for User Story 1

- [ ] T012 [P] [US1] Create [Entity1] EF Core entity in src/Models/[Entity1].cs
- [ ] T013 [P] [US1] Create [Entity2] EF Core entity in src/Models/[Entity2].cs
- [ ] T014 [US1] Add DbContext configuration for entities in src/Data/ApplicationDbContext.cs (depends on T012, T013)
- [ ] T015 [US1] Create EF migration for new entities (dotnet ef migrations add)
- [ ] T016 [US1] Implement [Service] business logic in src/Services/[Service].cs
- [ ] T017 [US1] Implement [Controller] API endpoint in src/Controllers/[Controller].cs
- [ ] T018 [US1] Add validation and error handling with proper HTTP status codes
- [ ] T019 [US1] Add logging using ILogger<T> for user story 1 operations

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - [Title] (Priority: P2)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 2 (TDD MANDATORY - Write FIRST) ✅

- [ ] T018 [P] [US2] Unit tests for [business logic/service] in tests/unit/[Name]Tests.cs (xUnit)
- [ ] T019 [P] [US2] Integration test for [API endpoint] in tests/integration/[Controller]IntegrationTests.cs

### Implementation for User Story 2

- [ ] T020 [P] [US2] Create [Entity] EF Core entity in src/Models/[Entity].cs
- [ ] T021 [US2] Add DbContext configuration and create migration
- [ ] T022 [US2] Implement [Service] business logic in src/Services/[Service].cs
- [ ] T023 [US2] Implement [Controller] API endpoint in src/Controllers/[Controller].cs
- [ ] T024 [US2] Integrate with User Story 1 components (if needed)

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - [Title] (Priority: P3)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 3 (TDD MANDATORY - Write FIRST) ✅

- [ ] T024 [P] [US3] Unit tests for [business logic/service] in tests/unit/[Name]Tests.cs (xUnit)
- [ ] T025 [P] [US3] Integration test for [API endpoint] in tests/integration/[Controller]IntegrationTests.cs

### Implementation for User Story 3

- [ ] T026 [P] [US3] Create [Entity] EF Core entity in src/Models/[Entity].cs
- [ ] T027 [US3] Add DbContext configuration and create migration
- [ ] T028 [US3] Implement [Service] business logic in src/Services/[Service].cs
- [ ] T029 [US3] Implement [Controller] API endpoint in src/Controllers/[Controller].cs

**Checkpoint**: All user stories should now be independently functional

---

[Add more user story phases as needed, following the same pattern]

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] TXXX [P] Documentation updates in docs/
- [ ] TXXX Code cleanup and refactoring
- [ ] TXXX Performance optimization across all stories
- [ ] TXXX [P] Additional unit tests (if requested) in tests/unit/
- [ ] TXXX Security hardening
- [ ] TXXX Run quickstart.md validation

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1 but should be independently testable
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - May integrate with US1/US2 but should be independently testable

### Within Each User Story

- Tests (if included) MUST be written and FAIL before implementation
- Models before services
- Services before endpoints
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- All tests for a user story marked [P] can run in parallel
- Models within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together (TDD - write these FIRST):
Task: "Unit tests for [Service] in tests/unit/[Service]Tests.cs"
Task: "Integration test for [Controller] in tests/integration/[Controller]IntegrationTests.cs"

# After tests fail (Red phase), launch models together:
Task: "Create [Entity1] EF Core entity in src/Models/[Entity1].cs"
Task: "Create [Entity2] EF Core entity in src/Models/[Entity2].cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo
5. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
