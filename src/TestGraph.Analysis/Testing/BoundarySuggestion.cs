namespace TestGraph.Analysis.Testing;

public sealed record BoundarySuggestion(
    string Variable,
    string Operator,
    decimal Threshold,
    IReadOnlyList<decimal> Values,
    int? SourceLine);
