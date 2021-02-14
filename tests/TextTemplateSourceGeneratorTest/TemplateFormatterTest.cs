using System.Text;
using TextTemplateSourceGenerator.Formatter;
using Xunit;

namespace TextTemplateSourceGeneratorTest
{
    public class TemplateFormatterTest
    {
        private static string Format(string source, string append)
        {
            var sb = new StringBuilder();
            TemplateFormatter.Format(sb, new(source), append);
            return sb.ToString();
        }

        [Fact]
        public void Empty()
        {
            Assert.Equal("", Format("", "any name"));
        }

        [Theory]
        [InlineData("aaa")]
        [InlineData(@"any text
not contain dollar sign
ΩДあ中
!""#%&'()=~|`{+*}<>?_
")]
        public void String(string source)
        {
            var append = "a";
            Assert.Equal(append + "(@\"" + source + "\");", Format(source, append));
        }

        [Theory]
        [InlineData("$aaa")]
        [InlineData("$あああ")]
        [InlineData("$a123")]
        [InlineData("$a\u0301あ\u3099")]
        public void Identifier(string source)
        {
            var append = "a";
            Assert.Equal(append + "(" + source.Replace("$", "") + ");", Format(source, append));
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
            var append = "a";
            Assert.Equal(append + "(" + source.Replace("${", "").Replace("}", "") + ");", Format(source, append));
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
            Assert.Equal(source.Replace("$<", "").Replace("$>", ""), Format(source, "any name"));
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

            const string expected = @"append(@""using System;

namespace MyCommon
{
    enum Generated
    {
"");
foreach (var (key, value) in args)
{
append(@""
        /// <summary>
        /// "");append(key.ToUpper());append(@""
        /// </summary>
        "");append(key);append(@"" = "");append(value);append(@"",
"");
}
append(@""
}"");";

            Assert.Equal(expected, Format(source, "append"));
        }
    }
}
