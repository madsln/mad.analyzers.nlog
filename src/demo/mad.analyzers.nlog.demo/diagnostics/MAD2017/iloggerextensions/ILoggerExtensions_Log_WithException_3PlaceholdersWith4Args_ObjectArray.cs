// MAD2017=18:75:57;int=3;int=4
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.iloggerextensions;

public class ILoggerExtensions_Log_WithException_3PlaceholdersWith4Args_ObjectArray
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var doubleArg = 3.14;
        var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg };
        _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, "Message with {Placeholder1}{Placeholder2}{Placeholder3}", objectArray);
    }
}
