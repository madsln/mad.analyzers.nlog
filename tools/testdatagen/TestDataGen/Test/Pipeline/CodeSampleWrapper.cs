namespace TestDataGen.Test.Pipeline;

/// <summary>
/// Wraps cleaned test-code in a file-scoped <c>namespace analyzer.test;</c> header
/// used by the Roslyn analyzer test framework, and adds the required top-level
/// <c>using</c> directives.
/// </summary>
/// <remarks>
/// Any existing file-scoped namespace declaration in the demo source is stripped
/// before wrapping to avoid duplicate namespace declarations in the generated snippet.
/// </remarks>
internal static class CodeSampleWrapper
{
    private static readonly string[] FixedPreambleUsings =
    [
        "using System;",
        "using NLog;",
    ];

    /// <summary>
    /// Strips the file-scoped namespace declaration from the demo source (if present),
    /// merges <paramref name="sourceUsings"/> with the fixed preamble usings, and
    /// wraps everything with a file-scoped <c>namespace analyzer.test;</c> header.
    /// </summary>
    /// <param name="source">The body source (already without <c>using</c> directives).</param>
    /// <param name="sourceUsings">The <c>using</c> directives extracted from the demo file.</param>
    public static string Wrap(string source, IReadOnlyList<string> sourceUsings)
    {
        // Merge: fixed preamble + any additional usings from the demo file.
        var fixedSet   = new HashSet<string>(FixedPreambleUsings, StringComparer.Ordinal);
        var additional = sourceUsings
            .Where(u => !fixedSet.Contains(u))
            .OrderBy(u => u, StringComparer.Ordinal);

        var allUsings  = FixedPreambleUsings.Concat(additional);
        var usingBlock = string.Join("\n", allUsings);

        // Strip file-scoped namespace declaration to avoid duplicates.
        var lines = source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var strippedSource = string.Join("\n", lines.Where(l =>
        {
            var t = l.Trim();
            return !(t.StartsWith("namespace ", StringComparison.Ordinal) && t.EndsWith(";"));
        }));

        return usingBlock + "\n" +
               "\n" +
               "namespace analyzer.test;\n" +
               "\n" +
               strippedSource;
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// Prepends <paramref name="spaces"/> spaces to every line of <paramref name="text"/>.
    /// Blank lines also receive the indentation (mirrors PowerShell's <c>IndentTextBlock</c>).
    /// </summary>
    internal static string IndentLines(string text, int spaces)
    {
        var prefix = new string(' ', spaces);
        var lines  = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        return string.Join("\n", lines.Select(l => prefix + l));
    }
}
