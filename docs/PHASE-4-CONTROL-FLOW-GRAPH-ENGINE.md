# Phase 4 — Control Flow Graph Engine

## Goal

Phase 4 converts the TGPL Abstract Syntax Tree into a deterministic Control Flow Graph (CFG). The graph is the structural foundation for every later analysis in TestGraph: visualization, cyclomatic complexity, basis paths, adjacency matrices and structural test coverage.

## Pipeline

```text
TGPL Source
    ↓
Lexer
    ↓
Tokens
    ↓
Parser
    ↓
AST
    ↓
ControlFlowGraphBuilder
    ↓
Nodes + Edges
```

## Graph model

### Node kinds

- `Entry` — unique graph entry;
- `Exit` — unique graph exit;
- `Statement` — declaration, I/O, assignment, loop initialization/increment;
- `Decision` — conditional or loop predicate;
- `Merge` — explicit join point after conditional branches.

### Edge kinds

- `Normal` — sequential execution;
- `True` — predicate succeeds;
- `False` — predicate fails;
- `Back` — loop back-edge.

Branch edges use Spanish display labels (`Sí`, `No`, `Volver`) so the graph can later be rendered directly in the UI without recomputing semantics.

## Supported control structures

### Sequential statements

Declarations, reads, writes and assignments are connected linearly.

### If / Else If / Else

Each predicate becomes a `Decision` node. All successful/alternative branches converge into a shared `Merge` node.

### While

A while predicate becomes a `Decision` node. The true branch enters the loop body, the body returns using a `Back` edge, and the false branch becomes the outgoing continuation.

### For

A for loop is expanded into:

```text
initializer
    ↓
 decision ──false──→ next
    │
   true
    ↓
   body
    ↓
 increment
    └────back────→ decision
```

The initial TGPL contract assumes ascending loops using `<=`. More advanced loop direction semantics can be added when TGPL gains richer for-loop syntax.

## Determinism

Node IDs are assigned in a stable traversal order starting at 1. Building the same AST twice produces the same node IDs, labels and edge sequence.

This property is required because later phases will persist path references, highlight nodes in React Flow and compare graph-derived test coverage.

## Source mapping

Every graph node retains the originating AST `TextSpan` whenever possible. This creates the bridge between:

- Monaco editor source lines;
- AST nodes;
- CFG nodes;
- future diagnostics and coverage highlighting.

## Definition of Done

- [x] Explicit CFG node and edge models.
- [x] Unique entry and exit nodes.
- [x] Sequential statement flow.
- [x] If/else-if/else branching.
- [x] Explicit merge nodes.
- [x] While-loop back edges.
- [x] For-loop initializer, decision, increment and back edge.
- [x] Nested control structures.
- [x] Deterministic node IDs and edge order.
- [x] Source-span preservation.
- [x] Unit tests for linear, conditional, loop and scholarship-style flows.

## Next

**Phase 5 — CFG Visualization**

The frontend will render this deterministic graph using React Flow/XYFlow with node types, branch labels, zoom/pan and path highlighting foundations.
