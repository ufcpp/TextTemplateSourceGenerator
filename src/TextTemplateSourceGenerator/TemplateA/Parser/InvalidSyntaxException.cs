namespace TextTemplateSourceGenerator.TemplateA.Parser;

public class InvalidSyntaxException : InvalidOperationException
{
    public string Text { get; }

    public Range Range { get; }

    public InvalidSyntaxException(string text, Range range)
    {
        Text = text;
        Range = range;
    }
}
