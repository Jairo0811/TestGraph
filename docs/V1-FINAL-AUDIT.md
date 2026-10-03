# TestGraph V1 — Final Integration Audit

## Context

The first stable tag (`v1.0.0`) validated the repository build and deterministic analysis engine, but a manual browser test exposed an important integration gap: the React UI still rendered the early Phase 5 Scholarship demo while the backend had already reached the complete Phase 0–16 roadmap.

Version `1.0.1` closes that gap.

## Findings and resolution

| Area | Finding in v1.0.0 | Resolution in v1.0.1 |
|---|---|---|
| Header | UI still identified itself as Phase 5 | UI identifies the integrated V1 patch version |
| TGPL editor | No live analysis workspace | Editable TGPL source calls the real backend |
| CFG | Hardcoded 13-node / 3-decision Scholarship graph | CFG comes from `ControlFlowGraphBuilder` through `/api/analysis` |
| Layout | Scholarship-specific node positions | Deterministic generic level layout |
| Complexity | Backend-only | Visible V(G), metrics and three formulations |
| Basis paths | Backend-only | Visible list with selectable path highlighting on CFG |
| Matrix | Derived from static frontend graph | Uses canonical backend matrix result |
| Assisted testing | Hardcoded boundary cards | Uses AST-derived backend suggestions |
| Test Case Designer | Local-only draft list | Can link cases to real basis paths and validate them through the API |
| Academic samples | Hardcoded frontend list | Loaded from `/api/samples`; runnable samples load into editor |
| Authentication | Backend-only | Guest Mode remains default; register/login exposed in UI |
| Projects | Backend-only | Account projects can be created and analyses saved from UI |
| Test-case persistence | Schema existed but UI did not submit cases | Designed cases can be stored with the analysis |
| Local database | No migration or automatic first-run initialization | Development LocalDB uses `EnsureCreatedAsync` for a fresh clone |
| Project JSON | Entity navigation properties could create response cycles | Explicit DTO projections are returned |
| Exports | JSON/CSV were generated from static graph in browser | JSON/CSV/Markdown use the canonical backend report pipeline; PNG captures the live CFG |
| Local API | No fixed development endpoint | API standardised on `http://localhost:5152`; Vite proxies `/api` |
| JWT configuration | Development key was present in source settings | Development key is rejected outside the Development environment |
| Documentation | Claimed Monaco despite it not being installed | README now documents the actual MUI TGPL editor and live integration |

## Deterministic Scholarship validation

The academic report originally documented `V(G)=7`. The executable TGPL representation contains seven binary decision nodes, so TestGraph intentionally reports:

```text
P = 7
V(G) = P + 1 = 8
Basis Paths = 8
```

This difference remains documented rather than modifying the algorithm to reproduce the historical manual count.

## V1.0.1 smoke-test checklist

With the backend on `http://localhost:5152` and the frontend on `http://localhost:5173`:

1. `GET /api/health` returns `status=ok` and version `1.0.1`.
2. Scholarship TGPL analyzes successfully.
3. Complexity shows `V(G)=8` and formulas agree.
4. The CFG contains seven decision nodes for the full Scholarship sample.
5. Eight basis paths are returned and a selected path highlights on the CFG.
6. The adjacency matrix dimensions match the generated CFG node count.
7. Boundary suggestions are returned by the backend rather than a frontend constant.
8. Discount Calculator loads from Academic Samples and returns `V(G)=3`.
9. Numbers Ending in Four loads and analyzes successfully.
10. Matrix academic samples remain visibly disabled with the TGPL V1 limitation documented.
11. A manual test case can be linked to a basis path and validated.
12. JSON, CSV, Markdown and CFG PNG exports complete.
13. Guest analysis works without authentication.
14. Registration/login works with a password of at least eight characters.
15. A project can be created and the current analysis/test cases saved.
16. A saved analysis can be retrieved without JSON reference-cycle errors.
17. Frontend lint/build and backend restore/build/tests pass in CI.

## Remaining V1 limitations

These are deliberate scope boundaries, not release blockers:

- TGPL arrays/matrices are not implemented; two academic samples are preserved references.
- PDF reporting is not part of V1.
- Direct parsing of C#, Java, JavaScript/TypeScript and Python is deferred.
- Runtime coverage instrumentation is not part of V1; structural coverage is modeled from CFG/path associations.
- Production deployments must provide their own SQL Server connection string and JWT signing secret.

## Release criterion

`v1.0.1` is ready to tag only after the pull-request CI and the post-merge `main` CI both pass, followed by the manual smoke test above.
