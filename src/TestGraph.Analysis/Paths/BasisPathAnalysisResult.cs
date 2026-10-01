namespace TestGraph.Analysis.Paths;

public sealed record BasisPathAnalysisResult(
    int CyclomaticComplexity,
    IReadOnlyList<ExecutionPath> Paths,
    int CandidatePathCount)
{
    public bool IsComplete => Paths.Count == CyclomaticComplexity;
}
