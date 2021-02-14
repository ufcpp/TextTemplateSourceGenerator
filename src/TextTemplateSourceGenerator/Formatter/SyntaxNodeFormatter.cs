using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using TextTemplateSourceGenerator.Parser;

namespace TextTemplateSourceGenerator.Formatter
{
    public class SyntaxNodeFormatter
    {
        public static string Format(TypeDeclarationSyntax type, IEnumerable<MethodDeclarationSyntax> methods, Func<MemberDeclarationSyntax, (TemplateParser elements, string appendMethodName)> getSyntaxElements)
        {
            var sb = new StringBuilder();
            sb.Append(@"#pragma warning disable 8019
");

            var n = AppendDeclarations(sb, type);
            foreach (var m in methods)
            {
                AppendMethodSignature(sb, m);
                var (e, a) = getSyntaxElements(m);
                AppendBody(sb, e, a);
            }
            AppendClose(sb, n);

            return sb.ToString();
        }

        private static void AppendBody(StringBuilder sb, TemplateParser elements, string appendMethodName)
        {
            sb.Append(@"
{
");
            TemplateFormatter.Format(sb, elements, appendMethodName);

            sb.Append(@"
}
");
        }

        private static void AppendMethodSignature(StringBuilder sb, MethodDeclarationSyntax m)
        {
            foreach (var mod in m.Modifiers)
            {
                sb.Append(mod.Text);
                sb.Append(' ');
            }

            sb.Append(m.ReturnType);
            sb.Append(' ');

            sb.Append(m.Identifier.Text);

            sb.Append(m.ParameterList.ToFullString());
        }

        private static int AppendDeclarations(StringBuilder sb, SyntaxNode? node)
        {
            if (node is null) return 0;

            var nest = AppendDeclarations(sb, node.Parent);

            switch (node)
            {
                case CompilationUnitSyntax c:
                    AppendUsings(sb, c);
                    return nest;
                case NamespaceDeclarationSyntax ns:
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
            sb.Append(@" {
");
        }

        private static void AppendNamespaceOpen(StringBuilder sb, NamespaceDeclarationSyntax ns)
        {
            sb.Append("namespace ");
            sb.Append(ns.Name.ToString());
            sb.Append(@" {
");
        }

        private static void AppendUsings(StringBuilder sb, CompilationUnitSyntax c)
        {
            foreach (var u in c.Usings)
            {
                sb.Append(u.ToFullString());
            }
        }

        private static void AppendClose(StringBuilder sb, int nest)
        {
            for (int i = 0; i < nest; i++)
            {
                sb.Append('}');
            }
            sb.Append(@"
");
        }
    }
}
