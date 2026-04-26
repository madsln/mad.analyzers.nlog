using NLog;

namespace mad.analyzers.nlog.demo.nodiag.MAD2256;

public enum OrderStatus { Pending, Processing, Completed }

public class Logger_Log_EnumToStringNodiag
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldNotRaiseDiagnostic(OrderStatus status)
    {
        _logger.Info("Status: {Status}", status.ToString());
    }
}
