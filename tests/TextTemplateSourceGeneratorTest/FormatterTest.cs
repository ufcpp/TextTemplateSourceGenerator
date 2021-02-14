using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading.Tasks;
using TextTemplateSourceGenerator.Formatter;
using Xunit;

namespace TextTemplateSourceGeneratorTest
{
    public class FormatterTest
    {
        [Fact]
        public async Task NestedTypes()
        {
            var c = CompilationHelper.Compile(@"using System;
using System.Linq;

namespace X.Y
{
    namespace Z
    {
        interface A
        {
            public class B
            {
                public record C<T>
                {
                    public struct D<U, V>
                    {
                        [Template]
                        public partial void M1(int n, DateTime d, List<(int x, int y)> list);

                        [Template]
                        internal partial void M2(System.Text.StringBuilder builder);
                    }
                }
            }
        }
    }
}");

            var root = await c.SyntaxTrees.First().GetRootAsync();

            var m = root.DescendantNodes(n => n is not MethodDeclarationSyntax).OfType<MethodDeclarationSyntax>();
            var t = (TypeDeclarationSyntax)m.First().Parent;

            var result = TemplateFormatter.Format(t, m, _ => new(""));

            Assert.Equal(@"#pragma warning disable 8019
using System;
using System.Linq;
namespace X.Y {
namespace Z {
partial interface A {
partial class B {
partial record C<T> {
partial struct D<U, V> {
public partial void M1(int n, DateTime d, List<(int x, int y)> list)
{

}
internal partial void M2(System.Text.StringBuilder builder)
{

}
}}}}}}
", result);
        }
    }
}
