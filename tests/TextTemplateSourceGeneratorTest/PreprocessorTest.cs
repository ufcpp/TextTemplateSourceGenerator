using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using TextTemplateSourceGenerator;
using Xunit;

namespace TextTemplateSourceGeneratorTest
{
    public class PreprocessorTest
    {
        [Fact]
        public void TemplateAttributeAdded()
        {
            var c = CompilationHelper.Compile("", new TextTemplatePreprocessor());
            Assert.Empty(c.GetDiagnostics());
            Assert.Contains(c.SyntaxTrees, s => s.FilePath.Contains("TemplateAttribute.cs"));
        }

        [Fact]
        public void NoTemplates()
        {
            var c = CompilationHelper.Compile("class A { }", new TextTemplatePreprocessor());
            Assert.Empty(c.GetDiagnostics());
            Assert.Equal(2, c.SyntaxTrees.Count());
        }

        [Fact]
        public void NoTemplateArguments()
        {
            var c = CompilationHelper.Compile("class A { [TextTemplate.Template] private partial void M(); }", new TextTemplatePreprocessor());
            Assert.Equal(2, c.SyntaxTrees.Count());
        }

        [Fact]
        public void NoPartial()
        {
            var c = CompilationHelper.Compile("class A { [TextTemplate.Template(\"\")] private void M() { } }", new TextTemplatePreprocessor());
            Assert.Empty(c.GetDiagnostics());
            Assert.Equal(2, c.SyntaxTrees.Count());

            c = CompilationHelper.Compile("abstract class A { [TextTemplate.Template(\"\")] protected abstract void M(); }", new TextTemplatePreprocessor());
            Assert.Empty(c.GetDiagnostics());
            Assert.Equal(2, c.SyntaxTrees.Count());
        }

        [Fact]
        public void GroupByDeclaration()
        {
            var c = CompilationHelper.Compile(@"using System.Text;
using TextTemplate;

partial class A
{
    [Template("""")]
    public partial void M1(StringBuilder builder);

    [Template("""")]
    public partial void M2(StringBuilder builder);
}

partial class A
{
    [Template("""")]
    private partial void M3(StringBuilder builder);

    [Template("""")]
    private partial void M4(StringBuilder builder);
}

partial class B
{
    [Template("""")]
    internal partial void M(StringBuilder builder);
}
", new TextTemplatePreprocessor());

            Assert.Empty(c.GetDiagnostics());
            Assert.Equal(5, c.SyntaxTrees.Count());
        }

        [Fact]
        public void EmptyTemplate()
        {
            var c = CompilationHelper.Compile(@"using System.Text;
using TextTemplate;

partial class A
{
    [Template("""")]
    public partial void M(StringBuilder builder);
}", new TextTemplatePreprocessor());

            Assert.Empty(c.GetDiagnostics());

            var tree = c.SyntaxTrees.First(t => t.FilePath.EndsWith("A.M.cs"));

            Assert.True(tree.IsEquivalentTo(SyntaxFactory.ParseSyntaxTree(@"#pragma warning 8019
using System.Text;
using TextTemplate;
partial class A
{
public partial void M(StringBuilder builder)
{

}
}
")));
        }

        [Fact]
        public void Template1()
        {
            var c = CompilationHelper.Compile(@"
partial class A
{
    [TextTemplate.Template(""($x, $y)"")]
    public partial void M(System.Text.StringBuilder builder, object x, object y);
}", new TextTemplatePreprocessor());

            Assert.Empty(c.GetDiagnostics());

            var tree = c.SyntaxTrees.First(t => t.FilePath.EndsWith("A.M.cs"));

            Assert.True(tree.IsEquivalentTo(SyntaxFactory.ParseSyntaxTree(@"#pragma warning 8019
partial class A
{
public partial void M(System.Text.StringBuilder builder, object x, object y)
{
builder.Append(@""("");builder.Append(x);builder.Append(@"", "");builder.Append(y);builder.Append(@"")"");
}
}
")));
        }

        [Fact]
        public void Template2()
        {
            var c = CompilationHelper.Compile(@"
partial class A
{
    [TextTemplate.Template(@""$<
void a(object x) => builder.Append(x);
$>($x, $y)"", ""a"")]
    public partial void M(System.Text.StringBuilder builder, object x, object y);
}", new TextTemplatePreprocessor());

            Assert.Empty(c.GetDiagnostics());

            var tree = c.SyntaxTrees.First(t => t.FilePath.EndsWith("A.M.cs"));

            Assert.True(tree.IsEquivalentTo(SyntaxFactory.ParseSyntaxTree(@"#pragma warning 8019
partial class A
{
public partial void M(System.Text.StringBuilder builder, object x, object y)
{

void a(object x) => builder.Append(x);
a(@""("");a(x);a(@"", "");a(y);a(@"")"");
}
}
")));
        }
    }
}
