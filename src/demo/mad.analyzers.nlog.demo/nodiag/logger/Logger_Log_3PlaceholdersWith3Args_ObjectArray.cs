using NLog;

namespace mad.analyzers.nlog.demo.nodiag.logger;

public class Logger_Log_3PlaceholdersWith3Args_ObjectArray
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldFindNoDiag()
    {
        int value1 = 42;
        double value2 = 43.0;
        string value3 = "foo";
        object[] values = new object[] { value1, value2, value3 };
        _logger.Info("Doing something with value: {Value1}{Value2}{Value3}", values);
    }
}
