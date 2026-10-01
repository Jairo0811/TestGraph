# Phase 9 — Test Case Designer

## Goal

Phase 9 introduces the manual structural test-case design workflow.

## Model

Each test case contains:

- sequential number;
- name;
- input values;
- expected result;
- testing technique;
- optional linked basis-path number;
- covered node IDs;
- covered edge indexes.

## Techniques

Supported classifications:

- Manual;
- BoundaryValue;
- EquivalencePartition;
- BranchCoverage;
- PathCoverage;
- InvalidInput.

## Path linkage

When a test case is linked to a basis path, TestGraph automatically copies the path's node and edge coverage into the designed case.

This prepares the data model for future structural coverage metrics without requiring runtime instrumentation.

## Validation

The designer reports diagnostics for:

- missing name;
- missing expected result;
- unknown basis-path references.

## API

Phase 9 adds:

```http
POST /api/analysis/test-cases/design
```

The endpoint accepts TGPL source plus user-authored test-case drafts and returns normalized structural test cases.

## Frontend

The analysis workspace now contains an interactive manual Test Case Designer where users can:

- name a case;
- document input values;
- define the expected result;
- choose a testing technique;
- build a local case list.

Persistence is intentionally deferred to Phase 11.

## Definition of Done

- [x] Structural test-case model.
- [x] Technique classification.
- [x] Manual drafts.
- [x] Validation diagnostics.
- [x] Optional basis-path linkage.
- [x] Structural node/edge coverage captured from linked paths.
- [x] API endpoint.
- [x] Interactive frontend designer.
- [x] Unit tests.

## Next

**Phase 10 — Assisted Test Generation**
