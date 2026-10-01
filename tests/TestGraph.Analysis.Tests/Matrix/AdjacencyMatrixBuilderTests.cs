using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Matrix;
using TestGraph.Analysis.Parsing;

namespace TestGraph.Analysis.Tests.Matrix;

public sealed class AdjacencyMatrixBuilderTests
{
    [Fact]
    public void Build_StraightLine_CreatesExpectedConnections()
    {
        var graph = BuildGraph("""
            Leer edad
            Escribir edad
            """);

        var matrix = new AdjacencyMatrixBuilder().Build(graph);

        Assert.Equal(graph.Nodes.Count, matrix.Size);
        Assert.Equal(1, matrix[0, 1]);
        Assert.Equal(1, matrix[1, 2]);
        Assert.Equal(1, matrix[2, 3]);
        Assert.Equal(0, matrix[3, 0]);
    }

    [Fact]
    public void Build_IfElse_ContainsBothDecisionBranches()
    {
        var graph = BuildGraph("""
            Si edad >= 18 Entonces
                Escribir "Adulto"
            Sino
                Escribir "Menor"
            Fin Si
            """);

        var matrix = new AdjacencyMatrixBuilder().Build(graph);
        var decision = Assert.Single(graph.Nodes.Where(node => node.Kind == FlowNodeKind.Decision));
        var row = matrix.NodeIds.IndexOf(decision.Id);

        Assert.Equal(2, matrix.Rows[row].Count(value => value > 0));
    }

    [Fact]
    public void Build_While_ContainsBackEdge()
    {
        var graph = BuildGraph("""
            Mientras edad < 18 Hacer
                edad <- edad + 1
            Fin Mientras
            """);

        var matrix = new AdjacencyMatrixBuilder().Build(graph);
        var back = Assert.Single(graph.Edges.Where(edge => edge.Kind == FlowEdgeKind.Back));
        var row = matrix.NodeIds.IndexOf(back.SourceId);
        var column = matrix.NodeIds.IndexOf(back.TargetId);

        Assert.Equal(1, matrix[row, column]);
    }

    [Fact]
    public void Build_Csv_HasNodeHeadersAndRows()
    {
        var graph = BuildGraph("Escribir 1");
        var matrix = new AdjacencyMatrixBuilder().Build(graph);

        var csv = matrix.ToCsv();

        Assert.StartsWith("," + string.Join(",", matrix.NodeIds), csv, StringComparison.Ordinal);
        Assert.Equal(matrix.Size + 1, csv.Split(Environment.NewLine).Length);
    }

    private static ControlFlowGraph BuildGraph(string source)
    {
        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);
        return new ControlFlowGraphBuilder().Build(parse.Root);
    }
}

internal static class ReadOnlyListExtensions
{
    public static int IndexOf<T>(this IReadOnlyList<T> values, T value)
    {
        for (var i = 0; i < values.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(values[i], value))
            {
                return i;
            }
        }

        return -1;
    }
}
