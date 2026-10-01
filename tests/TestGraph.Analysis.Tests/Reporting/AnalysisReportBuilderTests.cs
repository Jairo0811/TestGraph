using TestGraph.Analysis.Reporting;

namespace TestGraph.Analysis.Tests.Reporting;

public sealed class AnalysisReportBuilderTests
{
    [Fact]
    public void Build_ValidSource_ProducesCompleteReport()
    {
        const string source = """
            Si edad >= 18 Entonces
                Escribir "Adulto"
            Sino
                Escribir "Menor"
            Fin Si
            """;

        var report = new AnalysisReportBuilder().Build(source);

        Assert.Equal(2, report.Complexity.Value);
        Assert.Equal(2, report.BasisPaths.Paths.Count);
        Assert.Equal(report.Graph.Nodes.Count, report.Matrix.Size);
    }

    [Fact]
    public void ToMarkdown_IncludesStructuralSections()
    {
        const string source = "Escribir 1";
        var builder = new AnalysisReportBuilder();

        var markdown = builder.ToMarkdown(builder.Build(source));

        Assert.Contains("# TestGraph Analysis Report", markdown, StringComparison.Ordinal);
        Assert.Contains("## Basis Paths", markdown, StringComparison.Ordinal);
        Assert.Contains("## Adjacency Matrix", markdown, StringComparison.Ordinal);
    }
}
