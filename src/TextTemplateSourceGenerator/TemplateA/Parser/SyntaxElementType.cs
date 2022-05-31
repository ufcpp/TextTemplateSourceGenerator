namespace TextTemplateSourceGenerator.TemplateA.Parser
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
        /// $if ELEMENT ${
        /// $for ELEMENT ${
        /// $foreach ELEMENT ${
        /// $while ELEMENT ${
        /// ↓
        /// [keyword] ELEMENT {
        /// </summary>
        /// <remarks>
        /// shorthand for
        /// $lt;[keyword] ELEMENT {$gt;
        /// </remarks>
        ControlStart,

        /// <summary>
        /// $&lt; ELEMENT $&gt;
        /// ↓
        ///  ELEMENT
        /// </summary>
        Raw,

        /// <summary>
        /// \ [eol]
        /// ↓
        /// [removed]
        /// </summary>
        /// <remarks>
        /// used for formatting purpose.
        /// </remarks>
        EndOfLine,
    }
}
