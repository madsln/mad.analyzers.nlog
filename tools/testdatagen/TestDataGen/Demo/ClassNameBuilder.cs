namespace TestDataGen.Demo;

/// <summary>
/// Builds the class name for a demo variant following the convention:
/// <c>{LoggerType}_Log_[WithException_]{PascalCaseTestcaseName}[_{ObjectArray|ReadOnlySpan}]</c>
/// </summary>
internal static class ClassNameBuilder
{
    /// <summary>
    /// Builds the class name for a demo variant.
    /// </summary>
    /// <param name="loggerType">The logger type (e.g. "Logger", "ILogger", "ILoggerExtensions").</param>
    /// <param name="hasException">Whether the call template includes an exception parameter.</param>
    /// <param name="testCaseName">The test-case name (e.g. "no placeholder with 2 args").</param>
    /// <param name="variantSuffix">Variant suffix: "" (Direct), "ObjectArray", or "ReadOnlySpan".</param>
    public static string Build(
        string loggerType,
        bool hasException,
        string testCaseName,
        string variantSuffix)
    {
        // Python: "".join([segment.capitalize() for segment in testcase_name.split(" ")])
        // capitalize() = first char upper-case, rest lower-case
        var pascal = string.Concat(
            testCaseName.Split(' ')
                        .Select(s => s.Length > 0
                            ? char.ToUpperInvariant(s[0]) + s[1..].ToLowerInvariant()
                            : s));

        var sb = new System.Text.StringBuilder();
        sb.Append(loggerType);
        sb.Append("_Log_");
        if (hasException)
            sb.Append("WithException_");
        sb.Append(pascal);
        if (!string.IsNullOrEmpty(variantSuffix))
        {
            sb.Append('_');
            sb.Append(variantSuffix);
        }
        return sb.ToString();
    }
}
