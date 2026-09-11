namespace TestGraph.Analysis.Lexing;

public sealed record LexerResult(
    IReadOnlyList<Token> Tokens,
    IReadOnlyList<LexerDiagnostic> Diagnostics)
{
    public bool HasErrors => Diagnostics.Count > 0;
}
