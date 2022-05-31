using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextTemplateSourceGenerator.TemplateA.Formatter;
using TextTemplateSourceGenerator.TemplateA.Parser;

namespace TextTemplateSourceGenerator.TemplateA
{
    [Generator]
    public class TextTemplatePreprocessor : ISourceGenerator
    {
        private const string attributeText = @"using System;
namespace TextTemplate
{
    [System.Diagnostics.Conditional(""COMPILE_TIME_ONLY"")]
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class TemplateAttribute : Attribute
    {
        public TemplateAttribute(string template, string appendMethodName = null) { }
    }
}
";

        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<MethodDeclarationSyntax> CandidateMethods { get; } = new List<MethodDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is not MethodDeclarationSyntax m) return;

                var any = false;
                foreach (var mod in m.Modifiers)
                {
                    if (mod.Text == "partial")
                    {
                        any = true;
                        break;
                    }
                }
                if (!any) return;

                foreach (var attLists in m.AttributeLists)
                {
                    foreach (var att in attLists.Attributes)
                    {
                        if (att.Name.GetText().ToString().Contains("Template"))
                        {
                            CandidateMethods.Add(m);
                            return;
                        }
                    }
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("TemplateAttribute.cs", SourceText.From(attributeText, Encoding.UTF8));

            if (context.SyntaxReceiver is not SyntaxReceiver receiver) return;

            var groups = receiver.CandidateMethods.Where(m => GetTemplateAttribute(m) is not null).GroupBy(m => m.Parent);

            foreach (var g in groups)
            {
                var first = g.First();
                var hintPath = HintPathHelper.GetHintPath(first);

                if (g.Key is not TypeDeclarationSyntax t) continue;

                var semanticModel = context.Compilation.GetSemanticModel(g.Key.SyntaxTree);

                var generatedSource = SyntaxNodeFormatter.Format(t, g, m => ParseTemplateAttribute(m, semanticModel));

                context.AddSource(hintPath, SourceText.From(generatedSource, Encoding.UTF8));
            }
        }

        private static AttributeArgumentListSyntax? GetTemplateAttribute(MemberDeclarationSyntax m)
        {
            foreach (var list in m.AttributeLists)
            {
                foreach (var a in list.Attributes)
                {
                    var name = a.Name.ToString();
                    if (name == "Template"
                        || name == "TemplateAttribute"
                        || name.EndsWith(".Template")
                        || name.EndsWith(".TemplateAttribute")
                        )
                    {
                        return a.ArgumentList;
                    }
                }
            }

            return null;
        }

        private static (TemplateParser elements, string appendMethodName) ParseTemplateAttribute(MemberDeclarationSyntax m, SemanticModel semanticModel)
        {
            var a = GetTemplateAttribute(m);

            if (a is null) return default;


            TemplateParser parser = default;
            string append = "builder.Append";

            if (a.Arguments.Count >= 1)
            {
                var value = (string?)semanticModel.GetConstantValue(a.Arguments[0].Expression).Value;
                if (value is not null)
                {
                    parser = new(value);
                }
            }

            if (a.Arguments.Count >= 2)
            {
                var value = (string?)semanticModel.GetConstantValue(a.Arguments[1].Expression).Value;
                if (value is not null)
                {
                    append = value;
                }
            }

            //todo: IsNull diagnostincs

            return (parser, append);
        }
    }
}
