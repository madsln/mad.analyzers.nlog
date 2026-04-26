// MAD2017=16:22:29;int=1;int=5
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_1PlaceholderWith5Params
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var doubleArg = 3.14;
        var objectArg = new object();
        _logger.Info("Message with {Placeholder1}", intArg, boolArg, stringArg, doubleArg, objectArg);
    }
}
