using System.Text.RegularExpressions;

namespace TestDataGen.Test.Pipeline;

/// <summary>
/// Prepends preamble blocks (type declarations, extra usings) to the test code
/// when the source references known symbols from the usings table.
/// </summary>
/// <remarks>
/// The usings table is keyed by symbol name (e.g. <c>"Dto"</c>); its value is
/// the cleaned preamble content to prepend. Corresponds to
/// <c>FindUsingsInContent</c> + <c>CreatePreamble</c> in <c>CompileTestData.ps1</c>.
/// </remarks>
internal static class PreambleResolver
{
    /// <summary>
    /// Searches <paramref name="source"/> for each key in <paramref name="usingsTable"/>
    /// (whole-word match). Found preamble blocks are prepended to the source, separated
    /// by a blank line.
    /// </summary>
    public static string Resolve(string source, IReadOnlyDictionary<string, string> usingsTable)
    {
        if (usingsTable.Count == 0)
            return source;

        var preambles = new List<string>();
        foreach (var (symbol, preamble) in usingsTable)
        {
            var pattern = $@"\b{Regex.Escape(symbol)}\b";
            if (Regex.IsMatch(source, pattern))
                preambles.Add(preamble);
        }

        if (preambles.Count == 0)
            return source;

        var preambleBlock = string.Join("\n\n", preambles);
        return preambleBlock + "\n\n" + source;
    }
}
