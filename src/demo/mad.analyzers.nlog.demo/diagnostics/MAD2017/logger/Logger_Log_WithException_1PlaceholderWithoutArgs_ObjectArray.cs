// MAD2017=13:33:29;int=1;int=0
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_WithException_1PlaceholderWithoutArgs_ObjectArray
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var objectArray = new object[] {  };
        _logger.Info(exception, "Message with {Placeholder1}", objectArray);
    }
}
