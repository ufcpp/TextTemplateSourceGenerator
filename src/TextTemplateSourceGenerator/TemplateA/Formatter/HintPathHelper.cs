using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace TextTemplateSourceGenerator.TemplateA.Formatter;

public class HintPathHelper
{
    public static string GetHintPath(MethodDeclarationSyntax method)
    {
        var sb = new StringBuilder();

        AppendTypeName(sb, method);

        sb.Append(method.Identifier.Text);

        sb.Append(".cs");
        return sb.ToString();
    }

    private static void AppendTypeName(StringBuilder sb, SyntaxNode? node)
    {
        if (node is null) return;

        AppendTypeName(sb, node.Parent);

        switch (node)
        {
            case NamespaceDeclarationSyntax ns:
                sb.Append(ns.Name.ToString());
                sb.Append('.');
                break;
            case TypeDeclarationSyntax t:
                sb.Append(t.Identifier.Text);

                if (t.TypeParameterList is { } tl)
                {
                    sb.Append('`');
                    sb.Append(tl.Parameters.Count);
                }
                sb.Append('.');
                break;
        }
    }
}
