# Phase 2 — TGPL Lexer

## Goal

Implement the deterministic lexical-analysis layer for **TGPL (TestGraph Pseudocode Language)**. The lexer converts raw pseudocode into a token stream that Phase 3 can consume without interpreting control-flow semantics yet.

## Pipeline position

```text
TGPL Source
    ↓
  Lexer   ← Phase 2
    ↓
 Tokens
    ↓
  Parser  ← Phase 3
    ↓
   AST
```

## Supported lexical elements

### Keywords

```text
Programa
Variables
Entero
Real
Logico / Lógico
Leer
Escribir
Si
Entonces
Sino
Fin
Mientras
Hacer
Para
Hasta
Paso
Y
O
NO
Verdadero
Falso
```

Keywords are case-insensitive.

### Literals

- integer numbers: `18`, `2000`;
- decimal numbers using a dot: `7.5`, `9.25`;
- double-quoted strings: `"Digite un valor"`;
- supported string escapes: `\\n`, `\\r`, `\\t`, `\\"`, `\\\\`.

### Operators

```text
<-  =  <>  >  >=  <  <=
+   -  *   /  %
```

### Punctuation

```text
( ) , : ;
```

### Comments

Both line-comment forms are accepted:

```text
# comentario
// comentario
```

New lines remain explicit tokens so the future parser can use statement boundaries when useful.

## Source tracking

Every token contains a `TextSpan` with start/end positions:

- absolute offset;
- 1-based line;
- 1-based column.

This provides the foundation for editor diagnostics and Monaco integration later in the roadmap.

## Diagnostics

| Code | Meaning |
|---|---|
| `TGPL001` | Unexpected character |
| `TGPL002` | Unterminated string literal |

The lexer is recovery-oriented: an invalid character generates a diagnostic and scanning continues whenever possible.

## Main types

```text
Lexing/
├── Lexer.cs
├── LexerResult.cs
├── LexerDiagnostic.cs
├── Token.cs
├── TokenKind.cs
├── TextPosition.cs
└── TextSpan.cs
```

## Design decisions

- Lexing is deterministic and has no AI dependency.
- Numeric conversion is intentionally deferred to parsing/semantic analysis; the lexer preserves number text as a literal.
- `Fin Si` and `Sino Si` are represented as separate keyword tokens instead of compound tokens.
- Decimal comma is not supported because comma is punctuation in TGPL; decimals use `.`.
- Keywords are Spanish because TGPL preserves the terminology used by the original UNAPEC pseudocode exercises.

## Automated tests

`LexerTests` covers:

- the Scholarship Calculator sample shape;
- case-insensitive keywords;
- assignment/comparison/arithmetic operators;
- integer and decimal literals;
- escaped strings;
- `#` and `//` comments;
- line/column tracking;
- unexpected-character recovery;
- unterminated-string diagnostics.

## Definition of Done

- [x] Token model defined.
- [x] Keyword vocabulary defined.
- [x] Operators and punctuation tokenized.
- [x] Integer/decimal literals supported.
- [x] Strings and basic escapes supported.
- [x] Source spans tracked.
- [x] Comments supported.
- [x] Diagnostics implemented.
- [x] Lexer recovery implemented for invalid characters.
- [x] Automated lexer tests added.

## Next

**Phase 3 — TGPL Parser + AST** will consume this token stream and produce a structured syntax tree for declarations, I/O, assignments, conditionals and loops.
