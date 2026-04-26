using NLog;

namespace mad.analyzers.nlog.demo.nodiag.logger;

public class Logger_Log_WithException_1PlaceholderWith1Arg_ObjectArray
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldFindNoDiag()
    {
        int value = 42;
        var exception = new InvalidOperationException("invalid operation");
        object[] values = new object[] { value };
        _logger.Info(exception, "An error occurred while doing something with value: {Value}", values);
    }
}
