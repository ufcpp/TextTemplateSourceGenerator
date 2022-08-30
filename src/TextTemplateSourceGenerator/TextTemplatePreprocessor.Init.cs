using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Text;

namespace TextTemplateSourceGenerator;

partial class TextTemplatePreprocessor
{
    private const string attributesText = """
        using System;
        namespace TextTemplate;

        internal enum TemplateLanguage
        {
            ExperimentalA,
        }

        [System.Diagnostics.Conditional("COMPILE_TIME_ONLY")]
        [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
        internal sealed class TemplatePreprocessorAttribute : Attribute
        {
            public TemplateLanguage Language { get; set; }
            public TemplatePreprocessorAttribute(string template, string appendMethodName = null) { }
        }

        """;
    private static void AddAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("TemplateAttributes.cs", SourceText.From(attributesText, Encoding.UTF8));
    }
}
