// MAD2017=13:22:29;int=0;int=2
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_NoPlaceholderWith2Args
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        _logger.Info("Message without placeholder", intArg, boolArg);
    }
}
