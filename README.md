# TestGraph

> **Visualize logic. Discover paths. Design better tests.**

**TestGraph** is an open-source visual white-box testing and control-flow analysis platform. It is designed to transform structured pseudocode into interactive control-flow graphs, calculate cyclomatic complexity, identify linearly independent paths, generate adjacency matrices, and assist with structural test-case design.

TestGraph evolves from a 2024 academic software-engineering project into a modern engineering tool focused on deterministic program-flow analysis.

## Vision

The original academic workflow required students to manually:

1. Read and understand pseudocode.
2. Identify nodes, edges, regions, and predicate nodes.
3. Draw a control-flow graph.
4. Calculate cyclomatic complexity.
5. Determine a basis set of linearly independent paths.
6. Build a graph/adjacency matrix.
7. Design effective test cases.

TestGraph reimagines that workflow as:

```text
Structured Pseudocode
        ↓
      Lexer
        ↓
      Parser
        ↓
       AST
        ↓
Control Flow Graph
        ↓
┌──────────────────────────────┐
│ Cyclomatic Complexity        │
│ Independent Paths            │
│ Adjacency Matrix             │
│ Assisted Test Cases          │
│ Structural Coverage          │
└──────────────────────────────┘
```

## Core V1 Scope

- Structured pseudocode editor
- TGPL lexer and parser
- Abstract Syntax Tree (AST)
- Control Flow Graph (CFG) generation
- Interactive CFG visualization
- Cyclomatic complexity analysis
- Basis path analysis
- Adjacency matrix generation
- Manual and assisted test-case design
- Structural coverage model
- Academic sample projects
- Project persistence
- Export to PNG, CSV, and JSON

## TGPL — TestGraph Pseudocode Language

TestGraph V1 uses a controlled pseudocode language instead of trying to parse arbitrary natural-language pseudocode.

Example:

```text
Entero edad
Real promedio
Real beca

Leer edad
Leer promedio

Si edad > 18 Entonces
    Si promedio >= 9 Entonces
        beca <- 2000
    Sino Si promedio >= 7.5 Entonces
        beca <- 1000
    Sino Si promedio >= 6 Entonces
        beca <- 500
    Sino
        beca <- 0
    Fin Si
Sino
    Si promedio >= 9 Entonces
        beca <- 3000
    Sino Si promedio >= 8 Entonces
        beca <- 2000
    Sino Si promedio >= 6 Entonces
        beca <- 100
    Sino
        beca <- 0
    Fin Si
Fin Si
```

Initial language constructs:

- Variable declarations: `Entero`, `Real`, `Logico`
- Input/output: `Leer`, `Escribir`
- Assignments: `<-`
- Conditions: `Si`, `Sino Si`, `Sino`, `Fin Si`
- Loops: `Mientras`, `Para`
- Operators: `>`, `<`, `>=`, `<=`, `=`, `<>`, `Y`, `O`, `NO`

## Analysis Engine

The deterministic analysis pipeline is the core of TestGraph:

```text
Source Code
    ↓
Lexer
    ↓
Tokens
    ↓
Parser
    ↓
AST
    ↓
CFG Builder
    ↓
ControlFlowGraph
    ↓
Complexity / Paths / Matrix / Tests
```

### Cyclomatic Complexity

For a connected control-flow graph, TestGraph will support the standard equivalent views:

```text
V(G) = E - N + 2
V(G) = P + 1
V(G) = R
```

Where:

- `E` = edges
- `N` = nodes
- `P` = predicate nodes
- `R` = regions

The regions value is treated as the equivalent cyclomatic value rather than depending on a particular visual layout.

### Basis Path Analysis

The number of basis paths corresponds to the cyclomatic complexity:

```text
Basis Paths = V(G)
```

Users will be able to select a path and highlight it directly on the CFG.

### Structural Coverage

V1 will model structural coverage without runtime instrumentation:

- Node Coverage
- Edge Coverage
- Decision Coverage
- Basis Path Coverage

## Academic Sample Projects

The original academic exercises are preserved as sample projects and validation scenarios for the TestGraph engine.

### 1. Matrix Minimum Even
Read a 5x3 integer matrix and determine which column contains the smallest even number.

### 2. Scholarship Calculator
Determine a scholarship amount from age and academic average.

Original analysis:

```text
Nodes: 18
Edges: 23
Predicate Nodes: 6
Regions: 7
Cyclomatic Complexity: 7
```

### 3. Numbers Ending in Four
Read two numbers and display every number ending in `4` between them.

### 4. Find Number 24
Search for the number `24` inside a 4x3 matrix.

### 5. Discount Calculator
Calculate a discount according to price thresholds.

Original rules:

```text
price >= 200      → 15%
price < 100       → 10%
100 <= price < 200 → 12%
```

Original cyclomatic complexity: `3`.

## Technology Stack

### Frontend

- React 19
- TypeScript
- Vite
- React Router
- TanStack Query
- React Flow
- Monaco Editor
- MUI

### Backend

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server

### Architecture

TestGraph will use a **Modular Monolith + Clean Architecture** approach.

```text
TestGraph.sln

src/
├── TestGraph.Domain
├── TestGraph.Application
├── TestGraph.Analysis
├── TestGraph.Infrastructure
└── TestGraph.Api

tests/
├── TestGraph.Domain.Tests
├── TestGraph.Application.Tests
└── TestGraph.Analysis.Tests

web/
└── testgraph-web
```

`TestGraph.Analysis` is the main technical differentiator and will contain:

```text
TestGraph.Analysis
├── Lexing
├── Parsing
├── Syntax
├── ControlFlow
├── Complexity
├── Paths
├── Matrix
└── Testing
```

## Initial Domain Model

### Project

```text
Id
Name
Description
CreatedAt
UpdatedAt
```

### Analysis

```text
Id
ProjectId
SourceCode
Status
CyclomaticComplexity
CreatedAt
```

### GraphNode

```text
Id
AnalysisId
NodeNumber
Type
Label
SourceLine
```

### GraphEdge

```text
Id
AnalysisId
SourceNodeId
TargetNodeId
Condition
```

### ExecutionPath

```text
Id
AnalysisId
PathNumber
NodeSequence
```

### TestCase

```text
Id
AnalysisId
Name
Inputs
ExpectedResult
Technique
LinkedPathId
```

## Authentication Strategy

Initial model:

```text
Guest Mode
+
Optional Account
```

Guests can analyze pseudocode, visualize CFGs, and try samples. Accounts will add saved projects, history, exports, and cloud persistence.

## API Direction

Initial endpoints may include:

```http
POST /api/analysis/parse
POST /api/analysis/control-flow
POST /api/analysis/complexity
POST /api/analysis/paths
POST /api/analysis/test-cases
```

The API can later consolidate analysis into a single endpoint:

```http
POST /api/analysis
```

## Out of Scope for V1

To keep the first release focused, V1 will not include:

- C# source analysis
- Java source analysis
- JavaScript/TypeScript source analysis
- Python source analysis
- GitHub repository analysis
- CI/CD integrations
- Runtime instrumentation
- Mutation testing
- Load testing
- Security testing
- Enterprise static analysis

## Future Direction

### V2 — Real Source Code

The first real programming language target will be **C#**, using Roslyn where appropriate. Later targets may include Java, JavaScript/TypeScript, and Python.

### V3 — Repository Analysis

A future version may analyze repositories and identify complex methods, for example:

```text
CalculatePayment()   CC 18
ProcessInvoice()     CC 14
CreateUser()         CC 7
```

## AI Philosophy

AI is not required for TestGraph's structural analysis.

The following must remain deterministic:

- CFG generation
- Cyclomatic complexity
- Path analysis
- Adjacency matrices

AI may later assist with:

- Explaining graphs and paths
- Suggesting additional test scenarios
- Explaining high complexity
- Suggesting refactoring opportunities

## Product Model

Initial positioning:

- Free
- Open source
- Educational tool
- Portfolio project

A future **TestGraph Cloud** could add private projects, repository analysis, team workspaces, richer reports, CI integrations, history, and analytics.

## Brand Direction

**Primary tagline:**

> Visualize logic. Discover paths. Design better tests.

**Secondary message:**

> From algorithms to better software.

Core concepts:

```text
CONTROL FLOW • COMPLEXITY • PATHS • TEST CASES
```

Visual direction:

- Deep navy
- Electric blue
- Cyan
- White
- Slate gray
- Node-and-edge visual language
- Technical, modern, professional software-engineering aesthetic

## Academic Origins

TestGraph evolved from an academic final project developed for the course **Fundamentos de Ingeniería de Software (ISO-300)** at **Universidad APEC (UNAPEC)** during the **September – December 2024** academic period.

The original assignment focused on:

- Control Flow Graphs
- Cyclomatic Complexity
- Linearly Independent Paths
- Graph Matrices
- Test Case Design

### Original Team

- **Francis Jairo Matias Rosario** — A00115261
- **Diego Jose Montero Almonte** — A00115699
- **Robinson Junior Novo Lopez** — A00115885
- **Angel Emmanuel Gonzalez Acosta** — A00116360
- **Christian Rainel Menendez Hiciano** — A00116551

### Course Information

- **Course:** Fundamentos de Ingeniería de Software (ISO-300)
- **Professor:** Leandro Eduardo Fondeur Gil
- **Academic Period:** September – December 2024
- **Institution:** Universidad APEC (UNAPEC)
- **Original Group:** #4
- **Original Submission Date:** December 5, 2024

## Evolution into TestGraph

The original project required the team to manually build control-flow graphs, calculate cyclomatic complexity, identify independent execution paths, create graph matrices, and design test cases.

TestGraph preserves those exercises as sample projects and validation scenarios while transforming the original academic workflow into an interactive engineering platform.

## Roadmap

- [x] **Phase 0 — Product Definition & Technical Foundation**
- [ ] **Phase 1 — Solution Foundation**
- [ ] **Phase 2 — TGPL Lexer**
- [ ] **Phase 3 — TGPL Parser + AST**
- [ ] **Phase 4 — Control Flow Graph Engine**
- [ ] **Phase 5 — CFG Visualization**
- [ ] **Phase 6 — Cyclomatic Complexity**
- [ ] **Phase 7 — Basis Path Analysis**
- [ ] **Phase 8 — Adjacency Matrix**
- [ ] **Phase 9 — Test Case Designer**
- [ ] **Phase 10 — Assisted Test Generation**
- [ ] **Phase 11 — Projects + Persistence**
- [ ] **Phase 12 — Authentication**
- [ ] **Phase 13 — Academic Samples**
- [ ] **Phase 14 — Exports & Reports**
- [ ] **Phase 15 — QA + Hardening**
- [ ] **Phase 16 — Release Candidate**

## Current Status

**Phase 0 is complete.**

TestGraph is now formally defined as a visual control-flow and white-box testing platform rather than a collection of recreated academic exercises.

The next implementation milestone is **Phase 1 — Solution Foundation**.
