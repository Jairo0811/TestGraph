using TestGraph.Analysis.Lexing;
using TestGraph.Analysis.Syntax;

namespace TestGraph.Analysis.ControlFlow;

internal static class SyntaxLabelFormatter
{
    public static string Statement(StatementSyntax statement) =>
        statement switch
        {
            VariableDeclarationSyntax declaration =>
                $"{declaration.TypeKeyword.Lexeme} {declaration.Identifier.Lexeme}",

            ReadStatementSyntax read =>
                $"Leer {read.Identifier.Lexeme}",

            WriteStatementSyntax write =>
                $"Escribir {string.Join(", ", write.Expressions.Select(Expression))}",

            AssignmentStatementSyntax assignment =>
                $"{assignment.Identifier.Lexeme} <- {Expression(assignment.Expression)}",

            ErrorStatementSyntax error =>
                $"Error: {error.Token.Lexeme}",

            _ => statement.GetType().Name
        };

    public static string Expression(ExpressionSyntax expression) =>
        expression switch
        {
            LiteralExpressionSyntax literal => FormatLiteral(literal),
            NameExpressionSyntax name => name.Identifier.Lexeme,
            UnaryExpressionSyntax unary =>
                $"{unary.OperatorToken.Lexeme} {Expression(unary.Operand)}",
            BinaryExpressionSyntax binary =>
                $"{Expression(binary.Left)} {binary.OperatorToken.Lexeme} {Expression(binary.Right)}",
            ParenthesizedExpressionSyntax parenthesized =>
                $"({Expression(parenthesized.Expression)})",
            ErrorExpressionSyntax error => error.Token.Lexeme,
            _ => expression.GetType().Name
        };

    private static string FormatLiteral(LiteralExpressionSyntax literal)
    {
        if (literal.LiteralToken.Kind == TokenKind.String)
        {
            return $"\"{literal.Value}\"";
        }

        return literal.LiteralToken.Lexeme;
    }
}
