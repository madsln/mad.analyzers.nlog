// MAD2017=13:52:29;int=1;int=0
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_1PlaceholderWithoutArgs_ObjectArray
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var objectArray = new object[] {  };
        _logger.Info(CultureInfo.InvariantCulture, "Message with {Placeholder1}", objectArray);
    }
}
