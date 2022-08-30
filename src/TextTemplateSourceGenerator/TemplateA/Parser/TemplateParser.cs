namespace TextTemplateSourceGenerator.TemplateA.Parser;

public readonly struct TemplateParser
{
    public string Text { get; }
    public TemplateParser(string text) => Text = text;
    public TemplateParserEnumerator GetEnumerator() => new(Text);

    public bool IsNull => Text is null;
}
