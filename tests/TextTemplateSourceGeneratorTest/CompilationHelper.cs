using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace TextTemplateSourceGeneratorTest
{
    internal class CompilationHelper
    {
        private static readonly CSharpParseOptions _opt = new(languageVersion: LanguageVersion.Preview, kind: SourceCodeKind.Regular, documentationMode: DocumentationMode.Parse);
        private static readonly CSharpCompilationOptions _copt = new(OutputKind.DynamicallyLinkedLibrary);

        public static Compilation Compile(string source)
        {
            var dotnetCoreDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            return CSharpCompilation.Create("test",
                syntaxTrees: new[] { SyntaxFactory.ParseSyntaxTree(source, _opt) },
                references: new[]
                {
                    AssemblyMetadata.CreateFromFile(typeof(object).Assembly.Location).GetReference(),
                    MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "netstandard.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Runtime.dll")),
                },
                options: _copt);
        }

        public static Compilation Compile(string source, params ISourceGenerator[] generators)
        {
            var compilation = Compile(source);

            // apply the source generator
            var driver = CSharpGeneratorDriver.Create(generators, parseOptions: _opt);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var resultCompilation, out _);

            return resultCompilation;
        }
    }
}
