using TestGraph.Analysis.ControlFlow;

namespace TestGraph.Analysis.Complexity;

public sealed class CyclomaticComplexityAnalyzer
{
    public CyclomaticComplexityResult Analyze(ControlFlowGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        if (graph.Nodes.Count == 0)
        {
            throw new ArgumentException("The control-flow graph must contain at least one node.", nameof(graph));
        }

        ValidateEdges(graph);

        var nodeCount = graph.Nodes.Count;
        var edgeCount = graph.Edges.Count;
        var predicateNodeCount = graph.Nodes.Count(node => node.Kind == FlowNodeKind.Decision);
        var componentCount = CountWeaklyConnectedComponents(graph);

        var edgeNodeComplexity = edgeCount - nodeCount + (2 * componentCount);

        // TGPL control-flow decisions are binary. For one structured connected flow graph,
        // McCabe's predicate formulation is P + 1.
        var predicateComplexity = predicateNodeCount + componentCount;

        // In a planar representation of a structured CFG, the region count equals V(G).
        var regionCount = edgeNodeComplexity;

        return new CyclomaticComplexityResult(
            nodeCount,
            edgeCount,
            predicateNodeCount,
            componentCount,
            edgeNodeComplexity,
            predicateComplexity,
            regionCount,
            Classify(edgeNodeComplexity));
    }

    private static ComplexityLevel Classify(int value) =>
        value switch
        {
            <= 5 => ComplexityLevel.Low,
            <= 10 => ComplexityLevel.Moderate,
            <= 20 => ComplexityLevel.High,
            _ => ComplexityLevel.VeryHigh
        };

    private static void ValidateEdges(ControlFlowGraph graph)
    {
        var nodeIds = graph.Nodes.Select(node => node.Id).ToHashSet();

        foreach (var edge in graph.Edges)
        {
            if (!nodeIds.Contains(edge.SourceId) || !nodeIds.Contains(edge.TargetId))
            {
                throw new ArgumentException(
                    $"Edge {edge.SourceId}->{edge.TargetId} references a node that does not exist.",
                    nameof(graph));
            }
        }
    }

    private static int CountWeaklyConnectedComponents(ControlFlowGraph graph)
    {
        var adjacency = graph.Nodes.ToDictionary(
            node => node.Id,
            _ => new List<int>());

        foreach (var edge in graph.Edges)
        {
            adjacency[edge.SourceId].Add(edge.TargetId);
            adjacency[edge.TargetId].Add(edge.SourceId);
        }

        var visited = new HashSet<int>();
        var components = 0;

        foreach (var node in graph.Nodes)
        {
            if (!visited.Add(node.Id))
            {
                continue;
            }

            components++;
            var stack = new Stack<int>();
            stack.Push(node.Id);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                foreach (var neighbor in adjacency[current])
                {
                    if (visited.Add(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }

        return components;
    }
}
