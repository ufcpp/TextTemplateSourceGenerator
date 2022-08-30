using Microsoft.CodeAnalysis;

namespace TextTemplateSourceGenerator;

[Generator]
public partial class TextTemplatePreprocessor : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(AddAttribute);
        context.RegisterImplementationSourceOutput(Filter(context), Emit);
    }
}
