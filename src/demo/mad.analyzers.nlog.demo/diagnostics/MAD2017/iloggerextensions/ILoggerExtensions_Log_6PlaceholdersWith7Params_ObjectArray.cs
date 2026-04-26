// MAD2017=20:34:99;int=6;int=7
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.iloggerextensions;

public class ILoggerExtensions_Log_6PlaceholdersWith7Params_ObjectArray
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
        var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg, decimalArg, longArg };
        _logger.ConditionalTrace("Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", objectArray);
    }
}
