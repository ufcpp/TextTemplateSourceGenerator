using System.Collections.Generic;
using TextTemplateSourceGenerator.TemplateA.Parser;

namespace TextTemplateSourceGeneratorTest
{
    static class ParserHelper
    {
        public static List<SyntaxElement> ToList(this TemplateParser parser)
        {
            var list = new List<SyntaxElement>();

            foreach (var x in parser)
            {
                list.Add(x);
            }

            return list;
        }
    }
}
