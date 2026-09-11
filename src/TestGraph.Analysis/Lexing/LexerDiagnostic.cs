namespace TestGraph.Analysis.Lexing;

public sealed record LexerDiagnostic(
    string Code,
    string Message,
    TextSpan Span);
