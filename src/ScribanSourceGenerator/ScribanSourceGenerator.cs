using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ScribanSourceGenerator;

[Generator]
public class ScribanSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(Init);
        FromAdditionalFiles(context);
        FromClassMemberAttribute(context);
    }

    private void FromClassMemberAttribute(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 },
            static (context, _) => ClassNameTemplate.Create(context.SemanticModel, (TypeDeclarationSyntax)context.Node)!
            )
            .Where(x => x is not null)
            .Collect();

        context.RegisterImplementationSourceOutput(provider, (context, templates) =>
        {
            if (!templates.Any()) return;

            var buffer = new StringBuilder();

            var ordinal = 0;
            foreach (var t in templates)
            {
                context.AddSource(
                    filename(t.Type, ordinal++, buffer),
                    Source(render(t, buffer))
                    );
            }
        });

        static string render(ClassNameTemplate t, StringBuilder buffer)
        {
            buffer.Clear();
            buffer.Append("""
                // <auto-generated/>

                """);
            var nest = SyntaxNodeHelper.AppendDeclarations(buffer, t.Type);

            foreach (var template in t.Templates)
            {
                var result = Scriban.Template.Parse(template).Render(); //todo: error handling
                buffer.Append(result);
            }
            SyntaxNodeHelper.AppendClose(buffer, nest);
            return buffer.ToString();
        }

        static string filename(TypeDeclarationSyntax type, int ordinal, StringBuilder buffer)
        {
            buffer.Clear();
            buffer.Append($"{ordinal}_{type.Identifier.Text}_membertemplate.cs");
            return buffer.ToString();
        }
    }

    private void FromAdditionalFiles(IncrementalGeneratorInitializationContext context)
    {
        var additionalFiles = context.AdditionalTextsProvider
            .Where(f => f.Path.EndsWith(".scriban"))
            .Select((f, c) => (filename: Path.GetFileName(f.Path), template: f.GetText()!.ToString()));

        context.RegisterImplementationSourceOutput(additionalFiles, static (context, arg) =>
        {
            //todo: error handling
            var hintPath = arg.filename + ".cs";
            var result = Scriban.Template.Parse(arg.template).Render();
            context.AddSource(hintPath, Source(result));
        });
    }

    private void Init(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("Attributes.cs", Source("""
                // <auto-generated/>
                namespace ScribanGeneretor;

                [System.Diagnostics.Conditional("TEXT_TEMPLATE_COMPILE_TIME_ONLY")]
                [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
                internal sealed class ClassMemberAttribute : Attribute
                {
                    public ClassMemberAttribute(string template) { }
                }
                
                """));
    }

    private static SourceText Source(string text) => SourceText.From(text, System.Text.Encoding.UTF8);
}
