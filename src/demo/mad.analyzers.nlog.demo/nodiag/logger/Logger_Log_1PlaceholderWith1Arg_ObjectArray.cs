using NLog;

namespace mad.analyzers.nlog.demo.nodiag.logger;

public class Logger_Log_1PlaceholderWith1Arg_ObjectArray
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldFindNoDiag()
    {
        int value = 1;
        object[] values = new object[] { value };
        _logger.Info("Doing something with value: {Value1}", values);
    }
}
