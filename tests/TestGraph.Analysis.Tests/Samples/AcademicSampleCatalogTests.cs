using TestGraph.Analysis.Complexity;
using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Parsing;
using TestGraph.Analysis.Samples;

namespace TestGraph.Analysis.Tests.Samples;

public sealed class AcademicSampleCatalogTests
{
    [Fact]
    public void Catalog_ContainsFiveOriginalExercises()
    {
        Assert.Equal(5, AcademicSampleCatalog.All.Count);
    }

    [Fact]
    public void Catalog_AnalyzerReadySamples_ParseAndAnalyze()
    {
        foreach (var sample in AcademicSampleCatalog.All.Where(sample => sample.AnalyzerReady))
        {
            var parse = Parser.Parse(sample.SourceCode);
            Assert.False(parse.HasErrors);

            var graph = new ControlFlowGraphBuilder().Build(parse.Root);
            var complexity = new CyclomaticComplexityAnalyzer().Analyze(graph);

            if (sample.ExpectedCyclomaticComplexity is int expected)
            {
                Assert.Equal(expected, complexity.Value);
            }
        }
    }

    [Fact]
    public void Catalog_PreservesMatrixSamplesWithExplicitLimitation()
    {
        var matrixSamples = AcademicSampleCatalog.All.Where(sample => !sample.AnalyzerReady).ToArray();

        Assert.Equal(2, matrixSamples.Length);
        Assert.All(matrixSamples, sample => Assert.Contains("matrix", sample.Limitation!, StringComparison.OrdinalIgnoreCase));
    }
}
