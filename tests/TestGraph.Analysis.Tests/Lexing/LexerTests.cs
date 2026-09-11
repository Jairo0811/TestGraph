using TestGraph.Analysis.Lexing;

namespace TestGraph.Analysis.Tests.Lexing;

public sealed class LexerTests
{
    [Fact]
    public void Lex_ScholarshipSample_ProducesExpectedCoreTokens()
    {
        const string source = """
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
                Fin Si
            Fin Si
            """;

        var result = new Lexer(source).Lex();

        Assert.False(result.HasErrors);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.Entero);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.Real);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.Leer);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.Si);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.Entonces);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.Sino);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.Greater);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.GreaterOrEqual);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.Assignment);
        Assert.Equal(TokenKind.EndOfFile, result.Tokens[^1].Kind);
    }

    [Theory]
    [InlineData("si", TokenKind.Si)]
    [InlineData("SI", TokenKind.Si)]
    [InlineData("Si", TokenKind.Si)]
    [InlineData("logico", TokenKind.Logico)]
    [InlineData("Lógico", TokenKind.Logico)]
    public void Lex_Keywords_AreCaseInsensitive(string source, TokenKind expected)
    {
        var result = new Lexer(source).Lex();

        Assert.False(result.HasErrors);
        Assert.Equal(expected, result.Tokens[0].Kind);
    }

    [Fact]
    public void Lex_Operators_RecognizesTGPLComparisonAndAssignmentOperators()
    {
        const string source = "<- = <> > >= < <= + - * / %";

        var result = new Lexer(source).Lex();
        var kinds = result.Tokens
            .Where(token => token.Kind != TokenKind.EndOfFile)
            .Select(token => token.Kind)
            .ToArray();

        Assert.Equal(
            [
                TokenKind.Assignment,
                TokenKind.Equal,
                TokenKind.NotEqual,
                TokenKind.Greater,
                TokenKind.GreaterOrEqual,
                TokenKind.Less,
                TokenKind.LessOrEqual,
                TokenKind.Plus,
                TokenKind.Minus,
                TokenKind.Star,
                TokenKind.Slash,
                TokenKind.Percent
            ],
            kinds);
    }

    [Fact]
    public void Lex_Number_PreservesIntegerAndDecimalLiteral()
    {
        const string source = "9 7.5 2000";

        var result = new Lexer(source).Lex();
        var numbers = result.Tokens.Where(token => token.Kind == TokenKind.Number).ToArray();

        Assert.Equal(3, numbers.Length);
        Assert.Equal("9", numbers[0].Literal);
        Assert.Equal("7.5", numbers[1].Literal);
        Assert.Equal("2000", numbers[2].Literal);
    }

    [Fact]
    public void Lex_String_DecodesSupportedEscapes()
    {
        const string source = "Escribir \"Linea 1\\nLinea 2\"";

        var result = new Lexer(source).Lex();
        var stringToken = Assert.Single(result.Tokens, token => token.Kind == TokenKind.String);

        Assert.False(result.HasErrors);
        Assert.Equal("Linea 1\nLinea 2", stringToken.Literal);
    }

    [Fact]
    public void Lex_Comments_AreIgnoredButNewLinesRemainVisible()
    {
        const string source = "Leer edad # comentario\n// otra nota\nLeer promedio";

        var result = new Lexer(source).Lex();

        Assert.False(result.HasErrors);
        Assert.Equal(2, result.Tokens.Count(token => token.Kind == TokenKind.Leer));
        Assert.Equal(2, result.Tokens.Count(token => token.Kind == TokenKind.NewLine));
        Assert.DoesNotContain(result.Tokens, token => token.Lexeme.Contains("comentario", StringComparison.Ordinal));
    }

    [Fact]
    public void Lex_TracksSourceLocationAcrossLines()
    {
        const string source = "Leer edad\nSi edad > 18 Entonces";

        var result = new Lexer(source).Lex();
        var si = Assert.Single(result.Tokens, token => token.Kind == TokenKind.Si);

        Assert.Equal(2, si.Span.Start.Line);
        Assert.Equal(1, si.Span.Start.Column);
    }

    [Fact]
    public void Lex_UnexpectedCharacter_ReportsDiagnosticAndContinues()
    {
        const string source = "Leer @ edad";

        var result = new Lexer(source).Lex();

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("TGPL001", diagnostic.Code);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.Identifier && token.Lexeme == "edad");
    }

    [Fact]
    public void Lex_UnterminatedString_ReportsDiagnostic()
    {
        const string source = "Escribir \"mensaje";

        var result = new Lexer(source).Lex();

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("TGPL002", diagnostic.Code);
        Assert.Contains(result.Tokens, token => token.Kind == TokenKind.String);
    }
}
