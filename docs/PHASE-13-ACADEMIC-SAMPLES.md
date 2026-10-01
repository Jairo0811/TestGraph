# Phase 13 — Academic Samples

## Goal

Phase 13 preserves the five exercises from the original ISO-300 white-box-testing assignment as first-class TestGraph samples.

## Included exercises

1. Matrix Minimum Even — Angel Emmanuel Gonzalez Acosta.
2. Scholarship Calculator — Francis Jairo Matias Rosario.
3. Numbers Ending in Four — Robinson Junior Novo Lopez.
4. Find Number 24 — Christian Rainel Menendez Hiciano.
5. Discount Calculator — Diego Jose Montero Almonte.

## Analyzer readiness

TGPL V1 deliberately excludes array/matrix syntax.

Therefore:

- Scholarship Calculator — analyzer ready;
- Numbers Ending in Four — analyzer ready;
- Discount Calculator — analyzer ready;
- Matrix Minimum Even — preserved, matrix syntax pending;
- Find Number 24 — preserved, matrix syntax pending.

The limitation is explicit rather than silently rewriting the original matrix exercises into different algorithms.

## Validation

Analyzer-ready samples are parsed and analyzed in automated tests.

The Scholarship Calculator validates V(G)=7.
The Discount Calculator validates V(G)=3.

The Numbers Ending in Four sample is analyzed from the generated CFG. The original academic document contains an internal inconsistency in its complexity calculation, so TestGraph treats its own deterministic CFG result as the executable sample result.

## API

- GET /api/samples
- GET /api/samples/{id}

## Definition of Done

- [x] Five original exercises represented.
- [x] Original authors preserved.
- [x] Analyzer-ready TGPL samples.
- [x] Explicit matrix-language limitation.
- [x] Automated validation for runnable samples.
- [x] Sample API.
- [x] Frontend sample overview.

## Next

**Phase 14 — Exports & Reports**
