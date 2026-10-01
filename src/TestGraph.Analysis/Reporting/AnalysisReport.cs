using TestGraph.Analysis.Complexity;
using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Matrix;
using TestGraph.Analysis.Paths;

namespace TestGraph.Analysis.Reporting;

public sealed record AnalysisReport(
    string SourceCode,
    ControlFlowGraph Graph,
    CyclomaticComplexityResult Complexity,
    BasisPathAnalysisResult BasisPaths,
    AdjacencyMatrixResult Matrix);
