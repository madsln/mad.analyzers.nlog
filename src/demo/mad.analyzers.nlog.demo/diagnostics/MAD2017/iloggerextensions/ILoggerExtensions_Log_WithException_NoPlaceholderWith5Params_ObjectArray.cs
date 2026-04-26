// MAD2017=19:75:29;int=0;int=5
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.iloggerextensions;

public class ILoggerExtensions_Log_WithException_NoPlaceholderWith5Params_ObjectArray
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
        _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, "Message without placeholder", objectArray);
    }
}
