// MAD2017=14:22:57;int=3;int=2
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_3PlaceholdersWith2Args_ObjectArray
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        var objectArray = new object[] { intArg, boolArg };
        _logger.Info("Message with {Placeholder1}{Placeholder2}{Placeholder3}", objectArray);
    }
}
