using NLog;

namespace mad.analyzers.nlog.demo.nodiag.logger;

public class Logger_Log_WithException_1PlaceholderWith1Arg
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldFindNoDiag()
    {
        int value = 42;
        var exception = new InvalidOperationException("invalid operation");
        _logger.Info(exception, "An error occurred while doing something with value: {Value}", value);
    }
}