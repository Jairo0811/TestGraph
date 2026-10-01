using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Parsing;
using TestGraph.Analysis.Paths;

namespace TestGraph.Analysis.Tests.Paths;

public sealed class BasisPathAnalyzerTests
{
    [Fact]
    public void Analyze_StraightLine_ReturnsSinglePath()
    {
        var graph = BuildGraph("""
            Leer edad
            Escribir edad
            """);

        var result = new BasisPathAnalyzer().Analyze(graph);

        Assert.True(result.IsComplete);
        var path = Assert.Single(result.Paths);
        Assert.Equal(graph.EntryNodeId, path.NodeIds[0]);
        Assert.Equal(graph.ExitNodeId, path.NodeIds[^1]);
    }

    [Fact]
    public void Analyze_IfElse_ReturnsTwoIndependentPaths()
    {
        var graph = BuildGraph("""
            Si edad >= 18 Entonces
                Escribir "Adulto"
            Sino
                Escribir "Menor"
            Fin Si
            """);

        var result = new BasisPathAnalyzer().Analyze(graph);

        Assert.Equal(2, result.CyclomaticComplexity);
        Assert.Equal(2, result.Paths.Count);
        Assert.True(result.IsComplete);
        Assert.NotEqual(result.Paths[0].EdgeIndexes, result.Paths[1].EdgeIndexes);
    }

    [Fact]
    public void Analyze_While_ReturnsZeroAndOneIterationPaths()
    {
        var graph = BuildGraph("""
            Mientras edad < 18 Hacer
                edad <- edad + 1
            Fin Mientras
            """);

        var result = new BasisPathAnalyzer().Analyze(graph);

        Assert.Equal(2, result.Paths.Count);
        Assert.Contains(result.Paths, path =>
            path.EdgeIndexes.Any(index => graph.Edges[index].Kind == FlowEdgeKind.Back));
        Assert.Contains(result.Paths, path =>
            path.EdgeIndexes.All(index => graph.Edges[index].Kind != FlowEdgeKind.Back));
    }

    [Fact]
    public void Analyze_ScholarshipFlow_ReturnsSevenPaths()
    {
        var graph = BuildGraph("""
            Entero edad
            Real promedio
            Real beca

            Leer edad
            Leer promedio

            Si edad > 18 Entonces
                Si promedio >= 9 Entonces
                    beca <- 2000
                Sino Si promedio >= 7.5 Entonces
                    beca <- 1000
                Sino Si promedio >= 6 Entonces
                    beca <- 500
                Sino
                    beca <- 0
                Fin Si
            Sino
                Si promedio >= 9 Entonces
                    beca <- 3000
                Sino Si promedio >= 8 Entonces
                    beca <- 2000
                Sino Si promedio >= 6 Entonces
                    beca <- 100
                Sino
                    beca <- 0
                Fin Si
            Fin Si
            """);

        var result = new BasisPathAnalyzer().Analyze(graph);

        Assert.Equal(7, result.CyclomaticComplexity);
        Assert.Equal(7, result.Paths.Count);
        Assert.True(result.IsComplete);
        Assert.All(result.Paths, path =>
        {
            Assert.Equal(graph.EntryNodeId, path.NodeIds[0]);
            Assert.Equal(graph.ExitNodeId, path.NodeIds[^1]);
        });
    }

    [Fact]
    public void Analyze_IsDeterministic()
    {
        var graph = BuildGraph("""
            Si x > 0 Entonces
                Escribir x
            Sino
                Escribir 0
            Fin Si
            """);

        var analyzer = new BasisPathAnalyzer();
        var first = analyzer.Analyze(graph);
        var second = analyzer.Analyze(graph);

        Assert.Equal(
            first.Paths.Select(path => path.Display),
            second.Paths.Select(path => path.Display));
    }

    private static ControlFlowGraph BuildGraph(string source)
    {
        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);
        return new ControlFlowGraphBuilder().Build(parse.Root);
    }
}
