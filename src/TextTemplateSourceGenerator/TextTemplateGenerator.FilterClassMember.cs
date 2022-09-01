using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using TextTemplateSourceGenerator.Languages.TemplateA.Formatter;

namespace TextTemplateSourceGenerator;

public record ClassMemberTemplate(TypeDeclarationSyntax Type, string Template, int LanguageCode)
{
    public string Format() => SyntaxNodeFormatter.FormatClassMember(Type, Template, LanguageCode);
}

partial class TextTemplateGenerator
{
    private const string ClassMemberAttributeName = "TextTemplate.TemplateClassMemberAttribute";

    private static IncrementalValueProvider<System.Collections.Immutable.ImmutableArray<ClassMemberTemplate>> FilterClassMember(IncrementalGeneratorInitializationContext context)
        => context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 },
                static (context, _) => GetSemanticTargetForClassMemberGeneration(context.SemanticModel, (TypeDeclarationSyntax)context.Node)!
                )
            .Where(x => x is not null)
            .Collect();

    private static ClassMemberTemplate? GetSemanticTargetForClassMemberGeneration(SemanticModel semanticModel, TypeDeclarationSyntax t)
    {
        if (!t.Modifiers.Any(m => m.ValueText == "partial")) return null;
        if (GetAttribute(semanticModel, t, ClassMemberAttributeName) is not { } a) return null;
        if (a.Arguments.Count == 0) return null;


        int language = 0;
        if (a.Arguments.FirstOrDefault(arg => arg.NameEquals is { Name.Identifier.Value: "Language" }) is { } langArg)
        {
            language = (int?)semanticModel.GetConstantValue(langArg.Expression).Value ?? 0;
        }

        var template = (string)semanticModel.GetConstantValue(a.Arguments[0].Expression).Value!;

        return new(t, template, language);
    }
}
