using System.Text.RegularExpressions;

namespace TestDataGen.Test.Pipeline;

/// <summary>
/// Replaces the first log-level identifier in a source string with the
/// data-driven placeholder <c>%LOGLEVEL%</c>, enabling a single parameterised
/// test method to cover all six NLog log levels.
/// </summary>
internal static class LogLevelParameterizer
{
    /// <summary>All NLog log-level identifiers (case-sensitive).</summary>
    public static readonly string[] LogLevels = ["Trace", "Debug", "Info", "Warn", "Error", "Fatal"];

    private static readonly Regex LogLevelRegex = new(
        @"\b(Trace|Debug|Info|Warn|Error|Fatal)\b",
        RegexOptions.Compiled);

    /// <summary>
    /// Replaces the first occurrence of any log-level identifier in
    /// <paramref name="source"/> with <c>%LOGLEVEL%</c>.
    /// </summary>
    public static string Parameterize(string source)
    {
        bool replaced = false;
        return LogLevelRegex.Replace(source, m =>
        {
            if (replaced) return m.Value;
            replaced = true;
            return "%LOGLEVEL%";
        });
    }
}
