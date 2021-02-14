using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading.Tasks;
using TextTemplateSourceGenerator.Formatter;
using Xunit;

namespace TextTemplateSourceGeneratorTest
{
    public class SyntaxNodeFormatterTest
    {
        [Fact]
        public async Task SimpleTemplate()
        {
            var c = CompilationHelper.Compile(@"using System.Text;

class A
{
    public static partial void M(StringBuilder builder, int x, int y);
}");

            var root = await c.SyntaxTrees.First().GetRootAsync();

            var m = root.DescendantNodes(n => n is not MethodDeclarationSyntax).OfType<MethodDeclarationSyntax>();
            var t = (TypeDeclarationSyntax)m.First().Parent;

            var result = SyntaxNodeFormatter.Format(t, m, _ => (new(@"new[] {$<
for (var i = x; i < y; ++i)
{
$>
    $i,
$<
}
$>};"), "builder.Append"));

            Assert.Equal(@"#pragma warning disable 8019
using System.Text;
partial class A {
public static partial void M(StringBuilder builder, int x, int y)
{
builder.Append(@""new[] {"");
for (var i = x; i < y; ++i)
{
builder.Append(@""
    "");builder.Append(i);builder.Append(@"",
"");
}
builder.Append(@""};"");
}
}
", result);
        }

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

            var result = SyntaxNodeFormatter.Format(t, m, _ => (new(""), "any name"));

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
