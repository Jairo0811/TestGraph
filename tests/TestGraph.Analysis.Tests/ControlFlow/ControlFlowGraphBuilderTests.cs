using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Parsing;

namespace TestGraph.Analysis.Tests.ControlFlow;

public sealed class ControlFlowGraphBuilderTests
{
    [Fact]
    public void Build_StraightLineProgram_CreatesLinearGraph()
    {
        const string source = """
            Entero edad
            Leer edad
            Escribir edad
            """;

        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);

        var graph = new ControlFlowGraphBuilder().Build(parse.Root);

        Assert.Equal(5, graph.Nodes.Count);
        Assert.Equal(4, graph.Edges.Count);
        Assert.Equal(FlowNodeKind.Entry, graph.EntryNode.Kind);
        Assert.Equal(FlowNodeKind.Exit, graph.ExitNode.Kind);
        Assert.Equal(
            [FlowNodeKind.Entry, FlowNodeKind.Statement, FlowNodeKind.Statement, FlowNodeKind.Statement, FlowNodeKind.Exit],
            graph.Nodes.Select(node => node.Kind).ToArray());
    }

    [Fact]
    public void Build_IfElse_CreatesTrueFalseBranchesAndMerge()
    {
        const string source = """
            Si edad >= 18 Entonces
                Escribir "Adulto"
            Sino
                Escribir "Menor"
            Fin Si
            Escribir "Fin"
            """;

        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);

        var graph = new ControlFlowGraphBuilder().Build(parse.Root);

        var decision = Assert.Single(graph.Nodes, node => node.Kind == FlowNodeKind.Decision);
        var merge = Assert.Single(graph.Nodes, node => node.Kind == FlowNodeKind.Merge);

        Assert.Contains(graph.Edges, edge =>
            edge.SourceId == decision.Id && edge.Kind == FlowEdgeKind.True);
        Assert.Contains(graph.Edges, edge =>
            edge.SourceId == decision.Id && edge.Kind == FlowEdgeKind.False);

        var afterMerge = Assert.Single(graph.Outgoing(merge.Id));
        Assert.Equal(FlowEdgeKind.Normal, afterMerge.Kind);
    }

    [Fact]
    public void Build_ElseIfChain_CreatesDecisionForEachPredicate()
    {
        const string source = """
            Si promedio >= 9 Entonces
                beca <- 2000
            Sino Si promedio >= 7.5 Entonces
                beca <- 1000
            Sino Si promedio >= 6 Entonces
                beca <- 500
            Sino
                beca <- 0
            Fin Si
            """;

        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);

        var graph = new ControlFlowGraphBuilder().Build(parse.Root);

        Assert.Equal(3, graph.Nodes.Count(node => node.Kind == FlowNodeKind.Decision));
        Assert.Single(graph.Nodes, node => node.Kind == FlowNodeKind.Merge);
        Assert.Equal(3, graph.Edges.Count(edge => edge.Kind == FlowEdgeKind.True));
        Assert.Equal(3, graph.Edges.Count(edge => edge.Kind == FlowEdgeKind.False));
    }

    [Fact]
    public void Build_While_CreatesBackEdge()
    {
        const string source = """
            Mientras edad < 18 Hacer
                edad <- edad + 1
            Fin Mientras
            Escribir edad
            """;

        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);

        var graph = new ControlFlowGraphBuilder().Build(parse.Root);

        var decision = Assert.Single(graph.Nodes, node => node.Kind == FlowNodeKind.Decision);
        var backEdge = Assert.Single(graph.Edges, edge => edge.Kind == FlowEdgeKind.Back);

        Assert.Equal(decision.Id, backEdge.TargetId);
        Assert.Contains(graph.Edges, edge =>
            edge.SourceId == decision.Id && edge.Kind == FlowEdgeKind.False);
    }

    [Fact]
    public void Build_For_CreatesInitializerDecisionIncrementAndBackEdge()
    {
        const string source = """
            Para i = 1 Hasta 5 Paso 2
                Escribir i
            Fin Para
            """;

        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);

        var graph = new ControlFlowGraphBuilder().Build(parse.Root);

        Assert.Contains(graph.Nodes, node => node.Label == "i = 1");
        Assert.Contains(graph.Nodes, node => node.Label == "i <= 5");
        Assert.Contains(graph.Nodes, node => node.Label == "i += 2");
        Assert.Single(graph.Edges, edge => edge.Kind == FlowEdgeKind.Back);
    }

    [Fact]
    public void Build_ScholarshipStyleNestedFlow_ProducesDeterministicGraph()
    {
        const string source = """
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
                Sino
                    beca <- 500
                Fin Si
            Sino
                Si promedio >= 9 Entonces
                    beca <- 3000
                Sino
                    beca <- 0
                Fin Si
            Fin Si

            Escribir beca
            """;

        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);

        var builder = new ControlFlowGraphBuilder();
        var first = builder.Build(parse.Root);
        var second = builder.Build(parse.Root);

        Assert.Equal(
            first.Nodes.Select(node => (node.Id, node.Kind, node.Label)),
            second.Nodes.Select(node => (node.Id, node.Kind, node.Label)));

        Assert.Equal(first.Edges, second.Edges);
        Assert.True(first.Nodes.Count(node => node.Kind == FlowNodeKind.Decision) >= 4);
        Assert.True(first.Nodes.Count(node => node.Kind == FlowNodeKind.Merge) >= 3);
    }
}
