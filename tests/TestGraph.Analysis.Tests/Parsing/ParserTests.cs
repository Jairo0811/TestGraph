using TestGraph.Analysis.Lexing;
using TestGraph.Analysis.Parsing;
using TestGraph.Analysis.Syntax;

namespace TestGraph.Analysis.Tests.Parsing;

public sealed class ParserTests
{
    [Fact]
    public void Parse_ScholarshipSample_BuildsStructuredAst()
    {
        const string source = """
            Programa Becas

            Entero edad
            Real promedio
            Real beca

            Leer edad
            Leer promedio

            Si edad > 18 Entonces
                Si promedio >= 9 Entonces
                    beca <- 2000
                Sino Si promedio >= 7.5 Entonces
                    beca <- 1000
                Sino
                    beca <- 500
                Fin Si
            Sino
                beca <- 3000
            Fin Si
            """;

        var result = Parser.Parse(source);

        Assert.False(result.HasErrors);
        Assert.Equal("Becas", result.Root.ProgramName);
        Assert.Equal(6, result.Root.Statements.Count);

        var outerIf = Assert.IsType<IfStatementSyntax>(result.Root.Statements[^1]);
        Assert.Single(outerIf.ThenStatements);
        Assert.Single(outerIf.ElseStatements);

        var nestedIf = Assert.IsType<IfStatementSyntax>(outerIf.ThenStatements[0]);
        Assert.Single(nestedIf.ElseIfClauses);
        Assert.Single(nestedIf.ElseStatements);
    }

    [Fact]
    public void Parse_Expression_RespectsOperatorPrecedence()
    {
        const string source = "resultado <- 1 + 2 * 3";

        var result = Parser.Parse(source);

        Assert.False(result.HasErrors);
        var assignment = Assert.IsType<AssignmentStatementSyntax>(Assert.Single(result.Root.Statements));
        var addition = Assert.IsType<BinaryExpressionSyntax>(assignment.Expression);

        Assert.Equal(TokenKind.Plus, addition.OperatorToken.Kind);
        var multiplication = Assert.IsType<BinaryExpressionSyntax>(addition.Right);
        Assert.Equal(TokenKind.Star, multiplication.OperatorToken.Kind);
    }

    [Fact]
    public void Parse_WhileAndFor_BuildLoopNodes()
    {
        const string source = """
            Mientras edad < 18 Hacer
                edad <- edad + 1
            Fin Mientras

            Para i = 1 Hasta 10 Paso 2
                Escribir i
            Fin Para
            """;

        var result = Parser.Parse(source);

        Assert.False(result.HasErrors);
        var whileStatement = Assert.IsType<WhileStatementSyntax>(result.Root.Statements[0]);
        var forStatement = Assert.IsType<ForStatementSyntax>(result.Root.Statements[1]);

        Assert.Single(whileStatement.Statements);
        Assert.NotNull(forStatement.Step);
        Assert.Single(forStatement.Statements);
    }

    [Fact]
    public void Parse_Write_AllowsCommaSeparatedExpressions()
    {
        const string source = "Escribir \"Beca: \", beca, \" pesos\"";

        var result = Parser.Parse(source);

        Assert.False(result.HasErrors);
        var write = Assert.IsType<WriteStatementSyntax>(Assert.Single(result.Root.Statements));
        Assert.Equal(3, write.Expressions.Count);
    }

    [Fact]
    public void Parse_BooleanExpression_BuildsUnaryAndBinaryNodes()
    {
        const string source = "resultado <- NO activo O edad >= 18 Y promedio >= 9";

        var result = Parser.Parse(source);

        Assert.False(result.HasErrors);
        var assignment = Assert.IsType<AssignmentStatementSyntax>(Assert.Single(result.Root.Statements));
        var orExpression = Assert.IsType<BinaryExpressionSyntax>(assignment.Expression);

        Assert.Equal(TokenKind.O, orExpression.OperatorToken.Kind);
        Assert.IsType<UnaryExpressionSyntax>(orExpression.Left);
        var andExpression = Assert.IsType<BinaryExpressionSyntax>(orExpression.Right);
        Assert.Equal(TokenKind.Y, andExpression.OperatorToken.Kind);
    }

    [Fact]
    public void Parse_MissingThen_ReportsParserDiagnostic()
    {
        const string source = """
            Si edad > 18
                Escribir edad
            Fin Si
            """;

        var result = Parser.Parse(source);

        Assert.True(result.HasErrors);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Code == "TGPL101");
    }

    [Fact]
    public void Parse_LexerError_IsPreservedAlongsideAst()
    {
        const string source = """
            Leer @ edad
            Escribir edad
            """;

        var result = Parser.Parse(source);

        Assert.True(result.HasErrors);
        Assert.Contains(result.LexerDiagnostics, diagnostic => diagnostic.Code == "TGPL001");
        Assert.Contains(result.Root.Statements, statement => statement is WriteStatementSyntax);
    }
}
