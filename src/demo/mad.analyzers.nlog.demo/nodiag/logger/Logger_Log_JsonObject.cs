using NLog;

namespace mad.analyzers.nlog.demo.nodiag.logger;

public class Logger_Log_JsonObject
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void ShouldFindNoDiag()
    {
        var jsonSample = new Dto { Id = 1, Name = "Test" };
        _logger.Info("Here comes some json {@Json}", jsonSample);
    }
}