// MAD2255=12:49:2
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2255.ilogger;

public class ILogger_Log_ExceptionPassedAsArgument
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2255()
    {
        var ex = new InvalidOperationException("operation failed");
        _logger.Error("Operation failed: {Ex}", ex);
    }
}
