<!--
═══════════════════════════════════════════════════════════════════════════════
SYNC IMPACT REPORT - Constitution Update
═══════════════════════════════════════════════════════════════════════════════
Version Change: [NEW] 1.0.0 (Initial ratification)

Modified Principles:
  - All principles newly defined for initial constitution

Added Sections:
  - Core Principles (5 principles defined)
  - Technology Stack Requirements
  - Development Workflow
  - Governance

Removed Sections:
  - None (initial version)

Template Updates Required:
  ✅ plan-template.md - Updated Constitution Check section with .NET/TDD gates
  ✅ spec-template.md - Already compatible (technology-agnostic)
  ✅ tasks-template.md - Updated with TDD-first workflow and .NET testing notes
  ✅ checklist-template.md - No updates needed (general checklist)
  ✅ agent-file-template.md - No updates needed (general template)

Follow-up TODOs:
  - None - All placeholders filled

Rationale for Version 1.0.0:
  - Initial constitution for Estante Virtual project
  - Establishes foundational governance and technical principles
  - All five core principles defined with clear rationale
═══════════════════════════════════════════════════════════════════════════════
-->

# Estante Virtual Constitution

## Core Principles

### I. Stack Tecnológica (.NET 8+)

O projeto DEVE usar .NET 8 ou versão mais recente como plataforma base. O backend DEVE ser implementado como uma ASP.NET Core Web API e o frontend DEVE ser uma aplicação Blazor. A arquitetura inicial DEVE usar Blazor Server para simplicidade, porém a Web API DEVE ser completamente separada e independente para suportar futura migração para Blazor WebAssembly sem mudanças na API.

**Rationale**: .NET 8 fornece performance moderna, suporte de longo prazo (LTS) e ecossistema maduro. A separação API/Frontend garante flexibilidade arquitetural e permite evolução independente de cada camada, facilitando futuras migrações de tecnologia de UI sem impacto no backend.

### II. Persistência de Dados (EF Core + PostgreSQL)

O Entity Framework Core (EF Core) é o ORM oficial e obrigatório para todas as operações de banco de dados. O banco de dados alvo DEVE ser PostgreSQL. Acesso direto ao banco (SQL raw ou outros ORMs) é PROIBIDO exceto em casos excepcionais documentados e aprovados.

**Rationale**: EF Core provê abstração type-safe, migrations gerenciáveis e integração nativa com .NET. PostgreSQL oferece robustez, conformidade com padrões SQL e recursos avançados gratuitos. A uniformidade no ORM reduz curva de aprendizado e facilita manutenção.

### III. TDD é Lei (NON-NEGOTIABLE)

Test-Driven Development (TDD) é INEGOCIÁVEL. A IA DEVE seguir estritamente este workflow:

1. **Escrever testes**: Testes de unidade (xUnit) para lógica de negócio e testes de integração para endpoints da API DEVEM ser escritos PRIMEIRO
2. **Validar falha**: Executar testes e confirmar que falham (Red)
3. **Implementar**: Escrever código mínimo para passar os testes (Green)
4. **Refatorar**: Melhorar código mantendo testes passando (Refactor)

Código de implementação submetido SEM testes prévios será REJEITADO.

**Rationale**: TDD garante código testável por design, previne regressões, documenta comportamento esperado e reduz bugs em produção. O ciclo Red-Green-Refactor força design incremental e melhora qualidade do código.

### IV. API Limpa (RESTful e Separação de Camadas)

A Web API DEVE seguir princípios RESTful e convenções HTTP padrão. O frontend (Blazor) NUNCA DEVE acessar o banco de dados diretamente; ele DEVE consumir exclusivamente a Web API. A API DEVE ser stateless, usar códigos de status HTTP apropriados e seguir convenções de nomenclatura REST.

**Rationale**: APIs RESTful são amplamente compreendidas, fáceis de consumir e testáveis. A separação clara entre frontend e backend permite evolução independente, facilita testes automatizados, habilita reutilização da API por outros clientes e melhora segurança através de boundary claro.

### V. Simplicidade (YAGNI - You Ain't Gonna Need It)

NÃO adicione funcionalidades, abstrações ou padrões que não foram explicitamente solicitados na especificação. Comece com a solução mais simples que funciona. Padrões complexos (Repository, Unit of Work, etc.) DEVEM ser justificados com necessidade concreta documentada.

**Rationale**: Complexidade prematura aumenta custo de manutenção, dificulta compreensão e cria overhead desnecessário. YAGNI força foco no problema real, reduz código a manter e facilita mudanças futuras. Abstrações devem emergir de necessidade real, não de especulação.

## Technology Stack Requirements

### Mandatory Technologies

- **Runtime**: .NET 8.0 ou superior (LTS releases preferenciais)
- **Backend Framework**: ASP.NET Core Web API
- **Frontend Framework**: Blazor (inicialmente Blazor Server)
- **ORM**: Entity Framework Core (versão compatível com .NET usado)
- **Database**: PostgreSQL (versão 14+)
- **Unit Testing**: xUnit
- **Mocking** (se necessário): Moq ou NSubstitute
- **Integration Testing**: WebApplicationFactory (Microsoft.AspNetCore.Mvc.Testing)

### Prohibited Practices

- Acesso direto ao banco de dados pelo frontend (Blazor)
- SQL raw queries sem justificativa documentada
- ORMs alternativos sem aprovação explícita
- Uso de .NET Framework (legado) - DEVE ser .NET Core/5+

## Development Workflow

### Implementation Sequence

Para qualquer nova funcionalidade ou mudança de comportamento:

1. **Especificação**: Clarificar requisitos funcionais e critérios de aceitação
2. **Testes Primeiro**: Escrever testes xUnit (unidade) e testes de integração (API endpoints)
3. **Validar Falha**: Executar testes e confirmar falha (Red phase)
4. **Implementar**: Escrever código mínimo para passar testes (Green phase)
5. **Refatorar**: Melhorar design mantendo testes verdes (Refactor phase)
6. **Code Review**: Verificar compliance com constitution e qualidade

### Quality Gates

Antes de considerar qualquer tarefa como completa:

- ✅ Todos os testes unitários passam
- ✅ Todos os testes de integração passam
- ✅ Cobertura de testes adequada para lógica de negócio crítica
- ✅ API endpoints testados via WebApplicationFactory
- ✅ Código segue convenções .NET (PascalCase, async/await, etc.)
- ✅ Nenhuma violação dos princípios constitucionais

## Governance

Esta constituição supersede todas as outras práticas de desenvolvimento. Toda decisão técnica DEVE ser avaliada quanto à conformidade com estes princípios.

### Amendment Process

Mudanças na constituição requerem:

1. Proposta documentada com justificativa clara
2. Análise de impacto em código existente
3. Aprovação explícita do responsável pelo projeto
4. Atualização de version seguindo semântica descrita abaixo
5. Propagação de mudanças para templates e documentação dependentes

### Versioning

A constituição segue versionamento semântico (MAJOR.MINOR.PATCH):

- **MAJOR**: Remoção ou redefinição incompatível de princípios existentes
- **MINOR**: Adição de novos princípios ou seções, expansão material de guidance
- **PATCH**: Clarificações, correções de texto, refinamentos não-semânticos

### Complexity Justification

Violações dos princípios (especialmente YAGNI) DEVEM ser documentadas na seção "Complexity Tracking" do plano de implementação (plan.md) com:

- Descrição da violação
- Justificativa técnica clara da necessidade
- Alternativa mais simples considerada e por que foi rejeitada

### Runtime Development Guidance

Para guidance específico durante desenvolvimento, consulte documentação complementar no repositório (README.md, quickstart.md) que elaboram como aplicar estes princípios na prática.

**Version**: 1.0.0 | **Ratified**: 2025-10-23 | **Last Amended**: 2025-10-23
