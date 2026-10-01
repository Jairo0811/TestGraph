namespace TestGraph.Analysis.Testing;

public sealed record AssistedTestGenerationResult(
    IReadOnlyList<BoundarySuggestion> Boundaries,
    IReadOnlyList<SuggestedTestCase> TestCases);
