using NLog;

namespace mad.analyzers.nlog.demo.nodiag.logger;

public class Logger_Log_4PlaceholdersWith4Args_ObjectArray
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldFindNoDiag()
    {
        int value1 = 1;
        double value2 = 2.0;
        string value3 = "foo";
        bool value4 = false;
        object[] values = new object[] { value1, value2, value3, value4 };
        _logger.Info("Doing something with value: {Value1}{Value2}{Value3}{Value4}", values);
    }
}
