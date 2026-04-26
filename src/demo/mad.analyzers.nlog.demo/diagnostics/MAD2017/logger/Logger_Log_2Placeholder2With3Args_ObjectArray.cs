// MAD2017=15:22:43;int=2;int=3
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_2Placeholder2With3Args_ObjectArray
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var objectArray = new object[] { intArg, boolArg, stringArg };
        _logger.Info("Message with {Placeholder1}{Placeholder2}", objectArray);
    }
}
