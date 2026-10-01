using TestGraph.Analysis.ControlFlow;

namespace TestGraph.Analysis.Matrix;

public sealed class AdjacencyMatrixBuilder
{
    public AdjacencyMatrixResult Build(ControlFlowGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        var nodeIds = graph.Nodes
            .Select(node => node.Id)
            .OrderBy(id => id)
            .ToArray();

        var indexByNodeId = nodeIds
            .Select((id, index) => (id, index))
            .ToDictionary(item => item.id, item => item.index);

        var rows = Enumerable.Range(0, nodeIds.Length)
            .Select(_ => new int[nodeIds.Length])
            .ToArray();

        foreach (var edge in graph.Edges)
        {
            if (!indexByNodeId.TryGetValue(edge.SourceId, out var row) ||
                !indexByNodeId.TryGetValue(edge.TargetId, out var column))
            {
                throw new ArgumentException(
                    $"Edge {edge.SourceId}->{edge.TargetId} references an unknown node.",
                    nameof(graph));
            }

            rows[row][column]++;
        }

        return new AdjacencyMatrixResult(
            nodeIds,
            rows.Select(row => (IReadOnlyList<int>)row).ToArray());
    }
}
