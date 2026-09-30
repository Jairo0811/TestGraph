using TestGraph.Analysis.Lexing;
using TestGraph.Analysis.Syntax;

namespace TestGraph.Analysis.Parsing;

public sealed class Parser
{
    private readonly IReadOnlyList<Token> _tokens;
    private readonly IReadOnlyList<LexerDiagnostic> _lexerDiagnostics;
    private readonly List<ParserDiagnostic> _diagnostics = [];
    private int _current;

    public Parser(LexerResult lexerResult)
    {
        ArgumentNullException.ThrowIfNull(lexerResult);
        _tokens = lexerResult.Tokens;
        _lexerDiagnostics = lexerResult.Diagnostics;
    }

    public static ParserResult Parse(string source)
    {
        var lexerResult = new Lexer(source).Lex();
        return new Parser(lexerResult).Parse();
    }

    public ParserResult Parse()
    {
        SkipSeparators();

        string? programName = null;
        var start = Current.Span.Start;

        if (Current.Kind == TokenKind.Program)
        {
            Advance();
            if (Current.Kind == TokenKind.Identifier)
            {
                programName = Advance().Lexeme;
            }
            else
            {
                ReportExpected(TokenKind.Identifier, Current);
            }

            SkipToStatementEnd();
            SkipSeparators();
        }

        if (Current.Kind == TokenKind.Variables)
        {
            Advance();
            SkipToStatementEnd();
            SkipSeparators();
        }

        var statements = new List<StatementSyntax>();
        while (!IsAtEnd)
        {
            var before = _current;
            statements.Add(ParseStatement());
            if (_current == before)
            {
                Advance();
            }

            SkipSeparators();
        }

        var end = Current.Span.End;
        return new ParserResult(
            new CompilationUnitSyntax(programName, statements, new TextSpan(start, end)),
            _diagnostics,
            _lexerDiagnostics);
    }

    private StatementSyntax ParseStatement() =>
        Current.Kind switch
        {
            TokenKind.Entero or TokenKind.Real or TokenKind.Logico => ParseVariableDeclaration(),
            TokenKind.Leer => ParseReadStatement(),
            TokenKind.Escribir => ParseWriteStatement(),
            TokenKind.Identifier => ParseAssignmentStatement(),
            TokenKind.Si => ParseIfStatement(),
            TokenKind.Mientras => ParseWhileStatement(),
            TokenKind.Para => ParseForStatement(),
            _ => ParseUnexpectedStatement()
        };

    private VariableDeclarationSyntax ParseVariableDeclaration()
    {
        var type = Advance();
        var identifier = Match(TokenKind.Identifier);
        SkipToStatementEnd();
        return new VariableDeclarationSyntax(type, identifier, Span(type, identifier));
    }

    private ReadStatementSyntax ParseReadStatement()
    {
        var keyword = Advance();
        var identifier = Match(TokenKind.Identifier);
        SkipToStatementEnd();
        return new ReadStatementSyntax(keyword, identifier, Span(keyword, identifier));
    }

    private WriteStatementSyntax ParseWriteStatement()
    {
        var keyword = Advance();
        var expressions = new List<ExpressionSyntax>();

        if (!IsStatementEnd(Current.Kind))
        {
            do
            {
                expressions.Add(ParseExpression());
                if (Current.Kind != TokenKind.Comma)
                {
                    break;
                }

                Advance();
            }
            while (!IsStatementEnd(Current.Kind));
        }
        else
        {
            _diagnostics.Add(new ParserDiagnostic(
                "TGPL102",
                "Expected an expression after 'Escribir'.",
                Current.Span));
        }

        var end = expressions.Count > 0 ? expressions[^1].Span.End : keyword.Span.End;
        SkipToStatementEnd();
        return new WriteStatementSyntax(keyword, expressions, new TextSpan(keyword.Span.Start, end));
    }

    private AssignmentStatementSyntax ParseAssignmentStatement()
    {
        var identifier = Advance();
        var assignment = Match(TokenKind.Assignment);
        var expression = ParseExpression();
        var span = new TextSpan(identifier.Span.Start, expression.Span.End);
        SkipToStatementEnd();
        return new AssignmentStatementSyntax(identifier, assignment, expression, span);
    }

    private IfStatementSyntax ParseIfStatement()
    {
        var ifToken = Advance();
        var condition = ParseExpression();
        Match(TokenKind.Entonces);
        ConsumeLineBoundary();

        var thenStatements = ParseBlockUntil(TokenKind.Sino, TokenKind.Fin);
        var elseIfClauses = new List<ElseIfClauseSyntax>();
        var elseStatements = new List<StatementSyntax>();

        while (Current.Kind == TokenKind.Sino)
        {
            var sino = Advance();

            if (Current.Kind == TokenKind.Si)
            {
                Advance();
                var elseIfCondition = ParseExpression();
                Match(TokenKind.Entonces);
                ConsumeLineBoundary();

                var statements = ParseBlockUntil(TokenKind.Sino, TokenKind.Fin);
                var end = statements.Count > 0 ? statements[^1].Span.End : elseIfCondition.Span.End;
                elseIfClauses.Add(new ElseIfClauseSyntax(
                    elseIfCondition,
                    statements,
                    new TextSpan(sino.Span.Start, end)));
                continue;
            }

            ConsumeLineBoundary();
            elseStatements.AddRange(ParseBlockUntil(TokenKind.Fin));
            break;
        }

        var fin = Match(TokenKind.Fin);
        var si = Match(TokenKind.Si);
        var endToken = si.Kind == TokenKind.Si ? si : fin;
        SkipToStatementEnd();

        return new IfStatementSyntax(
            condition,
            thenStatements,
            elseIfClauses,
            elseStatements,
            new TextSpan(ifToken.Span.Start, endToken.Span.End));
    }

    private WhileStatementSyntax ParseWhileStatement()
    {
        var whileToken = Advance();
        var condition = ParseExpression();
        Match(TokenKind.Hacer);
        ConsumeLineBoundary();

        var statements = ParseBlockUntil(TokenKind.Fin);
        Match(TokenKind.Fin);
        var endToken = Match(TokenKind.Mientras);
        SkipToStatementEnd();

        return new WhileStatementSyntax(
            condition,
            statements,
            new TextSpan(whileToken.Span.Start, endToken.Span.End));
    }

    private ForStatementSyntax ParseForStatement()
    {
        var forToken = Advance();
        var identifier = Match(TokenKind.Identifier);
        Match(TokenKind.Equal);
        var start = ParseExpression();
        Match(TokenKind.Hasta);
        var end = ParseExpression();

        ExpressionSyntax? step = null;
        if (Current.Kind == TokenKind.Paso)
        {
            Advance();
            step = ParseExpression();
        }

        ConsumeLineBoundary();
        var statements = ParseBlockUntil(TokenKind.Fin);
        Match(TokenKind.Fin);
        var endToken = Match(TokenKind.Para);
        SkipToStatementEnd();

        return new ForStatementSyntax(
            identifier,
            start,
            end,
            step,
            statements,
            new TextSpan(forToken.Span.Start, endToken.Span.End));
    }

    private IReadOnlyList<StatementSyntax> ParseBlockUntil(params TokenKind[] stopKinds)
    {
        var statements = new List<StatementSyntax>();
        SkipSeparators();

        while (!IsAtEnd && !stopKinds.Contains(Current.Kind))
        {
            var before = _current;
            statements.Add(ParseStatement());
            if (_current == before)
            {
                Advance();
            }

            SkipSeparators();
        }

        return statements;
    }

    private StatementSyntax ParseUnexpectedStatement()
    {
        var token = Current;
        _diagnostics.Add(new ParserDiagnostic(
            "TGPL100",
            $"Unexpected token '{token.Lexeme}' at the start of a statement.",
            token.Span));

        SkipToStatementEnd();
        return new ErrorStatementSyntax(token, token.Span);
    }

    private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var unaryPrecedence = GetUnaryPrecedence(Current.Kind);

        if (unaryPrecedence > 0 && unaryPrecedence >= parentPrecedence)
        {
            var operatorToken = Advance();
            var operand = ParseExpression(unaryPrecedence);
            left = new UnaryExpressionSyntax(
                operatorToken,
                operand,
                new TextSpan(operatorToken.Span.Start, operand.Span.End));
        }
        else
        {
            left = ParsePrimaryExpression();
        }

        while (true)
        {
            var precedence = GetBinaryPrecedence(Current.Kind);
            if (precedence == 0 || precedence <= parentPrecedence)
            {
                break;
            }

            var operatorToken = Advance();
            var right = ParseExpression(precedence);
            left = new BinaryExpressionSyntax(
                left,
                operatorToken,
                right,
                new TextSpan(left.Span.Start, right.Span.End));
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case TokenKind.Number:
            {
                var token = Advance();
                return new LiteralExpressionSyntax(token, token.Literal, token.Span);
            }
            case TokenKind.String:
            {
                var token = Advance();
                return new LiteralExpressionSyntax(token, token.Literal, token.Span);
            }
            case TokenKind.Verdadero:
            case TokenKind.Falso:
            {
                var token = Advance();
                return new LiteralExpressionSyntax(
                    token,
                    token.Kind == TokenKind.Verdadero,
                    token.Span);
            }
            case TokenKind.Identifier:
            {
                var token = Advance();
                return new NameExpressionSyntax(token, token.Span);
            }
            case TokenKind.LeftParenthesis:
            {
                var open = Advance();
                var expression = ParseExpression();
                var close = Match(TokenKind.RightParenthesis);
                return new ParenthesizedExpressionSyntax(
                    open,
                    expression,
                    close,
                    new TextSpan(open.Span.Start, close.Span.End));
            }
            default:
            {
                var token = Current;
                _diagnostics.Add(new ParserDiagnostic(
                    "TGPL102",
                    $"Expected expression but found '{token.Lexeme}'.",
                    token.Span));

                if (!IsExpressionBoundary(token.Kind) && !IsAtEnd)
                {
                    Advance();
                }

                return new ErrorExpressionSyntax(token, token.Span);
            }
        }
    }

    private void ConsumeLineBoundary()
    {
        if (Current.Kind == TokenKind.Semicolon)
        {
            Advance();
        }

        if (Current.Kind == TokenKind.NewLine)
        {
            SkipSeparators();
            return;
        }

        if (!IsAtEnd)
        {
            _diagnostics.Add(new ParserDiagnostic(
                "TGPL103",
                "Expected end of line.",
                Current.Span));
            SkipToStatementEnd();
            SkipSeparators();
        }
    }

    private void SkipToStatementEnd()
    {
        while (!IsAtEnd && Current.Kind is not TokenKind.NewLine and not TokenKind.Semicolon)
        {
            Advance();
        }

        if (Current.Kind == TokenKind.Semicolon)
        {
            Advance();
        }
    }

    private void SkipSeparators()
    {
        while (Current.Kind is TokenKind.NewLine or TokenKind.Semicolon)
        {
            Advance();
        }
    }

    private Token Match(TokenKind expected)
    {
        if (Current.Kind == expected)
        {
            return Advance();
        }

        ReportExpected(expected, Current);
        var position = Current.Span.Start;
        return new Token(expected, string.Empty, null, new TextSpan(position, position));
    }

    private void ReportExpected(TokenKind expected, Token actual)
    {
        _diagnostics.Add(new ParserDiagnostic(
            "TGPL101",
            $"Expected {expected} but found {actual.Kind}.",
            actual.Span));
    }

    private Token Advance()
    {
        var current = Current;
        if (!IsAtEnd)
        {
            _current++;
        }

        return current;
    }

    private Token Current => Peek(0);

    private Token Peek(int offset)
    {
        var index = _current + offset;
        return index >= _tokens.Count ? _tokens[^1] : _tokens[index];
    }

    private bool IsAtEnd => Current.Kind == TokenKind.EndOfFile;

    private static bool IsStatementEnd(TokenKind kind) =>
        kind is TokenKind.NewLine or TokenKind.Semicolon or TokenKind.EndOfFile;

    private static bool IsExpressionBoundary(TokenKind kind) =>
        IsStatementEnd(kind) ||
        kind is TokenKind.Comma
            or TokenKind.Entonces
            or TokenKind.Hacer
            or TokenKind.Hasta
            or TokenKind.Paso
            or TokenKind.Sino
            or TokenKind.Fin;

    private static int GetUnaryPrecedence(TokenKind kind) =>
        kind switch
        {
            TokenKind.No or TokenKind.Plus or TokenKind.Minus => 6,
            _ => 0
        };

    private static int GetBinaryPrecedence(TokenKind kind) =>
        kind switch
        {
            TokenKind.O => 1,
            TokenKind.Y => 2,
            TokenKind.Equal or TokenKind.NotEqual
                or TokenKind.Greater or TokenKind.GreaterOrEqual
                or TokenKind.Less or TokenKind.LessOrEqual => 3,
            TokenKind.Plus or TokenKind.Minus => 4,
            TokenKind.Star or TokenKind.Slash or TokenKind.Percent => 5,
            _ => 0
        };

    private static TextSpan Span(Token first, Token last) =>
        new(first.Span.Start, last.Span.End);
}
