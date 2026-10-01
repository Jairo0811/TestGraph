using TestGraph.Analysis.Complexity;
using TestGraph.Analysis.ControlFlow;

namespace TestGraph.Analysis.Paths;

public sealed class BasisPathAnalyzer
{
    public BasisPathAnalysisResult Analyze(ControlFlowGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        var complexity = new CyclomaticComplexityAnalyzer().Analyze(graph).Value;
        var candidates = EnumerateCandidatePaths(graph);

        var selected = SelectIndependentPaths(candidates, graph.Edges.Count, complexity)
            .Select((path, index) => new ExecutionPath(index + 1, path.NodeIds, path.EdgeIndexes))
            .ToArray();

        return new BasisPathAnalysisResult(complexity, selected, candidates.Count);
    }

    private static IReadOnlyList<PathCandidate> EnumerateCandidatePaths(ControlFlowGraph graph)
    {
        var edgeIndexes = graph.Edges
            .Select((edge, index) => (edge, index))
            .GroupBy(item => item.edge.SourceId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(item => EdgeOrder(item.edge.Kind))
                    .ThenBy(item => item.edge.TargetId)
                    .ToArray());

        var candidates = new List<PathCandidate>();
        var nodePath = new List<int> { graph.EntryNodeId };
        var edgePath = new List<int>();
        var usedBackEdges = new HashSet<int>();
        var visitCounts = new Dictionary<int, int> { [graph.EntryNodeId] = 1 };

        var maxDepth = Math.Max(graph.Nodes.Count + graph.Edges.Count + 4, 16);

        Traverse(graph.EntryNodeId);

        return candidates
            .GroupBy(candidate => string.Join(",", candidate.EdgeIndexes))
            .Select(group => group.First())
            .OrderBy(candidate => candidate.EdgeIndexes.Count)
            .ThenBy(candidate => string.Join(",", candidate.EdgeIndexes))
            .ToArray();

        void Traverse(int nodeId)
        {
            if (edgePath.Count > maxDepth)
            {
                return;
            }

            if (nodeId == graph.ExitNodeId)
            {
                candidates.Add(new PathCandidate(nodePath.ToArray(), edgePath.ToArray()));
                return;
            }

            if (!edgeIndexes.TryGetValue(nodeId, out var outgoing))
            {
                return;
            }

            foreach (var (edge, edgeIndex) in outgoing)
            {
                var isBack = edge.Kind == FlowEdgeKind.Back;

                if (isBack && usedBackEdges.Contains(edgeIndex))
                {
                    continue;
                }

                var nextVisits = visitCounts.GetValueOrDefault(edge.TargetId);
                var maxVisits = isBack ? 2 : 1;

                if (nextVisits >= maxVisits && edge.TargetId != graph.ExitNodeId)
                {
                    continue;
                }

                if (isBack)
                {
                    usedBackEdges.Add(edgeIndex);
                }

                edgePath.Add(edgeIndex);
                nodePath.Add(edge.TargetId);
                visitCounts[edge.TargetId] = nextVisits + 1;

                Traverse(edge.TargetId);

                if (nextVisits == 0)
                {
                    visitCounts.Remove(edge.TargetId);
                }
                else
                {
                    visitCounts[edge.TargetId] = nextVisits;
                }

                nodePath.RemoveAt(nodePath.Count - 1);
                edgePath.RemoveAt(edgePath.Count - 1);

                if (isBack)
                {
                    usedBackEdges.Remove(edgeIndex);
                }
            }
        }
    }

    private static IReadOnlyList<PathCandidate> SelectIndependentPaths(
        IReadOnlyList<PathCandidate> candidates,
        int edgeCount,
        int targetCount)
    {
        var selected = new List<PathCandidate>();
        var basis = new List<BitVector>();

        foreach (var candidate in candidates)
        {
            var vector = BitVector.FromEdges(edgeCount, candidate.EdgeIndexes);

            if (TryAddToBasis(vector, basis))
            {
                selected.Add(candidate);

                if (selected.Count == targetCount)
                {
                    break;
                }
            }
        }

        return selected;
    }

    private static bool TryAddToBasis(BitVector candidate, List<BitVector> basis)
    {
        var reduced = candidate.Clone();

        foreach (var row in basis.OrderByDescending(row => row.Pivot))
        {
            if (reduced[row.Pivot])
            {
                reduced.Xor(row);
            }
        }

        if (reduced.IsZero)
        {
            return false;
        }

        var pivot = reduced.HighestSetBit;
        var normalized = reduced.Clone();

        for (var i = 0; i < basis.Count; i++)
        {
            if (basis[i][pivot])
            {
                basis[i].Xor(normalized);
            }
        }

        basis.Add(normalized);
        return true;
    }

    private static int EdgeOrder(FlowEdgeKind kind) =>
        kind switch
        {
            FlowEdgeKind.Normal => 0,
            FlowEdgeKind.True => 1,
            FlowEdgeKind.False => 2,
            FlowEdgeKind.Back => 3,
            _ => 4
        };

    private sealed record PathCandidate(
        IReadOnlyList<int> NodeIds,
        IReadOnlyList<int> EdgeIndexes);

    private sealed class BitVector
    {
        private readonly bool[] _bits;

        private BitVector(bool[] bits)
        {
            _bits = bits;
        }

        public bool this[int index] => _bits[index];

        public int Pivot => HighestSetBit;

        public int HighestSetBit
        {
            get
            {
                for (var i = _bits.Length - 1; i >= 0; i--)
                {
                    if (_bits[i])
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        public bool IsZero => !_bits.Any(bit => bit);

        public static BitVector FromEdges(int edgeCount, IEnumerable<int> edgeIndexes)
        {
            var bits = new bool[edgeCount];
            foreach (var index in edgeIndexes)
            {
                bits[index] = !bits[index];
            }

            return new BitVector(bits);
        }

        public BitVector Clone() => new((bool[])_bits.Clone());

        public void Xor(BitVector other)
        {
            for (var i = 0; i < _bits.Length; i++)
            {
                _bits[i] ^= other._bits[i];
            }
        }
    }
}
