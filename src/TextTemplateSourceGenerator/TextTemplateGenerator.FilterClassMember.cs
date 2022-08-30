using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace TextTemplateSourceGenerator;

public record ClassMemberTemplate(TypeDeclarationSyntax Method, string Template);

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

        var template = (string)semanticModel.GetConstantValue(a.Arguments[0].Expression).Value!;

        return new(t, template);
    }
}
