using TestGraph.Analysis.Syntax;

namespace TestGraph.Analysis.ControlFlow;

public sealed class ControlFlowGraphBuilder
{
    private readonly List<FlowNode> _nodes = [];
    private readonly List<FlowEdge> _edges = [];
    private int _nextNodeId = 1;

    public ControlFlowGraph Build(CompilationUnitSyntax root)
    {
        ArgumentNullException.ThrowIfNull(root);

        _nodes.Clear();
        _edges.Clear();
        _nextNodeId = 1;

        var entry = AddNode(FlowNodeKind.Entry, "Inicio", root.Span);
        var pending = BuildSequence(
            root.Statements,
            [new PendingEdge(entry.Id, FlowEdgeKind.Normal)]);

        var exit = AddNode(FlowNodeKind.Exit, "Fin", root.Span);
        ConnectPending(pending, exit.Id);

        return new ControlFlowGraph(
            _nodes.ToArray(),
            _edges.ToArray(),
            entry.Id,
            exit.Id);
    }

    private List<PendingEdge> BuildSequence(
        IReadOnlyList<StatementSyntax> statements,
        List<PendingEdge> incoming)
    {
        var pending = incoming;

        foreach (var statement in statements)
        {
            pending = BuildStatement(statement, pending);
        }

        return pending;
    }

    private List<PendingEdge> BuildStatement(
        StatementSyntax statement,
        List<PendingEdge> incoming) =>
        statement switch
        {
            IfStatementSyntax ifStatement => BuildIf(ifStatement, incoming),
            WhileStatementSyntax whileStatement => BuildWhile(whileStatement, incoming),
            ForStatementSyntax forStatement => BuildFor(forStatement, incoming),
            _ => BuildSimpleStatement(statement, incoming)
        };

    private List<PendingEdge> BuildSimpleStatement(
        StatementSyntax statement,
        List<PendingEdge> incoming)
    {
        var node = AddNode(
            FlowNodeKind.Statement,
            SyntaxLabelFormatter.Statement(statement),
            statement.Span);

        ConnectPending(incoming, node.Id);

        return [new PendingEdge(node.Id, FlowEdgeKind.Normal)];
    }

    private List<PendingEdge> BuildIf(
        IfStatementSyntax statement,
        List<PendingEdge> incoming)
    {
        var merge = AddNode(FlowNodeKind.Merge, "Merge", statement.Span);

        var decision = AddNode(
            FlowNodeKind.Decision,
            SyntaxLabelFormatter.Expression(statement.Condition),
            statement.Condition.Span);

        ConnectPending(incoming, decision.Id);

        var thenPending = BuildSequence(
            statement.ThenStatements,
            [new PendingEdge(decision.Id, FlowEdgeKind.True, "Sí")]);
        ConnectPending(thenPending, merge.Id);

        var falseSource = decision.Id;

        foreach (var elseIf in statement.ElseIfClauses)
        {
            var elseIfDecision = AddNode(
                FlowNodeKind.Decision,
                SyntaxLabelFormatter.Expression(elseIf.Condition),
                elseIf.Condition.Span);

            AddEdge(falseSource, elseIfDecision.Id, FlowEdgeKind.False, "No");

            var branchPending = BuildSequence(
                elseIf.Statements,
                [new PendingEdge(elseIfDecision.Id, FlowEdgeKind.True, "Sí")]);

            ConnectPending(branchPending, merge.Id);
            falseSource = elseIfDecision.Id;
        }

        if (statement.ElseStatements.Count > 0)
        {
            var elsePending = BuildSequence(
                statement.ElseStatements,
                [new PendingEdge(falseSource, FlowEdgeKind.False, "No")]);

            ConnectPending(elsePending, merge.Id);
        }
        else
        {
            AddEdge(falseSource, merge.Id, FlowEdgeKind.False, "No");
        }

        return [new PendingEdge(merge.Id, FlowEdgeKind.Normal)];
    }

    private List<PendingEdge> BuildWhile(
        WhileStatementSyntax statement,
        List<PendingEdge> incoming)
    {
        var decision = AddNode(
            FlowNodeKind.Decision,
            SyntaxLabelFormatter.Expression(statement.Condition),
            statement.Condition.Span);

        ConnectPending(incoming, decision.Id);

        var bodyPending = BuildSequence(
            statement.Statements,
            [new PendingEdge(decision.Id, FlowEdgeKind.True, "Sí")]);

        foreach (var pending in bodyPending)
        {
            AddEdge(pending.SourceId, decision.Id, FlowEdgeKind.Back, "Volver");
        }

        return [new PendingEdge(decision.Id, FlowEdgeKind.False, "No")];
    }

    private List<PendingEdge> BuildFor(
        ForStatementSyntax statement,
        List<PendingEdge> incoming)
    {
        var initializer = AddNode(
            FlowNodeKind.Statement,
            $"{statement.Identifier.Lexeme} = {SyntaxLabelFormatter.Expression(statement.Start)}",
            statement.Span);

        ConnectPending(incoming, initializer.Id);

        var decision = AddNode(
            FlowNodeKind.Decision,
            $"{statement.Identifier.Lexeme} <= {SyntaxLabelFormatter.Expression(statement.End)}",
            statement.Span);

        AddEdge(initializer.Id, decision.Id, FlowEdgeKind.Normal);

        var bodyPending = BuildSequence(
            statement.Statements,
            [new PendingEdge(decision.Id, FlowEdgeKind.True, "Sí")]);

        var stepExpression = statement.Step is null
            ? "1"
            : SyntaxLabelFormatter.Expression(statement.Step);

        var increment = AddNode(
            FlowNodeKind.Statement,
            $"{statement.Identifier.Lexeme} += {stepExpression}",
            statement.Span);

        ConnectPending(bodyPending, increment.Id);

        AddEdge(increment.Id, decision.Id, FlowEdgeKind.Back, "Volver");

        return [new PendingEdge(decision.Id, FlowEdgeKind.False, "No")];
    }

    private FlowNode AddNode(
        FlowNodeKind kind,
        string label,
        TestGraph.Analysis.Lexing.TextSpan? sourceSpan)
    {
        var node = new FlowNode(_nextNodeId++, kind, label, sourceSpan);
        _nodes.Add(node);
        return node;
    }

    private void ConnectPending(IEnumerable<PendingEdge> pendingEdges, int targetId)
    {
        foreach (var pending in pendingEdges)
        {
            AddEdge(pending.SourceId, targetId, pending.Kind, pending.Label);
        }
    }

    private void AddEdge(
        int sourceId,
        int targetId,
        FlowEdgeKind kind,
        string? label = null)
    {
        _edges.Add(new FlowEdge(sourceId, targetId, kind, label));
    }

    private sealed record PendingEdge(
        int SourceId,
        FlowEdgeKind Kind,
        string? Label = null);
}
