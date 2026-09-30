using TestGraph.Analysis.Lexing;

namespace TestGraph.Analysis.ControlFlow;

public sealed record FlowNode(
    int Id,
    FlowNodeKind Kind,
    string Label,
    TextSpan? SourceSpan);
