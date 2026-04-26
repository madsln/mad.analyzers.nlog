// MAD2255=12:53:13
// MAD2256=12:53:13;string=ex.ToString()
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2255.ilogger;

public class ILogger_Log_ExceptionToStringPassedAsArgument
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2255()
    {
        var ex = new InvalidOperationException("operation failed");
        _logger.Warn("Unexpected issue: {Details}", ex.ToString());
    }
}
