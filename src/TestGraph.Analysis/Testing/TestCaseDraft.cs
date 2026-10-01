namespace TestGraph.Analysis.Testing;

public sealed record TestCaseDraft(
    string Name,
    IReadOnlyDictionary<string, string> Inputs,
    string ExpectedResult,
    TestCaseTechnique Technique,
    int? LinkedPathNumber = null);
