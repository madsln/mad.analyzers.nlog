namespace TestDataGen.Config;

/// <summary>
/// A single call-template entry describing one logging-call pattern.
/// </summary>
/// <param name="LoggerType">The logger type this template belongs to.</param>
/// <param name="Pattern">
///   The raw call pattern string containing the <c>{MESSAGE_AND_ARGS}</c> placeholder,
///   e.g. <c>_logger.Info({MESSAGE_AND_ARGS});</c>.
/// </param>
/// <param name="HasException">Whether the pattern includes an <c>exception</c> parameter.</param>
/// <param name="HasCultureInfo">Whether the pattern includes a <c>CultureInfo</c> parameter.</param>
internal record CallTemplate(
    string LoggerType,
    string Pattern,
    bool HasException,
    bool HasCultureInfo);

/// <summary>
/// Static configuration for all logging call-templates per logger type.
/// Patterns use <c>%LOGLEVEL%</c> as the log-level placeholder (replaced at generation time)
/// and <c>{MESSAGE_AND_ARGS}</c> as the message+arguments placeholder.
/// </summary>
internal static class CallTemplateConfig
{
    public static readonly IReadOnlyList<CallTemplate> Templates =
    [
        // Logger
        new("Logger", "_logger.%LOGLEVEL%({MESSAGE_AND_ARGS});",                  HasException: false, HasCultureInfo: false),
        new("Logger", "_logger.%LOGLEVEL%(exception, {MESSAGE_AND_ARGS});",        HasException: true,  HasCultureInfo: false),

        // ILogger
        new("ILogger", "_logger.%LOGLEVEL%({MESSAGE_AND_ARGS});",                 HasException: false, HasCultureInfo: false),
        new("ILogger", "_logger.%LOGLEVEL%(exception, {MESSAGE_AND_ARGS});",       HasException: true,  HasCultureInfo: false),
        new("ILogger", "_logger.%LOGLEVEL%(CultureInfo.InvariantCulture, {MESSAGE_AND_ARGS});",            HasException: false, HasCultureInfo: true),
        new("ILogger", "_logger.%LOGLEVEL%(exception, CultureInfo.InvariantCulture, {MESSAGE_AND_ARGS});", HasException: true,  HasCultureInfo: true),

        // ILoggerExtensions
        new("ILoggerExtensions", "_logger.ConditionalDebug({MESSAGE_AND_ARGS});",            HasException: false, HasCultureInfo: true),
        new("ILoggerExtensions", "_logger.ConditionalDebug(exception, CultureInfo.InvariantCulture, {MESSAGE_AND_ARGS});", HasException: true,  HasCultureInfo: true),
        new("ILoggerExtensions", "_logger.ConditionalTrace({MESSAGE_AND_ARGS});",            HasException: false, HasCultureInfo: true),
        new("ILoggerExtensions", "_logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {MESSAGE_AND_ARGS});", HasException: true,  HasCultureInfo: true),
    ];
}
