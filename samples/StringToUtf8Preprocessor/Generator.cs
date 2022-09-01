using System;
using System.Collections.Generic;
using System.Text;

namespace StringToUtf8Preprocessor
{
    partial class Generator
    {
        [TextTemplate.TemplatePreprocessor(@"$<
var buffer = new StringBuilder();

if (!string.IsNullOrEmpty(@namespace))
{
$>\
namespace $(@namespace)
{
$<
}
$>partial class $typename
{
$<
    foreach (var (methodName, value, accessibility) in methods)
    {
$>\
    $(AccessibilityText(accessibility)) static partial System.ReadOnlySpan<byte> $methodName() => new byte[] { \
$<
        foreach (var b in GetBytes(value))
        {
$>\
$b, $<
        }
$>};
$<
    }
$>\
}
$<
if (!string.IsNullOrEmpty(@namespace))
{
$>\
}
$<
}
return buffer.ToString();
$>
", "buffer.Append")]
        public static partial string Generate(string? @namespace, string typename, IEnumerable<(string methodName, string value, Accessibility accessibility)> methods);

        private static byte[] GetBytes(string value) => Encoding.UTF8.GetBytes(value);

        private static string AccessibilityText(Accessibility accessibility) => accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.Private => "private",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => throw new InvalidOperationException(),
        };
    }
}
