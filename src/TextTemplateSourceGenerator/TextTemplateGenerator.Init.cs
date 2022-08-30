using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Text;

namespace TextTemplateSourceGenerator;

partial class TextTemplateGenerator
{
    private const string attributesText = """
        using System;
        namespace TextTemplate;

        internal enum TemplateLanguage
        {
            ExperimentalA,
        }

        /// <summary>
        /// Generates a template preprocessor method.
        /// </summary>
        [System.Diagnostics.Conditional("TEXT_TEMPLATE_COMPILE_TIME_ONLY")]
        [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
        internal sealed class TemplatePreprocessorAttribute : Attribute
        {
            public TemplateLanguage Language { get; set; }
            public TemplatePreprocessorAttribute(string template, string appendMethodName = null) { }
        }
        
        /// <summary>
        /// Generates class members from a template.
        /// </summary>
        [System.Diagnostics.Conditional("TEXT_TEMPLATE_COMPILE_TIME_ONLY")]
        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        internal sealed class TemplateClassMemberAttribute : Attribute
        {
            public TemplateLanguage Language { get; set; }
            public TemplateClassMemberAttribute(string template) { }
        }
        
        """;
    private static void AddAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("TemplateAttributes.cs", SourceText.From(attributesText, Encoding.UTF8));
    }
}
