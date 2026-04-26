namespace TestDataGen.Test.Pipeline;

/// <summary>
/// Detects the original line-ending convention of a raw file's content.
/// Retained for reference; no longer used by the main pipeline (ADR-012).
/// </summary>
internal static class LineEndingDetector
{
    /// <summary>Returns <c>true</c> when <paramref name="rawContent"/> contains CRLF sequences.</summary>
    public static bool HasCrlf(string rawContent) => rawContent.Contains("\r\n");
}
