using System.Text;
using TextTemplateSourceGenerator.Languages.TemplateA.Parser;

namespace TextTemplateSourceGenerator.Languages.TemplateA.Formatter;

public class TemplateFormatter
{
    public static void Format(StringBuilder builder, TemplateParser elements, string appendMethodName)
    {
        foreach (var x in elements)
        {
            switch (x.Type)
            {
                case SyntaxElementType.Invalid:
                    break;
                // simply skip invalid syntax elements.
                //throw new InvalidSyntaxException(x.Text, x.Range);
                case SyntaxElementType.String:
                    builder.Append($"""
                        {appendMethodName}(@"{x.Element.ToString()}");
                        """);
                    break;
                case SyntaxElementType.Identifier:
                case SyntaxElementType.Expression:
                    builder.Append($"""
                        {appendMethodName}({x.Element.ToString()});
                        """);
                    break;
                case SyntaxElementType.ControlStart:
                    builder.Append($$"""
                        {{x.Element.ToString()}}{
                        """);
                    break;
                case SyntaxElementType.Raw:
                    builder.Append(x.Element.ToString());
                    break;
            }
        }
    }
}
