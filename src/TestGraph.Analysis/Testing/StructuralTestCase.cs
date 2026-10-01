namespace TestGraph.Analysis.Testing;

public sealed record StructuralTestCase(
    int Number,
    string Name,
    IReadOnlyDictionary<string, string> Inputs,
    string ExpectedResult,
    TestCaseTechnique Technique,
    int? LinkedPathNumber,
    IReadOnlyList<int> CoveredNodeIds,
    IReadOnlyList<int> CoveredEdgeIndexes);
