# Phase 14 — Exports & Reports

## Goal

Phase 14 makes TestGraph analysis portable outside the application.

## V1 exports

The implemented V1 export set is:

- CFG image as PNG from the interactive frontend;
- adjacency matrix as CSV;
- analysis data as JSON;
- deterministic Markdown analysis report.

## Backend report model

`AnalysisReportBuilder` recomputes the canonical deterministic pipeline:

```text
TGPL → Parser → CFG → Complexity → Basis Paths → Matrix
```

It then produces a single report model that can be serialized or formatted.

## API

- `POST /api/analysis/export/json`
- `POST /api/analysis/export/matrix.csv`
- `POST /api/analysis/report/markdown`

## Frontend

The export panel supports JSON, matrix CSV and current CFG PNG capture.

## PDF

PDF remains a planned post-V1/reporting enhancement as originally scoped. Phase 14 deliberately avoids introducing a PDF renderer into the deterministic analysis engine.

## Definition of Done

- [x] JSON export.
- [x] CSV matrix export.
- [x] CFG PNG export.
- [x] Structured report model.
- [x] Markdown report.
- [x] API export endpoints.
- [x] Frontend export controls.
- [x] Report tests.

## Next

**Phase 15 — QA + Hardening**
