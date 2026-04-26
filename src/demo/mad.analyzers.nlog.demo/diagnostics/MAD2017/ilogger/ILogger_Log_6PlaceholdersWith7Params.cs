// MAD2017=19:52:99;int=6;int=7
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_6PlaceholdersWith7Params
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        var doubleArg = 3.14;
        var objectArg = new object();
        var decimalArg = 4.20m;
        var longArg = 8008135L;
        _logger.Info(CultureInfo.InvariantCulture, "Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", intArg, boolArg, stringArg, doubleArg, objectArg, decimalArg, longArg);
    }
}
