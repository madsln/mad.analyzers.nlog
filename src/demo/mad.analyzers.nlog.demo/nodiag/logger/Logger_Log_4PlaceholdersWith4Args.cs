using NLog;

namespace mad.analyzers.nlog.demo.nodiag.logger;

public class Logger_Log_4PlaceholdersWith4Args
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldFindNoDiag()
    {
        int value1 = 1;
        double value2 = 2.0;
        string value3 = "foo";
        bool value4 = false;
        _logger.Info("Doing something with value: {Value1}{Value2}{Value3}{Value4}", value1, value2, value3, value4);
    }
}