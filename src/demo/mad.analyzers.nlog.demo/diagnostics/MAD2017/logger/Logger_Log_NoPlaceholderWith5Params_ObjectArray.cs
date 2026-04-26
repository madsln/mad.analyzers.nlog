// MAD2017=17:22:29;int=0;int=5
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_NoPlaceholderWith5Params_ObjectArray
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var doubleArg = 3.14;
        var objectArg = new object();
        var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
        _logger.Info("Message without placeholder", objectArray);
    }
}
