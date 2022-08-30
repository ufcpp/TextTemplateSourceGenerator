using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using TextTemplateSourceGenerator.Languages.TemplateA.Formatter;

namespace TextTemplateSourceGenerator;

partial class TextTemplateGenerator
{
    private void EmitClassMember(SourceProductionContext context, ImmutableArray<ClassMemberTemplate> templates)
    {
        var buffer = new StringBuilder();

        var ordinal = 0;
        foreach (var (type, template) in templates)
        {
            var hintPath = GetClassMemberFilename(type, ordinal++, buffer);
            var generatedSource = ""; //todo: SyntaxNodeFormatter.Format(m, t, a);
            context.AddSource(hintPath, SourceText.From(generatedSource, Encoding.UTF8));
        }
    }

    private static string GetClassMemberFilename(TypeDeclarationSyntax type, int ordinal, StringBuilder buffer)
    {
        buffer.Clear();
        buffer.Append($"{ordinal}_{type.Identifier.Text}_membertemplate.cs");
        return buffer.ToString();
    }
}
