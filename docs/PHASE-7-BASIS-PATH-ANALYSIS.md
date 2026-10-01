# Phase 7 — Basis Path Analysis

## Goal

Phase 7 derives a deterministic basis set of linearly independent execution paths from the Control Flow Graph.

The target number of selected paths is the graph's cyclomatic complexity:

```text
Basis Paths = V(G)
```

## Strategy

TestGraph uses a two-step deterministic approach:

1. enumerate bounded executable entry-to-exit candidate paths;
2. encode every candidate as an edge-incidence vector and select linearly independent vectors over GF(2).

For loops, every back edge may be traversed at most once during candidate generation. This produces the educationally useful zero-iteration and one-iteration variants without creating infinite path enumeration.

## Path model

Each `ExecutionPath` stores:

- path number;
- ordered node IDs;
- ordered edge indexes;
- display form such as `1 → 2 → 4 → 7`.

## Linear independence

Candidate paths are represented as binary edge vectors. Gaussian-style elimination over GF(2) rejects candidates that can be expressed as a combination of paths already selected.

The analyzer stops when it has selected `V(G)` independent paths.

## Determinism

Candidate traversal order is stable:

1. Normal;
2. True;
3. False;
4. Back.

Node IDs and edge indexes are already deterministic from Phase 4, so repeated analysis returns the same basis ordering.

## Definition of Done

- [x] Entry-to-exit candidate enumeration.
- [x] Loop-safe bounded traversal.
- [x] Edge-vector representation.
- [x] Linear-independence selection.
- [x] Path count targets cyclomatic complexity.
- [x] Straight-line validation.
- [x] Conditional validation.
- [x] Loop validation.
- [x] Scholarship Calculator executable TGPL flow validates eight basis paths, matching its deterministic V(G)=8.
- [x] Deterministic ordering.

## Next

**Phase 8 — Adjacency Matrix**
