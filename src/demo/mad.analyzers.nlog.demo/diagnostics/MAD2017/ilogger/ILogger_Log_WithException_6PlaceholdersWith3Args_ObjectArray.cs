// MAD2017=17:63:99;int=6;int=3
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_WithException_6PlaceholdersWith3Args_ObjectArray
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var objectArray = new object[] { intArg, boolArg, stringArg };
        _logger.Info(exception, CultureInfo.InvariantCulture, "Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", objectArray);
    }
}
