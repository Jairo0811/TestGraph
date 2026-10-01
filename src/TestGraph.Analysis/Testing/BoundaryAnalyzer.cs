using System.Globalization;
using TestGraph.Analysis.Lexing;
using TestGraph.Analysis.Syntax;

namespace TestGraph.Analysis.Testing;

public sealed class BoundaryAnalyzer
{
    public IReadOnlyList<BoundarySuggestion> Analyze(CompilationUnitSyntax root)
    {
        ArgumentNullException.ThrowIfNull(root);

        var variableTypes = root.Statements
            .OfType<VariableDeclarationSyntax>()
            .ToDictionary(
                declaration => declaration.Identifier.Lexeme,
                declaration => declaration.TypeKeyword.Kind,
                StringComparer.OrdinalIgnoreCase);

        var suggestions = new List<BoundarySuggestion>();

        foreach (var statement in root.Statements)
        {
            VisitStatement(statement);
        }

        return suggestions
            .GroupBy(item => new
            {
                Variable = item.Variable.ToUpperInvariant(),
                item.Operator,
                item.Threshold
            })
            .Select(group => group.First())
            .ToArray();

        void VisitStatement(StatementSyntax statement)
        {
            switch (statement)
            {
                case IfStatementSyntax ifStatement:
                    VisitExpression(ifStatement.Condition);
                    foreach (var child in ifStatement.ThenStatements)
                    {
                        VisitStatement(child);
                    }

                    foreach (var clause in ifStatement.ElseIfClauses)
                    {
                        VisitExpression(clause.Condition);
                        foreach (var child in clause.Statements)
                        {
                            VisitStatement(child);
                        }
                    }

                    foreach (var child in ifStatement.ElseStatements)
                    {
                        VisitStatement(child);
                    }

                    break;

                case WhileStatementSyntax whileStatement:
                    VisitExpression(whileStatement.Condition);
                    foreach (var child in whileStatement.Statements)
                    {
                        VisitStatement(child);
                    }

                    break;

                case ForStatementSyntax forStatement:
                    VisitExpression(forStatement.Start);
                    VisitExpression(forStatement.End);
                    if (forStatement.Step is not null)
                    {
                        VisitExpression(forStatement.Step);
                    }

                    foreach (var child in forStatement.Statements)
                    {
                        VisitStatement(child);
                    }

                    break;

                case AssignmentStatementSyntax assignment:
                    VisitExpression(assignment.Expression);
                    break;

                case WriteStatementSyntax write:
                    foreach (var expression in write.Expressions)
                    {
                        VisitExpression(expression);
                    }

                    break;
            }
        }

        void VisitExpression(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case BinaryExpressionSyntax binary:
                    if (TryCreateSuggestion(binary, variableTypes, out var suggestion))
                    {
                        suggestions.Add(suggestion);
                    }

                    VisitExpression(binary.Left);
                    VisitExpression(binary.Right);
                    break;

                case UnaryExpressionSyntax unary:
                    VisitExpression(unary.Operand);
                    break;

                case ParenthesizedExpressionSyntax parenthesized:
                    VisitExpression(parenthesized.Expression);
                    break;
            }
        }
    }

    private static bool TryCreateSuggestion(
        BinaryExpressionSyntax binary,
        IReadOnlyDictionary<string, TokenKind> variableTypes,
        out BoundarySuggestion suggestion)
    {
        suggestion = default!;

        if (!IsComparison(binary.OperatorToken.Kind))
        {
            return false;
        }

        string variable;
        LiteralExpressionSyntax literal;
        var operatorText = binary.OperatorToken.Lexeme;

        if (binary.Left is NameExpressionSyntax leftName &&
            binary.Right is LiteralExpressionSyntax rightLiteral)
        {
            variable = leftName.Identifier.Lexeme;
            literal = rightLiteral;
        }
        else if (binary.Left is LiteralExpressionSyntax leftLiteral &&
                 binary.Right is NameExpressionSyntax rightName)
        {
            variable = rightName.Identifier.Lexeme;
            literal = leftLiteral;
            operatorText = ReverseOperator(binary.OperatorToken.Kind);
        }
        else
        {
            return false;
        }

        if (literal.LiteralToken.Kind != TokenKind.Number ||
            literal.Value is not string raw ||
            !decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var threshold))
        {
            return false;
        }

        var step = variableTypes.TryGetValue(variable, out var type) && type == TokenKind.Entero
            ? 1m
            : 0.01m;

        suggestion = new BoundarySuggestion(
            variable,
            operatorText,
            threshold,
            [threshold - step, threshold, threshold + step],
            binary.Span.Start.Line);

        return true;
    }

    private static bool IsComparison(TokenKind kind) =>
        kind is TokenKind.Equal
            or TokenKind.NotEqual
            or TokenKind.Greater
            or TokenKind.GreaterOrEqual
            or TokenKind.Less
            or TokenKind.LessOrEqual;

    private static string ReverseOperator(TokenKind kind) =>
        kind switch
        {
            TokenKind.Greater => "<",
            TokenKind.GreaterOrEqual => "<=",
            TokenKind.Less => ">",
            TokenKind.LessOrEqual => ">=",
            TokenKind.Equal => "=",
            TokenKind.NotEqual => "<>",
            _ => string.Empty
        };
}
