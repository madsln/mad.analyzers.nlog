// MAD2017=11:22:29;int=1;int=0
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_1PlaceholderWithoutArgs
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        _logger.Info("Message with {Placeholder1}");
    }
}
