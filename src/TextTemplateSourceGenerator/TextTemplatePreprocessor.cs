﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextTemplateSourceGenerator
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

            foreach (var m in receiver.CandidateMethods)
            {
            }
        }
    }
}
