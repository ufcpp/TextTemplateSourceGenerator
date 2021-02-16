using System.Linq;
using TextTemplateSourceGenerator.Parser;
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
        [InlineData("${aaa}")]
        [InlineData("${aaa.ToLower()}")]
        [InlineData("${aaa.Length * 5}")]
        [InlineData("${\"aaa\" + aaa()}")]
        [InlineData(@"${aaa
+ bbb
+ ccc}")]
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
$>
    /// <summary>
    /// ${key.ToUpper()}
    /// </summary>
    $key = $value,
$<
}
$>
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
                (SyntaxElementType.String, @"
    /// <summary>
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
                (SyntaxElementType.String, @"
}"),
            };

            var result = new TemplateParser(source).ToList();

            Assert.Equal(expected, result.Select(x => (x.Type, x.Element.ToString())));
        }
    }
}
