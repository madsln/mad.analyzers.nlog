using NLog;

namespace mad.analyzers.nlog.demo.nodiag.MAD2256;

public class Logger_Log_DestructuringOperatorNodiag
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldNotRaiseDiagnostic(Dto dto)
    {
        _logger.Info("Processing: {@Dto}", dto);  // ✅ correct — NLog destructures the object
    }
}
