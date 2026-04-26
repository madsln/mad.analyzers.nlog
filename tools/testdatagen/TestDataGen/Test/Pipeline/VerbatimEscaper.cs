namespace TestDataGen.Test.Pipeline;

/// <summary>
/// Escapes a code string for embedding inside a C# verbatim string literal
/// (<c>@"…"</c>) by doubling every double-quote character.
/// </summary>
internal static class VerbatimEscaper
{
    /// <summary>
    /// Replaces every <c>"</c> in <paramref name="code"/> with <c>""</c>.
    /// </summary>
    public static string Escape(string code) =>
        code.Replace("\"", "\"\"");
}
