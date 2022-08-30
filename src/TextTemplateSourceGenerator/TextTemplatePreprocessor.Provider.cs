using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TextTemplateSourceGenerator;

partial class TextTemplatePreprocessor
{
    private const string methodAttributeName = "TextTemplate.TemplatePreprocessorAttribute";

    private static IncrementalValueProvider<System.Collections.Immutable.ImmutableArray<MethodTemplate>> Filter(IncrementalGeneratorInitializationContext context)
        => context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => IsSyntaxTargetForGeneration(node),
                static (context, _) => GetSemanticTargetForGeneration(context.SemanticModel, (MethodDeclarationSyntax)context.Node)!
                )
            .Where(x => x is not null)
            .Collect();

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };

    private static MethodTemplate? GetSemanticTargetForGeneration(SemanticModel semanticModel, MethodDeclarationSyntax m)
    {
        if (m.ParameterList.Parameters.Count == 0) return null;
        if (!m.Modifiers.Any(m => m.ValueText == "partial")) return null;
        if (GetMethodTemplateAttribute(semanticModel, m) is not { } a) return null;
        if (a.Arguments.Count == 0) return null;

        var template = (string)semanticModel.GetConstantValue(a.Arguments[0].Expression).Value!;

        string? appendMethodName = null;
        if(a.Arguments.Count >= 2) appendMethodName = (string?)semanticModel.GetConstantValue(a.Arguments[1].Expression).Value;

        return new(m, template, appendMethodName);
    }

    private static AttributeArgumentListSyntax? GetMethodTemplateAttribute(SemanticModel semanticModel, MemberDeclarationSyntax m)
    {
        foreach (var list in m.AttributeLists)
        {
            foreach (var a in list.Attributes)
            {
                if (semanticModel.GetSymbolInfo(a).Symbol is { ContainingType: var t }
                    && t.ToDisplayString() == methodAttributeName)
                    return a.ArgumentList;
            }
        }

        return null;
    }
}
