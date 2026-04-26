using NLog;

namespace mad.analyzers.nlog.demo.nodiag.logger;

public class Logger_Log_NoParameters
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldFindNoDiag()
    {
        _logger.Info("Doing something without parameters");
    }
}
