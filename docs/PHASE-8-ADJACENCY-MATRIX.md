# Phase 8 — Adjacency Matrix

## Goal

Phase 8 generates the adjacency matrix directly from the deterministic Control Flow Graph.

## Matrix definition

For ordered CFG nodes `N1 ... Nn`:

```text
M[i,j] = number of directed edges from Ni to Nj
```

Most TestGraph CFG entries are `0` or `1`, but the representation intentionally supports counts greater than one if parallel edges are introduced later.

## Determinism

Rows and columns are ordered by deterministic node ID.

The same graph therefore produces the same matrix ordering across analysis runs.

## Output

`AdjacencyMatrixResult` provides:

- ordered node IDs;
- matrix rows;
- indexed access;
- CSV serialization.

## API

Phase 8 adds:

```http
POST /api/analysis/matrix
```

The API response includes node IDs and matrix rows.

## Frontend

The analysis workspace includes a scrollable matrix table generated from the current CFG sample. It uses the same node ordering as the backend contract.

## Definition of Done

- [x] Matrix derives from CFG edges.
- [x] Stable node ordering.
- [x] Directed connections represented correctly.
- [x] Loop back edges represented.
- [x] Conditional branches represented.
- [x] CSV representation available.
- [x] API endpoint available.
- [x] Frontend matrix viewer available.
- [x] Unit tests cover linear, conditional and loop graphs.

## Next

**Phase 9 — Test Case Designer**
