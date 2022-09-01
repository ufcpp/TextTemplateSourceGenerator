using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ScribanSourceGenerator;

public record ClassNameTemplate(TypeDeclarationSyntax Type, string Template)
{
    public static ClassNameTemplate? Create(SemanticModel semanticModel, TypeDeclarationSyntax type)
    {
        if (!type.Modifiers.Any(m => m.ValueText == "partial")) return null;
        if (GetAttribute(semanticModel, type, "ScribanGeneretor.ClassMemberAttribute") is not { } a) return null;
        if (a.Arguments.Count == 0) return null;

        var template = (string)semanticModel.GetConstantValue(a.Arguments[0].Expression).Value!;
        return new(type, template);
    }

    private static AttributeArgumentListSyntax? GetAttribute(SemanticModel semanticModel, MemberDeclarationSyntax m, string attributeFullName)
    {
        foreach (var list in m.AttributeLists)
        {
            foreach (var a in list.Attributes)
            {
                if (semanticModel.GetSymbolInfo(a).Symbol is { ContainingType: var t }
                    && t.ToDisplayString() == attributeFullName)
                    return a.ArgumentList;
            }
        }

        return null;
    }
}
