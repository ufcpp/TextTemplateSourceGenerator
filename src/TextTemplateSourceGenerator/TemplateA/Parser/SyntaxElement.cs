namespace TextTemplateSourceGenerator.TemplateA.Parser;

public readonly struct SyntaxElement
{
    public SyntaxElementType Type { get; }
    public string Text { get; }
    public Range Range { get; }

    public SyntaxElement(SyntaxElementType type, string text, Range range)
    {
        Type = type;
        Text = text;
        Range = range;
    }

    public ReadOnlySpan<char> Element => Type switch
    {
        SyntaxElementType.String => Text.AsSpan(Range.Start, Range.Length),
        SyntaxElementType.Identifier => Text.AsSpan(Range.Start + 1, Range.Length - 1),
        SyntaxElementType.Expression => Text.AsSpan(Range.Start + 2, Range.Length - 3),
        SyntaxElementType.ControlStart => Text.AsSpan(Range.Start + 1, Range.Length - 3),
        SyntaxElementType.Raw => Text.AsSpan(Range.Start + 2, Range.Length - 4),
        SyntaxElementType.EndOfLine => default,
        _ => default,
    };

    public override string ToString() => Type + ": " + Text.Substring(Range.Start, Range.Length);
}
