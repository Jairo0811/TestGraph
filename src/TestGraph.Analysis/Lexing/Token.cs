namespace TestGraph.Analysis.Lexing;

public sealed record Token(
    TokenKind Kind,
    string Lexeme,
    string? Literal,
    TextSpan Span);
