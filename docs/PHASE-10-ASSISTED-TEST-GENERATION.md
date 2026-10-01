# Phase 10 — Assisted Test Generation

## Goal

Phase 10 adds deterministic assistance for structural test-case design.

This phase does **not** use generative AI. Suggestions are derived directly from the parsed TGPL AST and its comparison predicates.

## Boundary analysis

The analyzer detects numeric predicates where one side is an identifier and the other is a numeric literal.

Examples:

```text
edad > 18
promedio >= 9
7.5 <= promedio
```

Reversed comparisons are normalized so TestGraph always describes the condition from the variable's perspective.

## Type-aware values

Declared variable type controls the boundary step:

- `Entero` → step `1`;
- `Real` → step `0.01`;
- unknown numeric type → step `0.01`.

For:

```text
edad > 18
```

the generator proposes:

```text
17
18
19
```

For:

```text
promedio >= 9
```

it proposes:

```text
8.99
9
9.01
```

## Suggestions

Each suggested test case includes:

- sequential number;
- generated name;
- input dictionary;
- expected verification description;
- `BoundaryValue` technique;
- rationale;
- source line.

Duplicate variable/value suggestions are removed deterministically.

## API

Phase 10 adds:

```http
POST /api/analysis/test-cases/suggest
```

The endpoint accepts TGPL source and returns detected boundaries plus suggested test cases.

## Frontend

The analysis workspace includes a visual Assisted Test Generation panel demonstrating the Scholarship boundary suggestions.

The deterministic backend remains the source of truth; wiring suggestions to the live editor/API can occur when the project workspace and persistence layers are introduced.

## Definition of Done

- [x] AST predicate traversal.
- [x] Numeric comparison detection.
- [x] Reversed-comparison normalization.
- [x] Integer-aware boundary steps.
- [x] Real-number boundary steps.
- [x] Boundary-value test suggestions.
- [x] Duplicate suppression.
- [x] Source-line traceability.
- [x] Scholarship predicates validated by tests.
- [x] API endpoint.
- [x] Frontend suggestion panel.

## Next

**Phase 11 — Projects + Persistence**
