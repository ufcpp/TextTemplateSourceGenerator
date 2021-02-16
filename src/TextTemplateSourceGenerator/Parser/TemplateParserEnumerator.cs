namespace TextTemplateSourceGenerator.Parser
{
    public struct TemplateParserEnumerator
    {
        private readonly string _text;
        private int _start;
        private int _end;
        private SyntaxElementType _type;

        public TemplateParserEnumerator(string text)
        {
            _text = text;
            _start = 0;
            _end = 0;
            _type = default;
        }

        public bool MoveNext()
        {
            var text = _text;

            _start = _end;
            if (text.Length <= _start) return false;

            var i = _start;

            var first = text[i];

            _type = SyntaxElementType.Invalid;

            if (first == '$')
            {
                ++i;

                if (i < text.Length)
                {
                    var second = text[i];
                    if (second == '(') // $(expression)
                    {
                        var bracketNest = 0;
                        for (++i; i < text.Length; ++i)
                        {
                            var c = text[i];
                            if (c == '(')
                            {
                                ++bracketNest;
                            }
                            else if (c == ')')
                            {
                                --bracketNest;

                                if (bracketNest < 0)
                                {
                                    ++i;
                                    _type = SyntaxElementType.Expression;
                                    break;
                                }
                            }
                        }
                    }
                    else if (second == '<') // $< any raw codes $>
                    {
                        for (++i; i < text.Length; ++i)
                        {
                            var c = text[i];
                            if (c == '$')
                            {
                                ++i;
                                if (i < text.Length && text[i] == '>')
                                {
                                    ++i;
                                    _type = SyntaxElementType.Raw;
                                    break;
                                }
                            }
                        }
                    }
                    else // $identifier
                    {
                        if (CharHelper.IsIdentifierStart(text[i]))
                        {
                            _type = SyntaxElementType.Identifier;

                            for (++i; i < text.Length; ++i)
                            {
                                if (!CharHelper.IsIdentifierPart(text[i]))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (first == '\\') // ignore characters until \n (including \n)
            {
                for (++i; i < text.Length; ++i)
                {
                    if (text[i] == '\n')
                    {
                        ++i;
                        _type = SyntaxElementType.EndOfLine;
                        break;
                    }
                }
            }
            else // string
            {
                _type = SyntaxElementType.String;

                for (++i; i < text.Length; ++i)
                {
                    var c = text[i];
                    if (c == '$')
                    {
                        break;
                    }
                    if (c == '\\')
                    {
                        var j = i + 1;
                        if (j < text.Length)
                        {
                            var c1 = text[j];
                            if (c1 == '\r' || c1 == '\n')
                            {
                                break;
                            }
                        }
                    }
                }
            }

            _end = i;

            return true;
        }

        public SyntaxElement Current => new(_type, _text, new(_start, _end));
    }
}
