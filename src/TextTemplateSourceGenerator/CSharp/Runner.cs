using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace TextTemplateSourceGenerator.CSharp;

public class Runner
{
    private readonly MemoryStream _memory = new(64 * 1024);
    private readonly StringBuilder _environment = new(32 * 1024);
    private readonly IEnumerable<MetadataReference> _references;

    private static readonly CSharpParseOptions _parseOption = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
    private static readonly CSharpCompilationOptions _compilationOptions = new(OutputKind.DynamicallyLinkedLibrary);

    private const string TypeName = "Template";
    private const string MethodName = "Generate";

    public Runner()
    {
        var asms = AppDomain.CurrentDomain.GetAssemblies();
        _references = asms
            .Where(a => !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToArray();
    }

    public string Run(string source)
    {
        source = $$"""
        public class {{TypeName}}
        {
            public static string {{MethodName}}(System.Text.StringBuilder _env)
            {
        {{source}}
                return _env.ToString();
            }
        }
        """;

        var s = CSharpSyntaxTree.ParseText(source, _parseOption);
        var asm = Compile(s);
        return Invoke(asm);
    }

    private string Invoke(Assembly a)
    {
        var t = a.GetType(TypeName)!;
        var m = t.GetMethod(MethodName)!;

        _environment.Clear();
        m.Invoke(null, new[] { _environment });
        return _environment.ToString();
    }

    private Assembly Compile(SyntaxTree syntaxTree)
    {
        var compilation = CSharpCompilation.Create(Path.GetRandomFileName(), new[] { syntaxTree }, _references, _compilationOptions);

        var ms = _memory;
        ms.Seek(0, SeekOrigin.Begin);
        ms.SetLength(0);

        var result = compilation.Emit(ms);
        if (result.Success)
        {
            ms.Seek(0, SeekOrigin.Begin);
            return AssemblyLoadContext.Default.LoadFromStream(ms);
        }
        else
        {
            throw new InvalidOperationException(string.Join("\n", result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error).Select(d => $"{d.Id}: {d.GetMessage()}")));
        }
    }
}
