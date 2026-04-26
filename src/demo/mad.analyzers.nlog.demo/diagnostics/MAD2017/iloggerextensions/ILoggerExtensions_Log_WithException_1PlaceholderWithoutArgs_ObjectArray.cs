// MAD2017=14:75:29;int=1;int=0
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.iloggerextensions;

public class ILoggerExtensions_Log_WithException_1PlaceholderWithoutArgs_ObjectArray
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var exception = new InvalidOperationException("Demo exception");
        var objectArray = new object[] {  };
        _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, "Message with {Placeholder1}", objectArray);
    }
}
