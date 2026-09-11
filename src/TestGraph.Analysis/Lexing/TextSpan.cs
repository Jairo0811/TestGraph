namespace TestGraph.Analysis.Lexing;

public readonly record struct TextSpan(TextPosition Start, TextPosition End)
{
    public int Length => End.Offset - Start.Offset;
}
