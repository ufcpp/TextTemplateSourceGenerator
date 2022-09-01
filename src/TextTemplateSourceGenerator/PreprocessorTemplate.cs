using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TextTemplateSourceGenerator;

public record PreprocessorTemplate(MethodDeclarationSyntax Method, string Template, string? AppendMethodName);
