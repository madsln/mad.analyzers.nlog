using NLog;

namespace mad.analyzers.nlog.demo.nodiag.MAD2256;

public class Logger_Log_StringArgumentNodiag
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldNotRaiseDiagnostic(string message)
    {
        _logger.Info("Message: {Message}", message);
    }
}
