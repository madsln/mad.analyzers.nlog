// MAD2017=19:63:57;int=3;int=5
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_WithException_3PlaceholdersWith5Params_ObjectArray
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var doubleArg = 3.14;
        var objectArg = new object();
        var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
        _logger.Info(exception, CultureInfo.InvariantCulture, "Message with {Placeholder1}{Placeholder2}{Placeholder3}", objectArray);
    }
}
