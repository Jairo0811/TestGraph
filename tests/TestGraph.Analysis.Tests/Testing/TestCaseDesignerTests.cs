using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Parsing;
using TestGraph.Analysis.Paths;
using TestGraph.Analysis.Testing;

namespace TestGraph.Analysis.Tests.Testing;

public sealed class TestCaseDesignerTests
{
    [Fact]
    public void Design_ValidManualCase_ReturnsNormalizedCase()
    {
        var paths = BuildPaths("""
            Si edad >= 18 Entonces
                Escribir "Adulto"
            Sino
                Escribir "Menor"
            Fin Si
            """);

        var result = new TestCaseDesigner().Design(
            paths,
            [
                new TestCaseDraft(
                    "Adult branch",
                    new Dictionary<string, string> { ["edad"] = "20" },
                    "Adulto",
                    TestCaseTechnique.PathCoverage,
                    1)
            ]);

        Assert.True(result.IsValid);
        var testCase = Assert.Single(result.TestCases);
        Assert.Equal(1, testCase.Number);
        Assert.NotEmpty(testCase.CoveredNodeIds);
        Assert.NotEmpty(testCase.CoveredEdgeIndexes);
    }

    [Fact]
    public void Design_UnknownPath_ReturnsDiagnostic()
    {
        var result = new TestCaseDesigner().Design(
            [],
            [
                new TestCaseDraft(
                    "Invalid path",
                    new Dictionary<string, string>(),
                    "Anything",
                    TestCaseTechnique.PathCoverage,
                    99)
            ]);

        Assert.False(result.IsValid);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Code == "TGPL-TC003");
    }

    [Fact]
    public void Design_MissingRequiredFields_ReturnsDiagnostics()
    {
        var result = new TestCaseDesigner().Design(
            [],
            [
                new TestCaseDraft(
                    "",
                    new Dictionary<string, string>(),
                    "",
                    TestCaseTechnique.Manual)
            ]);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Diagnostics);
    }

    private static IReadOnlyList<ExecutionPath> BuildPaths(string source)
    {
        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);
        var graph = new ControlFlowGraphBuilder().Build(parse.Root);
        return new BasisPathAnalyzer().Analyze(graph).Paths;
    }
}
