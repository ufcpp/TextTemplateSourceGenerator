using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void GroupByDeclaration()
        {
            var c = CompilationHelper.Compile(@"using System.Text;
using TextTemplate;

partial class A
{
    [Template("")]
    public partial void M1(StringBuilder builder);

    [Template("")]
    public partial void M2(StringBuilder builder);
}

partialc class A
{
    [Template("")]
    private partial void M3(StringBuilder builder);

    [Template("")]
    private partial void M4(StringBuilder builder);
}

partial class B
{
    [Template("")]
    internal partial void M(StringBuilder builder);
}
", new TextTemplatePreprocessor());

            //Assert.Empty(c.GetDiagnostics());
            Assert.Equal(5, c.SyntaxTrees.Count());
        }
    }
}
