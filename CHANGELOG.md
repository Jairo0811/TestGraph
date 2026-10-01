# Changelog

All notable changes to TestGraph are documented here.

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
