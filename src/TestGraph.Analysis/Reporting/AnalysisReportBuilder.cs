using TestGraph.Analysis.Complexity;
using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Matrix;
using TestGraph.Analysis.Parsing;
using TestGraph.Analysis.Paths;

namespace TestGraph.Analysis.Reporting;

public sealed class AnalysisReportBuilder
{
    public AnalysisReport Build(string sourceCode)
    {
        if (string.IsNullOrWhiteSpace(sourceCode))
        {
            throw new ArgumentException("Source code is required.", nameof(sourceCode));
        }

        var parse = Parser.Parse(sourceCode);
        if (parse.HasErrors)
        {
            throw new InvalidOperationException("Cannot build report from invalid TGPL source.");
        }

        var graph = new ControlFlowGraphBuilder().Build(parse.Root);
        var complexity = new CyclomaticComplexityAnalyzer().Analyze(graph);
        var paths = new BasisPathAnalyzer().Analyze(graph);
        var matrix = new AdjacencyMatrixBuilder().Build(graph);

        return new AnalysisReport(sourceCode, graph, complexity, paths, matrix);
    }

    public string ToMarkdown(AnalysisReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var lines = new List<string>
        {
            "# TestGraph Analysis Report",
            "",
            "## Summary",
            "",
            $"Cyclomatic Complexity: **{report.Complexity.Value}**",
            $"Nodes: **{report.Complexity.NodeCount}**",
            $"Edges: **{report.Complexity.EdgeCount}**",
            $"Predicate Nodes: **{report.Complexity.PredicateNodeCount}**",
            $"Basis Paths: **{report.BasisPaths.Paths.Count}**",
            "",
            "## Source",
            "",
            "```text",
            report.SourceCode,
            "```",
            "",
            "## Basis Paths",
            ""
        };

        foreach (var path in report.BasisPaths.Paths)
        {
            lines.Add($"- Path {path.Number}: {path.Display}");
        }

        lines.Add("");
        lines.Add("## Adjacency Matrix");
        lines.Add("");
        lines.Add("```csv");
        lines.Add(report.Matrix.ToCsv());
        lines.Add("```");

        return string.Join(Environment.NewLine, lines);
    }
}
