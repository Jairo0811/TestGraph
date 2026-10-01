# Phase 11 — Projects + Persistence

## Goal

Phase 11 turns TestGraph from a transient analyzer into a project-based application with durable SQL Server storage.

## Persistence model

The first persistent aggregate is `Project`. Each project can contain multiple `AnalysisRecord` entries.

An analysis stores:

- TGPL source code;
- cyclomatic complexity;
- graph nodes;
- graph edges;
- execution paths;
- structural test cases.

The AST remains transient and is rebuilt from source code when needed.

## Technology

- Entity Framework Core;
- SQL Server;
- ASP.NET Core dependency injection;
- Clean Architecture boundary through Infrastructure.

## Tables

- Projects
- Analyses
- GraphNodes
- GraphEdges
- ExecutionPaths
- TestCases

## API

Phase 11 adds:

- `GET /api/projects`
- `POST /api/projects`
- `GET /api/projects/{id}`
- `POST /api/projects/{id}/analyses`
- `GET /api/projects/{projectId}/analyses/{analysisId}`

Saving an analysis recomputes the deterministic analysis pipeline before persistence so stored graph/path data cannot drift from the saved TGPL source.

## Definition of Done

- [x] Project aggregate.
- [x] Analysis persistence model.
- [x] Graph node/edge persistence.
- [x] Basis-path persistence.
- [x] Test-case persistence schema.
- [x] EF Core DbContext.
- [x] SQL Server provider.
- [x] Project API endpoints.
- [x] Persisted analysis endpoint.
- [x] Source remains the canonical input.

## Next

**Phase 12 — Authentication**
