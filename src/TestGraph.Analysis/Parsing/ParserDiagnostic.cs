using TestGraph.Analysis.Lexing;

namespace TestGraph.Analysis.Parsing;

public sealed record ParserDiagnostic(
    string Code,
    string Message,
    TextSpan Span);
