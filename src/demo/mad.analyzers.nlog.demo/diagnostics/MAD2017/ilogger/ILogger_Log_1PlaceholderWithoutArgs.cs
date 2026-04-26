// MAD2017=12:52:29;int=1;int=0
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_1PlaceholderWithoutArgs
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        _logger.Info(CultureInfo.InvariantCulture, "Message with {Placeholder1}");
    }
}
