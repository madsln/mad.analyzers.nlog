using NLog;

namespace mad.analyzers.nlog.demo.nodiag.MAD2256;

public class Logger_Log_GuidToStringNodiag
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldNotRaiseDiagnostic(Guid id)
    {
        _logger.Info("Id: {Id}", id.ToString());
    }
}
