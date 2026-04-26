namespace TestDataGen.Test.Pipeline;

/// <summary>
/// Removes comment lines and empty lines from demo source files,
/// and splits the leading metadata header from the body.
/// </summary>
internal static class SourceCleaner
{
    /// <summary>
    /// Removes all lines starting with <c>//</c> and all blank lines from <paramref name="source"/>.
    /// Line endings are normalised to <c>\n</c>.
    /// </summary>
    public static string Clean(string source)
    {
        var lines = SplitLines(source);
        var kept = lines
            .Where(l => !l.StartsWith("//", StringComparison.Ordinal))
            .Where(l => !string.IsNullOrWhiteSpace(l));
        return string.Join("\n", kept);
    }

    /// <summary>
    /// Splits <paramref name="source"/> into a leading header block (lines starting with
    /// <c>//</c>) and the remaining body. Both parts are returned with <c>\n</c> line endings.
    /// </summary>
    public static (string Header, string Body) SplitHeaderAndBody(string source)
    {
        var lines = SplitLines(source);
        int firstNonComment = 0;
        while (firstNonComment < lines.Length && lines[firstNonComment].StartsWith("//", StringComparison.Ordinal))
            firstNonComment++;

        var header = string.Join("\n", lines[..firstNonComment]);
        var body   = string.Join("\n", lines[firstNonComment..]);
        return (header, body);
    }

    // -------------------------------------------------------------------------

    private static string[] SplitLines(string source) =>
        source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
}
