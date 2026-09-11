using System.Text;

namespace TestGraph.Analysis.Lexing;

public sealed class Lexer
{
    private static readonly IReadOnlyDictionary<string, TokenKind> Keywords =
        new Dictionary<string, TokenKind>(StringComparer.OrdinalIgnoreCase)
        {
            ["Programa"] = TokenKind.Program,
            ["Variables"] = TokenKind.Variables,
            ["Entero"] = TokenKind.Entero,
            ["Real"] = TokenKind.Real,
            ["Logico"] = TokenKind.Logico,
            ["Lógico"] = TokenKind.Logico,
            ["Leer"] = TokenKind.Leer,
            ["Escribir"] = TokenKind.Escribir,
            ["Si"] = TokenKind.Si,
            ["Entonces"] = TokenKind.Entonces,
            ["Sino"] = TokenKind.Sino,
            ["Fin"] = TokenKind.Fin,
            ["Mientras"] = TokenKind.Mientras,
            ["Hacer"] = TokenKind.Hacer,
            ["Para"] = TokenKind.Para,
            ["Hasta"] = TokenKind.Hasta,
            ["Paso"] = TokenKind.Paso,
            ["Y"] = TokenKind.Y,
            ["O"] = TokenKind.O,
            ["NO"] = TokenKind.No,
            ["Verdadero"] = TokenKind.Verdadero,
            ["Falso"] = TokenKind.Falso
        };

    private readonly string _source;
    private readonly List<Token> _tokens = [];
    private readonly List<LexerDiagnostic> _diagnostics = [];
    private int _current;
    private int _line = 1;
    private int _column = 1;

    public Lexer(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        _source = source;
    }

    public LexerResult Lex()
    {
        while (!IsAtEnd)
        {
            ScanToken();
        }

        var position = Position;
        _tokens.Add(new Token(
            TokenKind.EndOfFile,
            string.Empty,
            null,
            new TextSpan(position, position)));

        return new LexerResult(_tokens, _diagnostics);
    }

    private bool IsAtEnd => _current >= _source.Length;

    private TextPosition Position => new(_current, _line, _column);

    private void ScanToken()
    {
        var start = Position;
        var c = Peek();

        switch (c)
        {
            case ' ':
            case '\t':
            case '\f':
                SkipHorizontalWhitespace();
                return;
            case '\r':
            case '\n':
                ReadNewLine(start);
                return;
            case '#':
                SkipLineComment();
                return;
            case '/':
                if (PeekNext() == '/')
                {
                    SkipLineComment();
                    return;
                }

                Advance();
                AddToken(TokenKind.Slash, start);
                return;
            case '(':
                Advance();
                AddToken(TokenKind.LeftParenthesis, start);
                return;
            case ')':
                Advance();
                AddToken(TokenKind.RightParenthesis, start);
                return;
            case ',':
                Advance();
                AddToken(TokenKind.Comma, start);
                return;
            case ':':
                Advance();
                AddToken(TokenKind.Colon, start);
                return;
            case ';':
                Advance();
                AddToken(TokenKind.Semicolon, start);
                return;
            case '+':
                Advance();
                AddToken(TokenKind.Plus, start);
                return;
            case '-':
                Advance();
                AddToken(TokenKind.Minus, start);
                return;
            case '*':
                Advance();
                AddToken(TokenKind.Star, start);
                return;
            case '%':
                Advance();
                AddToken(TokenKind.Percent, start);
                return;
            case '=':
                Advance();
                AddToken(TokenKind.Equal, start);
                return;
            case '>':
                Advance();
                AddToken(Match('=') ? TokenKind.GreaterOrEqual : TokenKind.Greater, start);
                return;
            case '<':
                Advance();
                if (Match('-'))
                {
                    AddToken(TokenKind.Assignment, start);
                }
                else if (Match('='))
                {
                    AddToken(TokenKind.LessOrEqual, start);
                }
                else if (Match('>'))
                {
                    AddToken(TokenKind.NotEqual, start);
                }
                else
                {
                    AddToken(TokenKind.Less, start);
                }

                return;
            case '"':
                ReadString(start);
                return;
        }

        if (char.IsDigit(c))
        {
            ReadNumber(start);
            return;
        }

        if (IsIdentifierStart(c))
        {
            ReadIdentifier(start);
            return;
        }

        Advance();
        _diagnostics.Add(new LexerDiagnostic(
            "TGPL001",
            $"Unexpected character '{c}'.",
            new TextSpan(start, Position)));
    }

    private void SkipHorizontalWhitespace()
    {
        while (!IsAtEnd && Peek() is ' ' or '\t' or '\f')
        {
            Advance();
        }
    }

    private void ReadNewLine(TextPosition start)
    {
        var startOffset = _current;

        if (Peek() == '\r')
        {
            _current++;
            if (!IsAtEnd && Peek() == '\n')
            {
                _current++;
            }
        }
        else
        {
            _current++;
        }

        _line++;
        _column = 1;

        var lexeme = _source[startOffset.._current];
        _tokens.Add(new Token(TokenKind.NewLine, lexeme, null, new TextSpan(start, Position)));
    }

    private void SkipLineComment()
    {
        while (!IsAtEnd && Peek() is not '\r' and not '\n')
        {
            Advance();
        }
    }

    private void ReadNumber(TextPosition start)
    {
        while (char.IsDigit(Peek()))
        {
            Advance();
        }

        if (Peek() == '.' && char.IsDigit(PeekNext()))
        {
            Advance();
            while (char.IsDigit(Peek()))
            {
                Advance();
            }
        }

        AddToken(TokenKind.Number, start, CurrentLexeme(start.Offset));
    }

    private void ReadIdentifier(TextPosition start)
    {
        while (IsIdentifierPart(Peek()))
        {
            Advance();
        }

        var lexeme = CurrentLexeme(start.Offset);
        var kind = Keywords.TryGetValue(lexeme, out var keyword)
            ? keyword
            : TokenKind.Identifier;

        AddToken(kind, start);
    }

    private void ReadString(TextPosition start)
    {
        Advance();
        var literal = new StringBuilder();
        var terminated = false;

        while (!IsAtEnd)
        {
            var c = Peek();
            if (c == '"')
            {
                Advance();
                terminated = true;
                break;
            }

            if (c is '\r' or '\n')
            {
                break;
            }

            if (c == '\\')
            {
                Advance();
                if (IsAtEnd)
                {
                    break;
                }

                var escaped = Peek();
                Advance();
                literal.Append(escaped switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '"' => '"',
                    '\\' => '\\',
                    _ => escaped
                });
                continue;
            }

            literal.Append(c);
            Advance();
        }

        if (!terminated)
        {
            _diagnostics.Add(new LexerDiagnostic(
                "TGPL002",
                "Unterminated string literal.",
                new TextSpan(start, Position)));
        }

        AddToken(TokenKind.String, start, literal.ToString());
    }

    private void AddToken(TokenKind kind, TextPosition start, string? literal = null)
    {
        _tokens.Add(new Token(
            kind,
            CurrentLexeme(start.Offset),
            literal,
            new TextSpan(start, Position)));
    }

    private string CurrentLexeme(int startOffset) => _source[startOffset.._current];

    private char Peek() => IsAtEnd ? '\0' : _source[_current];

    private char PeekNext() => _current + 1 >= _source.Length ? '\0' : _source[_current + 1];

    private void Advance()
    {
        if (IsAtEnd)
        {
            return;
        }

        _current++;
        _column++;
    }

    private bool Match(char expected)
    {
        if (IsAtEnd || Peek() != expected)
        {
            return false;
        }

        Advance();
        return true;
    }

    private static bool IsIdentifierStart(char c) => c == '_' || char.IsLetter(c);

    private static bool IsIdentifierPart(char c) => c == '_' || char.IsLetterOrDigit(c);
}
