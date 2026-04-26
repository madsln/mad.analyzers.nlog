using NLog;

namespace mad.analyzers.nlog.demo.nodiag.MAD2256;

public class Logger_Log_PrimitiveToStringNodiag
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldNotRaiseDiagnostic(int value)
    {
        _logger.Info("Value: {Value}", value.ToString());
    }
}
