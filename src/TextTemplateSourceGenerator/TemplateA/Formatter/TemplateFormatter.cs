using System.Text;
using TextTemplateSourceGenerator.TemplateA.Parser;

namespace TextTemplateSourceGenerator.TemplateA.Formatter;

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
                    builder.Append(appendMethodName);
                    builder.Append("""
                        (@"
                        """);
                    builder.Append(x.Element.ToString());
                    builder.Append("""
                        ");
                        """);
                    break;
                case SyntaxElementType.Identifier:
                case SyntaxElementType.Expression:
                    builder.Append(appendMethodName);
                    builder.Append("(");
                    builder.Append(x.Element.ToString());
                    builder.Append(");");
                    break;
                case SyntaxElementType.ControlStart:
                    builder.Append(x.Element.ToString());
                    builder.Append('{');
                    break;
                case SyntaxElementType.Raw:
                    builder.Append(x.Element.ToString());
                    break;
            }
        }

        // builder.Append(x.Element.ToString());
        // can be replaced to
        // builder.Append(x.Element);
        // if TargetFramework is netcoreapp2.1 or greater
        // or
        // fixed (char* p = x.Element) { builder.Append(p, x.Element.Length); }
        // with unsafe option.
    }
}
