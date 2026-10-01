namespace TestGraph.Analysis.Samples;

public sealed record AcademicSample(
    string Id,
    string Name,
    string OriginalAuthor,
    string Description,
    string SourceCode,
    int? ExpectedCyclomaticComplexity,
    bool AnalyzerReady,
    string? Limitation = null);
