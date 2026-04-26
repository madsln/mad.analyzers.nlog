// MAD2017=14:52:29;int=0;int=2
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_NoPlaceholderWith2Args
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        _logger.Info(CultureInfo.InvariantCulture, "Message without placeholder", intArg, boolArg);
    }
}
