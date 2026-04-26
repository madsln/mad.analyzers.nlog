// MAD2254=10:22:38
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2254.logger;

public class Logger_Log_InterpolatedString
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldRaiseDiagnostic_MAD2254(int value)
    {
        _logger.Info($"Doing something with value: {value}");
    }
}