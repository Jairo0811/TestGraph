using TestGraph.Analysis.Lexing;

namespace TestGraph.Analysis.Syntax;

public abstract record SyntaxNode(TextSpan Span);

public abstract record StatementSyntax(TextSpan Span) : SyntaxNode(Span);

public abstract record ExpressionSyntax(TextSpan Span) : SyntaxNode(Span);

public sealed record CompilationUnitSyntax(
    string? ProgramName,
    IReadOnlyList<StatementSyntax> Statements,
    TextSpan Span) : SyntaxNode(Span);

public sealed record VariableDeclarationSyntax(
    Token TypeKeyword,
    Token Identifier,
    TextSpan Span) : StatementSyntax(Span);

public sealed record ReadStatementSyntax(
    Token Keyword,
    Token Identifier,
    TextSpan Span) : StatementSyntax(Span);

public sealed record WriteStatementSyntax(
    Token Keyword,
    IReadOnlyList<ExpressionSyntax> Expressions,
    TextSpan Span) : StatementSyntax(Span);

public sealed record AssignmentStatementSyntax(
    Token Identifier,
    Token Assignment,
    ExpressionSyntax Expression,
    TextSpan Span) : StatementSyntax(Span);

public sealed record IfStatementSyntax(
    ExpressionSyntax Condition,
    IReadOnlyList<StatementSyntax> ThenStatements,
    IReadOnlyList<ElseIfClauseSyntax> ElseIfClauses,
    IReadOnlyList<StatementSyntax> ElseStatements,
    TextSpan Span) : StatementSyntax(Span);

public sealed record ElseIfClauseSyntax(
    ExpressionSyntax Condition,
    IReadOnlyList<StatementSyntax> Statements,
    TextSpan Span) : SyntaxNode(Span);

public sealed record WhileStatementSyntax(
    ExpressionSyntax Condition,
    IReadOnlyList<StatementSyntax> Statements,
    TextSpan Span) : StatementSyntax(Span);

public sealed record ForStatementSyntax(
    Token Identifier,
    ExpressionSyntax Start,
    ExpressionSyntax End,
    ExpressionSyntax? Step,
    IReadOnlyList<StatementSyntax> Statements,
    TextSpan Span) : StatementSyntax(Span);

public sealed record ErrorStatementSyntax(
    Token Token,
    TextSpan Span) : StatementSyntax(Span);

public sealed record LiteralExpressionSyntax(
    Token LiteralToken,
    object? Value,
    TextSpan Span) : ExpressionSyntax(Span);

public sealed record NameExpressionSyntax(
    Token Identifier,
    TextSpan Span) : ExpressionSyntax(Span);

public sealed record UnaryExpressionSyntax(
    Token OperatorToken,
    ExpressionSyntax Operand,
    TextSpan Span) : ExpressionSyntax(Span);

public sealed record BinaryExpressionSyntax(
    ExpressionSyntax Left,
    Token OperatorToken,
    ExpressionSyntax Right,
    TextSpan Span) : ExpressionSyntax(Span);

public sealed record ParenthesizedExpressionSyntax(
    Token OpenParenthesis,
    ExpressionSyntax Expression,
    Token CloseParenthesis,
    TextSpan Span) : ExpressionSyntax(Span);

public sealed record ErrorExpressionSyntax(
    Token Token,
    TextSpan Span) : ExpressionSyntax(Span);
