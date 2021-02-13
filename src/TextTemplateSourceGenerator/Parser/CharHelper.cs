using System.Globalization;
using static System.Globalization.UnicodeCategory;

namespace TextTemplateSourceGenerator.Parser
{
    class CharHelper
    {
        public static bool IsIdentifierStart(char c) => CharUnicodeInfo.GetUnicodeCategory(c) is <= OtherLetter or LetterNumber;
        public static bool IsIdentifierPart(char c) => CharUnicodeInfo.GetUnicodeCategory(c) is <= OtherLetter or LetterNumber or NonSpacingMark or SpacingCombiningMark or DecimalDigitNumber or ConnectorPunctuation or Format;
    }
}
