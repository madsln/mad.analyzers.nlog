namespace TestDataGen.Config;

/// <summary>
/// Static configuration for the NLog logger types that are subjects under test.
/// </summary>
internal static class LoggerConfig
{
    /// <summary>
    /// Ordered list of logger types (subjects under test).
    /// </summary>
    public static readonly IReadOnlyList<string> LoggerTypes =
    [
        "Logger",
        "ILogger",
        "ILoggerExtensions",
    ];

    /// <summary>
    /// Maps each logger type to its C# field declaration string used in the generated demo class.
    /// Note: ILoggerExtensions uses ILogger as the field type (matches Python reference script).
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> FieldDeclarations =
        new Dictionary<string, string>
        {
            ["Logger"]            = "private readonly Logger _logger = LogManager.GetCurrentClassLogger();",
            ["ILogger"]           = "private readonly ILogger _logger = LogManager.GetCurrentClassLogger();",
            ["ILoggerExtensions"] = "private readonly ILogger _logger = LogManager.GetCurrentClassLogger();",
        };
}
