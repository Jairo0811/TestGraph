# Phase 15 — QA + Hardening

## Goal

Phase 15 stabilizes TestGraph before the release candidate. It focuses on build quality, API abuse resistance, configuration safety and deployment hygiene rather than adding new analysis features.

## CI quality gates

The CI pipeline now validates:

- README/documentation baseline;
- .NET restore;
- .NET Release build;
- xUnit test suite;
- frontend ESLint;
- frontend TypeScript/Vite production build.

Warnings are already treated as errors through the repository's shared .NET build configuration.

## API hardening

The API adds:

- 1 MiB request-body limit;
- global fixed-window rate limiting;
- configurable CORS origins;
- JWT signing-key minimum length;
- security headers:
  - X-Content-Type-Options;
  - X-Frame-Options;
  - Referrer-Policy;
  - Content-Security-Policy.

## Authorization

Saved projects remain account-owned and project endpoints require authentication. Guest users can still use the stateless analysis endpoints.

## Secret handling

Development defaults are intentionally marked as local placeholders.

Production deployments must override:

- JWT signing key;
- SQL Server connection string;
- allowed CORS origins.

Secrets must not be committed to the repository.

## Security policy

The repository now contains `SECURITY.md` with supported-version and vulnerability-reporting guidance.

## Definition of Done

- [x] Backend build/test gate.
- [x] Frontend lint/build gate.
- [x] Request size limit.
- [x] Rate limiting.
- [x] Configurable CORS.
- [x] JWT key validation.
- [x] Security response headers.
- [x] Account ownership enforced for persisted projects.
- [x] Security policy documented.
- [x] Production secret expectations documented.

## Next

**Phase 16 — Release Candidate**
