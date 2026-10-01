# Phase 5 — CFG Visualization

## Goal

Phase 5 introduces the first interactive visual representation of the deterministic Control Flow Graph produced by the analysis engine.

The frontend uses **React Flow / XYFlow** to render nodes and edges while keeping the graph model independent from the visualization library.

## Architecture

```text
ControlFlowGraph DTO
        ↓
toReactFlowElements()
        ↓
React Flow Nodes + Edges
        ↓
ControlFlowGraphViewer
```

The visualization layer does not recompute control-flow semantics. It receives an already-built graph and focuses exclusively on presentation and interaction.

## Node representation

Custom CFG nodes visually distinguish:

- Entry;
- Exit;
- Statement;
- Decision;
- Merge.

Each node can display its source-line mapping when available.

## Edge representation

Edges are visually distinguished by semantic kind:

- Normal;
- True / Sí;
- False / No;
- Back / Volver.

Back edges can be animated to make loop behavior easier to recognize.

## Interaction

The viewer provides:

- zoom;
- pan;
- fit-to-view;
- minimap;
- draggable nodes;
- node selection;
- connected-edge highlighting;
- selection details;
- graph metrics summary;
- semantic legend.

Selecting a node highlights its incoming and outgoing edges. This forms the interaction foundation that Phase 7 can later reuse for full basis-path highlighting.

## Current data source

Until an API endpoint exposes CFG results, Phase 5 includes a typed Scholarship sample graph in the frontend. The viewer already consumes a neutral `ControlFlowGraphDto`, so replacing the sample with API data will not require rewriting the visualization component.

## Design principles

- visualization is separate from graph semantics;
- React Flow IDs are derived from deterministic backend node IDs;
- semantic colors are consistent across nodes and edges;
- source-line information is preserved for future editor synchronization;
- the viewer remains reusable for academic samples and persisted analyses.

## Definition of Done

- [x] Typed frontend CFG contract.
- [x] React Flow integration.
- [x] Custom CFG node renderer.
- [x] Entry, exit, statement, decision and merge styling.
- [x] True, false, normal and back-edge styling.
- [x] Zoom, pan, fit view, minimap and controls.
- [x] Node selection and connected-edge highlighting.
- [x] Node detail panel.
- [x] Source-line display.
- [x] Responsive analysis workspace.
- [x] Frontend build added to CI.
- [x] Sample Scholarship graph validates the viewer contract.

## Next

**Phase 6 — Cyclomatic Complexity**

The next phase will compute graph metrics from the deterministic CFG and expose the equivalent cyclomatic-complexity formulations used by TestGraph.
