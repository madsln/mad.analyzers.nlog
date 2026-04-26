// MAD2017=14:33:57;int=3;int=2
using NLog;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.logger;

public class Logger_Log_WithException_3PlaceholdersWith2Args
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var intArg = 42;
        var boolArg = true;
        _logger.Info(exception, "Message with {Placeholder1}{Placeholder2}{Placeholder3}", intArg, boolArg);
    }
}
