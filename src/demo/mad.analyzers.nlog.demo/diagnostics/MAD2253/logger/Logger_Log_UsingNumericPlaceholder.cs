// MAD2253=11:22:22
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2253.logger;

public class Logger_Log_UsingNumericPlaceholder
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldRaiseDiagnostic_MAD2253()
    {
        int value = 42;
        _logger.Info("Processing value {0}", value);
    }
}
