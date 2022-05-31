using System.Linq;
using TextTemplateSourceGenerator.TemplateA.Parser;
using Xunit;

namespace TextTemplateSourceGeneratorTest
{
    public class ParserTest
    {
        [Fact]
        public void EmptyText()
        {
            var result = new TemplateParser("").ToList();
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("aaa")]
        [InlineData(@"any text
not contain dollar sign
ƒ¶„D‚ ’†
!""#%&'()=~|`{+*}<>?_
")]
        public void PlainText(string source)
        {
            var result = new TemplateParser(source).ToList();

            Assert.Single(result);

            var first = result[0];
            Assert.Equal(SyntaxElementType.String, first.Type);
            Assert.Equal(source, first.Text);
            Assert.Equal(source, first.Element.ToString());
            Assert.Equal(0, first.Range.Start);
            Assert.Equal(source.Length, first.Range.End);
        }

        [Theory]
        [InlineData("$aaa")]
        [InlineData("$‚ ‚ ‚ ")]
        [InlineData("$a123")]
        [InlineData("$a\u0301‚ \u3099")]
        public void Identifier(string source)
        {
            var result = new TemplateParser(source).ToList();

            Assert.Single(result);

            var first = result[0];
            Assert.Equal(SyntaxElementType.Identifier, first.Type);
            Assert.Equal(source, first.Text);
            Assert.Equal(source[1..], first.Element.ToString());
            Assert.Equal(0, first.Range.Start);
            Assert.Equal(source.Length, first.Range.End);
        }

        [Theory]
        [InlineData("$aaa=$bbb")]
        [InlineData("$‚ ‚ ‚ /$ƒ¶„D’†")]
        [InlineData("$a123 * $b456")]
        [InlineData("$a\u0301‚ \u3099, $b\u0302‚¢\u309A")]
        public void TwoIdentifiers(string source)
        {
            var result = new TemplateParser(source).ToList();

            Assert.Equal(3, result.Count);

            Assert.Equal(SyntaxElementType.Identifier, result[0].Type);
            Assert.Equal(SyntaxElementType.String, result[1].Type);
            Assert.Equal(SyntaxElementType.Identifier, result[2].Type);
        }

        [Theory]
        [InlineData("$")]
        [InlineData("$1")]
        [InlineData("$=")]
        [InlineData("$\u0301")]
        public void InvalidIdentifier(string source)
        {
            var result = new TemplateParser(source).ToList();

            Assert.True(result.Count >= 1);

            var first = result[0];
            Assert.Equal(SyntaxElementType.Invalid, first.Type);
        }

        [Theory]
        [InlineData("$(aaa)")]
        [InlineData("$((1))")]
        [InlineData("$(((1)))")]
        [InlineData("$(aaa.ToLower())")]
        [InlineData("$(aaa.Length * 5)")]
        [InlineData("$(\"aaa\" + aaa())")]
        [InlineData(@"$(aaa
+ bbb
+ ccc)")]
        public void Expression(string source)
        {
            var result = new TemplateParser(source).ToList();

            Assert.Single(result);

            var first = result[0];
            Assert.Equal(SyntaxElementType.Expression, first.Type);
            Assert.Equal(source, first.Text);
            Assert.Equal(source[2..^1], first.Element.ToString());
            Assert.Equal(0, first.Range.Start);
            Assert.Equal(source.Length, first.Range.End);
        }

        [Theory]
        [InlineData("$<if (true) {$>")]
        [InlineData("$<while (i >= 0 && i < Length ) {$>")]
        [InlineData("$<}$>")]
        [InlineData(@"$<
if (a is not null)
{
    foreach (var x in a)
    {
$>")]
        public void Raw(string source)
        {
            var result = new TemplateParser(source).ToList();

            Assert.Single(result);

            var first = result[0];
            Assert.Equal(SyntaxElementType.Raw, first.Type);
            Assert.Equal(source, first.Text);
            Assert.Equal(source[2..^2], first.Element.ToString());
            Assert.Equal(0, first.Range.Start);
            Assert.Equal(source.Length, first.Range.End);
        }

        [Theory]
        [InlineData(@"$if (x == ""abc"") ${")]
        [InlineData(@"$for (var i = 0; i < length; ++i) ${")]
        [InlineData(@"$foreach (var x in new[] { 1, 2, 3 }) ${")]
        [InlineData(@"$while (source.MoveNext()) ${")]
        public void ControlStart(string source)
        {
            var result = new TemplateParser(source).ToList();

            Assert.Single(result);

            var first = result[0];
            Assert.Equal(SyntaxElementType.ControlStart, first.Type);
            Assert.Equal(source, first.Text);
            Assert.Equal(source[1..^2], first.Element.ToString());
            Assert.Equal(0, first.Range.Start);
            Assert.Equal(source.Length, first.Range.End);
        }

        [Theory]
        [InlineData(@"a")]
        [InlineData(@"\
a")]
        [InlineData(@"\comment
a")]
        [InlineData(@"\ a
\ b
\ c
a")]
        public void EndOfLine1(string source)
        {
            var numBackslash = source.Count(c => c == '\\');

            var result = new TemplateParser(source).ToList();

            Assert.Equal(numBackslash + 1, result.Count);

            foreach (var x in result.SkipLast(1))
            {
                Assert.Equal(SyntaxElementType.EndOfLine, x.Type);
            }

            var last = result.Last();
            Assert.Equal(SyntaxElementType.String, last.Type);
            Assert.Equal(source.Split('\n').Last(), last.Element.ToString());
        }

        [Theory]
        [InlineData(@"a\
b")]
        [InlineData(@"abc \
def\
ghi")]
        public void EndOfLine2(string source)
        {
            var numBackslash = source.Count(c => c == '\\');
            var result = new TemplateParser(source).ToList();

            Assert.Equal(2 * numBackslash + 1, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                if ((i % 2) == 0) Assert.Equal(SyntaxElementType.String, result[i].Type);
                else Assert.Equal(SyntaxElementType.EndOfLine, result[i].Type);
            }
        }

        [Fact]
        public void Template1()
        {
            const string source = @"using System;

namespace MyCommon
{
    enum Generated
    {
$<
foreach (var (key, value) in args)
{
$>\
    /// <summary>
    /// $(key.ToUpper())
    /// </summary>
    $key = $value,
$<
}
$>\
}";
            var expected = new[]
            {
                (SyntaxElementType.String, @"using System;

namespace MyCommon
{
    enum Generated
    {
"),
                (SyntaxElementType.Raw, @"
foreach (var (key, value) in args)
{
"),
                (SyntaxElementType.EndOfLine, ""),
                (SyntaxElementType.String, @"    /// <summary>
    /// "),
                (SyntaxElementType.Expression, @"key.ToUpper()"),
                (SyntaxElementType.String, @"
    /// </summary>
    "),
                (SyntaxElementType.Identifier, @"key"),
                (SyntaxElementType.String, @" = "),
                (SyntaxElementType.Identifier, @"value"),
                (SyntaxElementType.String, @",
"),
                (SyntaxElementType.Raw, @"
}
"),
                (SyntaxElementType.EndOfLine, ""),
                (SyntaxElementType.String, @"}"),
            };

            var result = new TemplateParser(source).ToList();

            for (int i = 0; i < expected.Length; i++)
            {
                var e = expected[i];
                var a = result[i];

                Assert.Equal(e.Item1, a.Type);
                Assert.Equal(e.Item2, a.Element.ToString());
            }

            //Assert.Equal(expected, result.Select(x => (x.Type, x.Element.ToString())));
        }
    }
}
