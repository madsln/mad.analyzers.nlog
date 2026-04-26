using NLog;

namespace mad.analyzers.nlog.demo.nodiag.logger;

public class Logger_Log_2PlaceholdersWith2Args_ObjectArray
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldFindNoDiag()
    {
        int value1 = 1;
        double value2 = 2.0;
        object[] values = new object[] { value1, value2 };
        _logger.Info("Doing something with value: {Value1}{Value2}", values);
    }
}
