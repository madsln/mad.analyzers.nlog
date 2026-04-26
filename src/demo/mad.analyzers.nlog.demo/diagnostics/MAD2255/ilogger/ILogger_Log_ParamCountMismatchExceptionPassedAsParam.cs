// MAD2255=13:62:2
// MAD2017=13:22:27;int=1;int=2
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2255.ilogger;

public class ILogger_Log_ParamCountMismatchExceptionPassedAsParam
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2255()
    {
        var ex = new InvalidOperationException("operation failed");
        int someValue = 42;
        _logger.Info("Done. Exception was: {Ex}", someValue, ex);
    }
}
