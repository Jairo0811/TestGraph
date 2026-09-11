# Phase 1 — Solution Foundation

## Status

Implemented on `feature/solution-foundation`.

## Goal

Establish the compile-time and repository foundation for TestGraph before implementing TGPL language analysis.

## Backend structure

```text
TestGraph.sln
src/
├── TestGraph.Domain
├── TestGraph.Application
├── TestGraph.Analysis
├── TestGraph.Infrastructure
└── TestGraph.Api

tests/
├── TestGraph.Domain.Tests
├── TestGraph.Application.Tests
└── TestGraph.Analysis.Tests
```

### Responsibilities

- `TestGraph.Domain`: domain concepts and business invariants.
- `TestGraph.Application`: application use cases and orchestration.
- `TestGraph.Analysis`: deterministic TGPL, CFG, complexity, paths, matrices and test-generation engine.
- `TestGraph.Infrastructure`: persistence and external infrastructure implementations.
- `TestGraph.Api`: ASP.NET Core HTTP host.

The dependency direction is intentionally inward. `Domain` has no project dependencies. `Application` depends on `Domain`; `Analysis` depends on `Domain`; `Infrastructure` depends on `Application` and `Domain`; `Api` composes the application.

## .NET baseline

- .NET 10
- nullable reference types enabled
- implicit usings enabled
- warnings treated as errors
- latest C# language version
- SDK pinned through `global.json`

The API exposes a first smoke-test endpoint:

```http
GET /api/health
```

Expected payload:

```json
{
  "service": "TestGraph.Api",
  "status": "ok",
  "version": "0.1.0"
}
```

CORS is initially configured for the local Vite development host on `http://localhost:5173`.

## Frontend structure

```text
frontend/
└── testgraph-web/
    ├── src/
    │   ├── App.tsx
    │   └── main.tsx
    ├── index.html
    ├── package.json
    ├── tsconfig.json
    └── vite.config.ts
```

Baseline stack:

- React 19
- TypeScript
- Vite
- Material UI
- TanStack Query
- React Flow / XYFlow
- React Router

The initial shell establishes the TestGraph visual identity and reserves the product modules that will be implemented in later phases.

## Local startup

Backend:

```bash
dotnet restore
dotnet build TestGraph.sln
dotnet run --project src/TestGraph.Api
```

Frontend:

```bash
cd frontend/testgraph-web
npm install
npm run dev
```

## Definition of Done

Phase 1 is considered complete when:

- the solution structure exists;
- Clean Architecture dependency boundaries are established;
- the API host exists and provides a health endpoint;
- test projects exist for Domain, Application and Analysis;
- the React 19 + TypeScript + Vite frontend shell exists;
- common repository ignores and SDK settings are configured;
- the project is ready to begin Phase 2 without restructuring the foundation.

## Next phase

**Phase 2 — TGPL Lexer**

The next implementation should introduce the lexical model (`Token`, `TokenType`, source positions, diagnostics) and a deterministic lexer for the initial TestGraph Pseudocode Language grammar.
