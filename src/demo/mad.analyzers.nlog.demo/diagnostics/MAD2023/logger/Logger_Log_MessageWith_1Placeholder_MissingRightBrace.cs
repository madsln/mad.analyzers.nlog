// MAD2023=11:22:36
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2023.logger;

public class Logger_Log_MessageWith_1Placeholder_MissingRightBrace
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldRaiseDiagnostic_MAD2023()
    {
        int value = 42;
        _logger.Info("Doing something with value: {Value", value);
    }
}