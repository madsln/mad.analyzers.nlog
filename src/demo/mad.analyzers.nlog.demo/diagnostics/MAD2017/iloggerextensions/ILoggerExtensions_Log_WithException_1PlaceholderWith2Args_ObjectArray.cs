// MAD2017=16:75:29;int=1;int=2
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.iloggerextensions;

public class ILoggerExtensions_Log_WithException_1PlaceholderWith2Args_ObjectArray
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var intArg = 42;
        var boolArg = true;
        var objectArray = new object[] { intArg, boolArg };
        _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, "Message with {Placeholder1}", objectArray);
    }
}
