namespace TestGraph.Analysis.ControlFlow;

public sealed class ControlFlowGraph
{
    public ControlFlowGraph(
        IReadOnlyList<FlowNode> nodes,
        IReadOnlyList<FlowEdge> edges,
        int entryNodeId,
        int exitNodeId)
    {
        Nodes = nodes;
        Edges = edges;
        EntryNodeId = entryNodeId;
        ExitNodeId = exitNodeId;
    }

    public IReadOnlyList<FlowNode> Nodes { get; }

    public IReadOnlyList<FlowEdge> Edges { get; }

    public int EntryNodeId { get; }

    public int ExitNodeId { get; }

    public FlowNode EntryNode => Nodes.Single(node => node.Id == EntryNodeId);

    public FlowNode ExitNode => Nodes.Single(node => node.Id == ExitNodeId);

    public IEnumerable<FlowEdge> Outgoing(int nodeId) =>
        Edges.Where(edge => edge.SourceId == nodeId);

    public IEnumerable<FlowEdge> Incoming(int nodeId) =>
        Edges.Where(edge => edge.TargetId == nodeId);
}
