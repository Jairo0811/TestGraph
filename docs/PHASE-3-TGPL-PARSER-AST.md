# Phase 3 — TGPL Parser + AST

## Goal

Phase 3 turns the token stream produced by the TGPL lexer into a deterministic Abstract Syntax Tree (AST). This is the semantic structure that later phases will use to build the Control Flow Graph.

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
CFG Builder (Phase 4)
```

## Supported statements

The initial parser supports:

- variable declarations: `Entero`, `Real`, `Logico`;
- input: `Leer`;
- output: `Escribir` with comma-separated expressions;
- assignments using `<-`;
- `Si / Sino Si / Sino / Fin Si`;
- `Mientras / Hacer / Fin Mientras`;
- `Para / Hasta / Paso / Fin Para`;
- optional `Programa <nombre>` header;
- optional `Variables` section marker.

## Expressions

Expressions use deterministic precedence:

| Precedence | Operators |
|---:|---|
| 1 | `O` |
| 2 | `Y` |
| 3 | `=`, `<>`, `>`, `>=`, `<`, `<=` |
| 4 | `+`, `-` |
| 5 | `*`, `/`, `%` |
| 6 | unary `NO`, `+`, `-` |

Parenthesized expressions are supported.

## AST model

The syntax tree contains dedicated nodes for:

- `CompilationUnitSyntax`;
- `VariableDeclarationSyntax`;
- `ReadStatementSyntax`;
- `WriteStatementSyntax`;
- `AssignmentStatementSyntax`;
- `IfStatementSyntax` and `ElseIfClauseSyntax`;
- `WhileStatementSyntax`;
- `ForStatementSyntax`;
- literal, name, unary, binary and parenthesized expressions;
- recoverable error nodes.

Every node carries a source `TextSpan` so future UI diagnostics can map parser structures back to the editor.

## Diagnostics

Parser diagnostics introduced in this phase:

- `TGPL100` — unexpected token at the start of a statement;
- `TGPL101` — expected token is missing;
- `TGPL102` — expected expression;
- `TGPL103` — expected end of line.

Lexer diagnostics are preserved in `ParserResult` instead of being discarded.

## Recovery

The parser uses line-oriented recovery because TGPL is an educational pseudocode language with explicit statement boundaries. When malformed input is encountered, it advances to the next line or block boundary and continues building as much of the AST as possible.

This behavior is important for the future Monaco editor experience: users should receive useful diagnostics without losing the rest of the analysis.

## Current limitations

Arrays/matrix indexing and procedure/function calls are intentionally deferred. They are not required for the Phase 3 parser contract and can be added before the academic sample phase where needed.

## Definition of Done

- [x] Parser consumes the Phase 2 token stream.
- [x] AST model is separated from lexical tokens.
- [x] Variable declarations, I/O and assignments parse.
- [x] If/else-if/else blocks parse.
- [x] While loops parse.
- [x] For loops parse.
- [x] Operator precedence is deterministic.
- [x] Parenthesized, unary and boolean expressions parse.
- [x] Parser diagnostics include source spans.
- [x] Lexer diagnostics survive the parser boundary.
- [x] Automated tests cover the academic Scholarship-style flow and core grammar.

## Next

**Phase 4 — Control Flow Graph Engine**

The CFG builder will traverse this AST and convert structured statements into deterministic graph nodes and edges.
