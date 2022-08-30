using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using TextTemplateSourceGenerator.TemplateA.Parser;

namespace TextTemplateSourceGenerator.TemplateA.Formatter;

public class SymbolFormatter
{
    public static string Format(IMethodSymbol method, TemplateParser elements, string appendMethodName)
    {
        var sb = new StringBuilder();
        sb.Append("""
            #pragma warning disable 8019

            """);

        var n = AppendDeclarations(sb, method.ContainingType);
        //foreach (var m in methods)
        //{
        //    AppendMethodSignature(sb, m);
        //    var (e, a) = getSyntaxElements(m);
        //    AppendBody(sb, e, a);
        //}
        //AppendClose(sb, n);

        return sb.ToString();
    }

    private static int AppendDeclarations(StringBuilder sb, INamedTypeSymbol node)
    {
        if (node is null) return 0;

        //var nest = AppendDeclarations(sb, node.Parent);

        //switch (node)
        //{
        //    case CompilationUnitSyntax c:
        //        AppendUsings(sb, c);
        //        return nest;
        //    case NamespaceDeclarationSyntax ns:
        //        AppendNamespaceOpen(sb, ns);
        //        return nest + 1;
        //    case TypeDeclarationSyntax t:
        //        AppendTypeOpen(sb, t);
        //        return nest + 1;
        //    default:
        //        return nest;
        //}
        return 0;
    }
}
