// MAD2017=16:63:57;int=3;int=2
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_WithException_3PlaceholdersWith2Args_ObjectArray
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var intArg = 42;
        var boolArg = true;
        var objectArray = new object[] { intArg, boolArg };
        _logger.Info(exception, CultureInfo.InvariantCulture, "Message with {Placeholder1}{Placeholder2}{Placeholder3}", objectArray);
    }
}
