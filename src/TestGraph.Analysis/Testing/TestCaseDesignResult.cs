namespace TestGraph.Analysis.Testing;

public sealed record TestCaseDesignResult(
    IReadOnlyList<StructuralTestCase> TestCases,
    IReadOnlyList<TestCaseDesignDiagnostic> Diagnostics)
{
    public bool IsValid => Diagnostics.Count == 0;
}
