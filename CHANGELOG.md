# Changelog

All notable changes to TestGraph are documented here.

## [1.0.1] - 2026-10-03

### Fixed

- replaces the Phase 5 static frontend demo with a live TGPL analysis workspace backed by `POST /api/analysis`;
- renders the real CFG, cyclomatic complexity, basis paths, adjacency matrix and assisted test suggestions returned by the backend;
- replaces the Scholarship hardcoded 13-node/3-decision graph with the deterministic CFG generated from the complete TGPL sample;
- adds a generic deterministic graph layout for arbitrary supported TGPL control flow;
- connects the Test Case Designer to basis paths and backend structural validation;
- exposes Guest Mode plus account registration/login, project creation and analysis persistence in the frontend;
- initializes the LocalDB schema automatically in Development so authentication and projects work on a fresh local clone;
- standardizes the local API URL at `http://localhost:5152` and adds the Vite `/api` proxy;
- makes JSON, CSV and Markdown exports use the canonical backend analysis pipeline;
- avoids persistence JSON reference cycles by returning explicit DTO projections;
- allows persisted analyses to include structural test cases;
- validates authentication inputs and rejects the development JWT signing key outside Development;
- updates the UI and API version to `1.0.1`;
- restores the transparent TestGraph logo after the final browser smoke test;
- adds the responsive branded application shell and mobile-friendly analysis panels;
- adds the TestGraph isotipo as `favicon.ico` and finalizes browser metadata.

### Status

- TestGraph V1 is frozen at `v1.0.1` after the final integration, responsive UI, asset and CI review;
- future changes to the frozen V1 line are limited to critical defects, security fixes or compatibility maintenance.

## [1.0.0] - 2026-10-02

### Stable release

- promotes the validated `v1.0.0-rc.1` codebase to the first stable TestGraph V1 release;
- preserves the complete Phase 0–16 roadmap implementation;
- confirms green backend restore/build/tests and frontend install/lint/build gates;
- keeps the Scholarship Calculator academic `V(G)=7` value as historical documentation while the deterministic executable TGPL flow correctly evaluates to `V(G)=8`;
- retains the documented V1 limitations for matrix/array syntax and PDF reporting.

## [1.0.0-rc.1] - 2026-09-30

### Added

- TGPL lexer, parser and AST.
- deterministic Control Flow Graph engine.
- interactive CFG visualization.
- cyclomatic complexity analysis.
- basis path analysis.
- adjacency matrix generation.
- manual structural Test Case Designer.
- assisted boundary-value test generation.
- project persistence with EF Core and SQL Server.
- Guest Mode + optional JWT accounts.
- account-owned saved projects.
- five preserved ISO-300 academic samples.
- JSON, CSV, PNG and Markdown exports/reports.
- CI for backend tests, frontend lint and production build.
- API hardening, rate limiting, security headers and security policy.

### Known limitations

- TGPL V1 does not yet support array/matrix syntax, so two preserved academic matrix exercises are reference samples rather than executable TGPL V1 samples.
- PDF report generation remains post-V1.
- production deployment configuration and secrets must be supplied outside source control.
