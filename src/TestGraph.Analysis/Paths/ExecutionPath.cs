namespace TestGraph.Analysis.Paths;

public sealed record ExecutionPath(
    int Number,
    IReadOnlyList<int> NodeIds,
    IReadOnlyList<int> EdgeIndexes)
{
    public string Display => string.Join(" → ", NodeIds);
}
