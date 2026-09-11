namespace TestGraph.Analysis.Lexing;

public enum TokenKind
{
    EndOfFile,
    NewLine,

    Identifier,
    Number,
    String,

    Program,
    Variables,
    Entero,
    Real,
    Logico,
    Leer,
    Escribir,
    Si,
    Entonces,
    Sino,
    Fin,
    Mientras,
    Hacer,
    Para,
    Hasta,
    Paso,
    Y,
    O,
    No,
    Verdadero,
    Falso,

    LeftParenthesis,
    RightParenthesis,
    Comma,
    Colon,
    Semicolon,

    Plus,
    Minus,
    Star,
    Slash,
    Percent,

    Assignment,
    Equal,
    NotEqual,
    Greater,
    GreaterOrEqual,
    Less,
    LessOrEqual
}
