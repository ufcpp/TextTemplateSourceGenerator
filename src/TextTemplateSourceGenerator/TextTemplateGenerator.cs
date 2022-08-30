using Microsoft.CodeAnalysis;

namespace TextTemplateSourceGenerator;

[Generator]
public partial class TextTemplateGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(AddAttribute);
        context.RegisterImplementationSourceOutput(FilterPreprocessor(context), EmitPreprocessor);
        context.RegisterImplementationSourceOutput(FilterClassMember(context), EmitClassMember);
    }
}
