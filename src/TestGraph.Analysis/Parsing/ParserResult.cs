using TestGraph.Analysis.Lexing;
using TestGraph.Analysis.Syntax;

namespace TestGraph.Analysis.Parsing;

public sealed record ParserResult(
    CompilationUnitSyntax Root,
    IReadOnlyList<ParserDiagnostic> Diagnostics,
    IReadOnlyList<LexerDiagnostic> LexerDiagnostics)
{
    public bool HasErrors => Diagnostics.Count > 0 || LexerDiagnostics.Count > 0;
}
