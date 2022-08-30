using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TextTemplateSourceGenerator;

partial class TextTemplateGenerator
{
    private const string PreprocessorAttributeName = "TextTemplate.TemplatePreprocessorAttribute";

    private static IncrementalValueProvider<System.Collections.Immutable.ImmutableArray<PreprocessorTemplate>> FilterPreprocessor(IncrementalGeneratorInitializationContext context)
        => context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 },
                static (context, _) => GetSemanticTargetForPreprocessorGeneration(context.SemanticModel, (MethodDeclarationSyntax)context.Node)!
                )
            .Where(x => x is not null)
            .Collect();

    private static PreprocessorTemplate? GetSemanticTargetForPreprocessorGeneration(SemanticModel semanticModel, MethodDeclarationSyntax m)
    {
        if (m.ParameterList.Parameters.Count == 0) return null;
        if (!m.Modifiers.Any(m => m.ValueText == "partial")) return null;
        if (GetAttribute(semanticModel, m, PreprocessorAttributeName) is not { } a) return null;
        if (a.Arguments.Count == 0) return null;

        var template = (string)semanticModel.GetConstantValue(a.Arguments[0].Expression).Value!;

        string? appendMethodName = null;
        if(a.Arguments.Count >= 2) appendMethodName = (string?)semanticModel.GetConstantValue(a.Arguments[1].Expression).Value;

        return new(m, template, appendMethodName);
    }

    private static AttributeArgumentListSyntax? GetAttribute(SemanticModel semanticModel, MemberDeclarationSyntax m, string attributeFullName)
    {
        foreach (var list in m.AttributeLists)
        {
            foreach (var a in list.Attributes)
            {
                if (semanticModel.GetSymbolInfo(a).Symbol is { ContainingType: var t }
                    && t.ToDisplayString() == PreprocessorAttributeName)
                    return a.ArgumentList;
            }
        }

        return null;
    }
}
