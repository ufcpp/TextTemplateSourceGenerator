namespace TextTemplateSourceGenerator.Parser
{
    public struct TemplateParser
    {
        public string Text { get; }
        public TemplateParser(string text) => Text = text;
        public TemplateParserEnumerator GetEnumerator() => new(Text);
    }
}
