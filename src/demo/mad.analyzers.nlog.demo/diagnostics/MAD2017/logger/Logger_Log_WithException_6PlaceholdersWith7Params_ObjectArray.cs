// MAD2017=20:33:99;int=6;int=7
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_WithException_6PlaceholdersWith7Params_ObjectArray
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var doubleArg = 3.14;
        var objectArg = new object();
        var decimalArg = 4.20m;
        var longArg = 8008135L;
        var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg, decimalArg, longArg };
        _logger.Info(exception, "Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", objectArray);
    }
}
