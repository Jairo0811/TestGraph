namespace TestGraph.Analysis.Testing;

public sealed record SuggestedTestCase(
    int Number,
    string Name,
    IReadOnlyDictionary<string, string> Inputs,
    string ExpectedResult,
    TestCaseTechnique Technique,
    string Rationale,
    int? SourceLine);
