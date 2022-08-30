using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TextTemplateSourceGenerator;

public record MethodTemplate(MethodDeclarationSyntax Method, string Template, string? AppendMethodName);
