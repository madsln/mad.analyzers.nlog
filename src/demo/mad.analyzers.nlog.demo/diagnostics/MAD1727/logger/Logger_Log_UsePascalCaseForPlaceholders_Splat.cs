// MAD1727=11:22:27
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD1727.logger;

public class Logger_Log_UsePascalCaseForPlaceholders_Splat
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldRaiseDiagnostic_MAD1727()
    {
        int value = 42;
        _logger.Info("Processing value {@value}", value);
    }
}
