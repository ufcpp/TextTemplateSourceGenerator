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
    }
}
