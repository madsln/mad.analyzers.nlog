// MAD2017=15:52:29;int=1;int=2
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_1PlaceholderWith2Args_ObjectArray
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        var objectArray = new object[] { intArg, boolArg };
        _logger.Info(CultureInfo.InvariantCulture, "Message with {Placeholder1}", objectArray);
    }
}
