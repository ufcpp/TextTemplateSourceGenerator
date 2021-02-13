using System;

namespace TextTemplateSourceGenerator.Parser
{
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
            SyntaxElementType.String => Text.AsSpan()[Range],
            SyntaxElementType.Identifier => Text.AsSpan()[Range][1..],
            SyntaxElementType.Expression => Text.AsSpan()[Range][2..^1],
            SyntaxElementType.Raw => Text.AsSpan()[Range][2..^2],
            _ => default,
        };

        public override string ToString() => Type + ": " + Text[Range];
    }
}
