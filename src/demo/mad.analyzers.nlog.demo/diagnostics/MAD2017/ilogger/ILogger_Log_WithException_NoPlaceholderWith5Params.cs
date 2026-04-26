// MAD2017=18:63:29;int=0;int=5
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_WithException_NoPlaceholderWith5Params
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
        _logger.Info(exception, CultureInfo.InvariantCulture, "Message without placeholder", intArg, boolArg, stringArg, doubleArg, objectArg);
    }
}
