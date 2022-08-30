using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using TextTemplateSourceGenerator.Languages.TemplateA.Formatter;

namespace TextTemplateSourceGenerator;

partial class TextTemplateGenerator
{
    private void EmitPreprocessor(SourceProductionContext context, ImmutableArray<MethodTemplate> methods)
    {
        var buffer = new StringBuilder();

        var ordinal = 0;
        foreach (var (m, t, a) in methods)
        {
            var hintPath = GetFilename(m, ordinal++, buffer);
            var generatedSource = SyntaxNodeFormatter.Format(m, t, a);
            context.AddSource(hintPath, SourceText.From(generatedSource, Encoding.UTF8));
        }
    }

    private static string GetFilename(MethodDeclarationSyntax method, int ordinal, StringBuilder buffer)
    {
        buffer.Clear();
        var t = (TypeDeclarationSyntax)method.Parent!;
        buffer.Append($"{ordinal}_{t.Identifier.Text}_{method.Identifier.Text}_template.cs");
        return buffer.ToString();
    }
}
