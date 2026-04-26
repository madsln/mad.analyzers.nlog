namespace TestDataGen.Demo;

/// <summary>
/// Calculates the <c>// MAD2017=Line:Column:Length;int=PlaceholderCount;int=ArgumentCount</c>
/// metadata header line for a demo variant (ADR-012).
/// </summary>
/// <remarks>
/// <para>
/// <c>Line</c> and <c>Column</c> are 1-based and refer to the opening <c>"</c> of the
/// message-template string within the file body, which uses exclusively <c>\n</c> line
/// endings (SourceCleaner invariant).
/// </para>
/// </remarks>
internal static class MetadataHeaderBuilder
{
    /// <summary>
    /// Builds the metadata header comment for the given variant.
    /// </summary>
    /// <param name="variant">The resolved demo variant (provides PlaceholderCount, ArgumentCount).</param>
    /// <param name="fileBodyCode">
    ///   The complete file body (usings + namespace + class), built with <c>\n</c> line endings,
    ///   <strong>without</strong> the header line prepended yet.
    /// </param>
    /// <returns>
    ///   A single-line comment string, e.g.
    ///   <c>// MAD2017=12:28:27;int=1;int=2</c>.
    /// </returns>
    public static string Build(DemoVariant variant, string fileBodyCode)
    {
        int messageStart = fileBodyCode.IndexOf("\"Message", StringComparison.Ordinal);
        // End of message string: next '"' after the opening '"'
        int messageEnd   = fileBodyCode.IndexOf('"', messageStart + 1) + 1;
        int length       = messageEnd - messageStart;

        var (line, column) = PositionToLineColumn(fileBodyCode, messageStart);

        return $"// MAD2017={line}:{column}:{length};int={variant.PlaceholderCount};int={variant.ArgumentCount}";
    }

    /// <summary>
    /// Converts an absolute character position in a <c>\n</c>-normalised string to
    /// 1-based line and column numbers (ADR-012).
    /// </summary>
    internal static (int line, int column) PositionToLineColumn(string normalizedBody, int absolutePos)
    {
        int line   = 1;
        int lastNl = -1;
        for (int i = 0; i < absolutePos; i++)
        {
            if (normalizedBody[i] == '\n')
            {
                line++;
                lastNl = i;
            }
        }
        int column = absolutePos - lastNl; // 1-based: lastNl == -1 → column = absolutePos + 1
        return (line, column);
    }
}
