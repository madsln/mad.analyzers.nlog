using System.Text.RegularExpressions;

namespace TestDataGen.Test.Pipeline;

/// <summary>
/// Extracts <c>using</c> directives from a source file as an early pipeline step,
/// so that all subsequent pipeline steps receive a body without any <c>using</c> lines.
/// </summary>
/// <remarks>
/// Handles simple directives (<c>using NLog;</c>), alias directives
/// (<c>using X = Foo.Bar;</c>), and <c>global using</c> directives.
/// </remarks>
internal static class UsingExtractor
{
    private static readonly Regex UsingLineRegex = new(
        @"^\s*(global\s+)?using\s+",
        RegexOptions.Compiled);

    /// <summary>
    /// Splits all <c>using</c> directives from <paramref name="source"/> and returns
    /// them separately from the remaining body.
    /// </summary>
    /// <returns>
    /// <c>Usings</c>: the extracted directive lines (trimmed, original order);
    /// <c>Body</c>: the remaining source without any <c>using</c> lines.
    /// </returns>
    public static (IReadOnlyList<string> Usings, string Body) Extract(string source)
    {
        var lines  = source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var usings = new List<string>();
        var rest   = new List<string>();

        foreach (var line in lines)
        {
            if (UsingLineRegex.IsMatch(line))
                usings.Add(line.Trim());
            else
                rest.Add(line);
        }

        // Deduplicate while preserving order.
        var seen            = new HashSet<string>(StringComparer.Ordinal);
        var deduplicatedUsings = new List<string>();
        foreach (var u in usings)
        {
            if (seen.Add(u))
                deduplicatedUsings.Add(u);
        }

        return (deduplicatedUsings, string.Join("\n", rest));
    }
}
