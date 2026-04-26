// MAD2017=16:33:43;int=2;int=3
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_WithException_2Placeholder2With3Args_ObjectArray
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var objectArray = new object[] { intArg, boolArg, stringArg };
        _logger.Info(exception, "Message with {Placeholder1}{Placeholder2}", objectArray);
    }
}
