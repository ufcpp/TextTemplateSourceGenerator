namespace TextTemplateSourceGenerator.Parser
{
    public enum SyntaxElementType
    {
        /// <summary>
        /// invalid token.
        /// </summary>
        Invalid,

        /// <summary>
        /// ELEMENT
        /// ↓
        /// builder.Append($"ELEMENT");
        /// </summary>
        String,

        /// <summary>
        /// $ELEMENT
        /// ↓
        /// builder.Append(ELEMENT);
        /// </summary>
        Identifier,

        /// <summary>
        /// $(ELEMENT)
        /// ↓
        /// builder.Append(ELEMENT);
        /// </summary>
        Expression,

        /// <summary>
        /// $&lt; ELEMENT $&gt;
        /// ↓
        ///  ELEMENT
        /// </summary>
        Raw,
    }
}
