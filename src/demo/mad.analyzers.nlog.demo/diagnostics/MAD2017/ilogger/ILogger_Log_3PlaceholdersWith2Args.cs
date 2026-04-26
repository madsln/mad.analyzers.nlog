// MAD2017=14:52:57;int=3;int=2
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_3PlaceholdersWith2Args
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        _logger.Info(CultureInfo.InvariantCulture, "Message with {Placeholder1}{Placeholder2}{Placeholder3}", intArg, boolArg);
    }
}
