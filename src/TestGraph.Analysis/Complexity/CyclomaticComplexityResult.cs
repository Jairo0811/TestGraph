namespace TestGraph.Analysis.Complexity;

public sealed record CyclomaticComplexityResult(
    int NodeCount,
    int EdgeCount,
    int PredicateNodeCount,
    int ConnectedComponentCount,
    int EdgeNodeComplexity,
    int PredicateComplexity,
    int RegionCount,
    ComplexityLevel Level)
{
    public int Value => EdgeNodeComplexity;

    public bool FormulasAgree =>
        EdgeNodeComplexity == PredicateComplexity &&
        PredicateComplexity == RegionCount;
}
