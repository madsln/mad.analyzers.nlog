// MAD2017=17:52:57;int=3;int=4
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_3PlaceholdersWith4Args_ObjectArray
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var doubleArg = 3.14;
        var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg };
        _logger.Info(CultureInfo.InvariantCulture, "Message with {Placeholder1}{Placeholder2}{Placeholder3}", objectArray);
    }
}
