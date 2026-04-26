// MAD2017=15:22:57;int=3;int=4
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_3PlaceholdersWith4Args
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var doubleArg = 3.14;
        _logger.Info("Message with {Placeholder1}{Placeholder2}{Placeholder3}", intArg, boolArg, stringArg, doubleArg);
    }
}
