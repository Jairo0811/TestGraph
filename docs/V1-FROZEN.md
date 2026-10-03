# TestGraph V1 — Frozen State

TestGraph V1 is frozen at version `1.0.1` as of **2026-10-03**.

## Frozen baseline

- Product roadmap: Phase 0 through Phase 16 complete.
- Backend: .NET 10 / ASP.NET Core / EF Core / SQL Server.
- Frontend: React 19 / TypeScript / Vite / MUI / React Flow.
- TGPL analysis pipeline: lexer, parser, AST, CFG, cyclomatic complexity, basis paths, adjacency matrix and structural test support.
- Guest analysis plus optional JWT account/project persistence.
- Academic samples preserved from ISO-300.
- JSON, CSV, PNG and Markdown exports.
- Responsive V1 interface with TestGraph logo and favicon assets.

## Validation at freeze

- latest `main` CI before the freeze passed documentation validation, .NET restore/build/tests and frontend install/lint/build;
- frontend CI installation reported **0 npm vulnerabilities**;
- `GET /api/health` was manually verified with `status=ok` and version `1.0.1`;
- the integrated browser interface and responsive branded shell were manually reviewed;
- the final TestGraph transparent logo is stored in `frontend/testgraph-web/public/testgraph-logo.png`.

## Maintenance policy

V1 receives no planned feature development while frozen. Changes should be limited to:

- critical defects;
- security fixes;
- dependency/runtime compatibility fixes;
- corrections required to keep the documented V1 behavior working.

New language parsers, repository analysis, PDF reporting, runtime instrumentation and other major capabilities belong to a future V2 or later roadmap.

## Deliberate V1 limitations

- TGPL arrays and matrices are not supported, so two academic matrix samples remain preserved references.
- Direct C#, Java, JavaScript/TypeScript and Python parsing is deferred.
- Runtime coverage instrumentation is not part of V1.
- Production deployments must provide external SQL Server and JWT secret configuration.

The immutable release marker for this frozen baseline should be the annotated Git tag `v1.0.1` after the freeze commit is merged to `main` and its post-merge CI passes.
