﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Text;

namespace ScribanSourceGenerator;

public class SyntaxNodeHelper
{
    public static int AppendDeclarations(StringBuilder sb, SyntaxNode? node)
    {
        if (node is null) return 0;

        var nest = AppendDeclarations(sb, node.Parent);

        switch (node)
        {
            case CompilationUnitSyntax c:
                AppendUsings(sb, c);
                return nest;
            case BaseNamespaceDeclarationSyntax ns:
                AppendNamespaceOpen(sb, ns);
                return nest + 1;
            case TypeDeclarationSyntax t:
                AppendTypeOpen(sb, t);
                return nest + 1;
            default:
                return nest;
        }
    }

    private static void AppendTypeOpen(StringBuilder sb, TypeDeclarationSyntax t)
    {
        sb.Append("partial ");
        sb.Append(t.Keyword.Text);
        sb.Append(' ');
        sb.Append(t.Identifier.Text);

        if (t.TypeParameterList is { } tl)
        {
            sb.Append('<');
            var first = true;
            foreach (var tp in tl.Parameters)
            {
                if (first) first = false;
                else sb.Append(", ");
                sb.Append(tp.Identifier.Text);
            }
            sb.Append('>');
        }
        sb.Append("""
             {

            """);
    }

    private static void AppendNamespaceOpen(StringBuilder sb, BaseNamespaceDeclarationSyntax ns)
    {
        sb.Append($$"""
        namespace {{ns.Name}} {

        """);
    }

    private static void AppendUsings(StringBuilder sb, CompilationUnitSyntax c)
    {
        foreach (var u in c.Usings)
        {
            sb.Append(u.ToFullString());
        }
    }

    public static void AppendClose(StringBuilder sb, int nest)
    {
        for (int i = 0; i < nest; i++)
        {
            sb.Append('}');
        }
        sb.Append("""


            """);
    }
}
