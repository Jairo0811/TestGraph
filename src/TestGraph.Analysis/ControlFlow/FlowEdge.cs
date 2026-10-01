namespace TestGraph.Analysis.ControlFlow;

public sealed record FlowEdge(
    int SourceId,
    int TargetId,
    FlowEdgeKind Kind,
    string? Label = null);
