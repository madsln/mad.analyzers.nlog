// MAD2255=12:70:10
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2255.iloggerextensions;

public class ILoggerExtensions_Log_ExceptionMessagePassedAsArgument
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2255()
    {
        var ex = new InvalidOperationException("operation failed");
        _logger.ConditionalDebug("Operation failed: {ErrorMessage}", ex.Message);
    }
}
