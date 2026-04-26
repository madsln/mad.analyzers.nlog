// MAD2017=15:52:43;int=2;int=3
using NLog;
using System.Globalization;

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.ilogger;

public class ILogger_Log_2Placeholder2With3Args
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg = 42;
        var boolArg = true;
        var stringArg = "foo";
        _logger.Info(CultureInfo.InvariantCulture, "Message with {Placeholder1}{Placeholder2}", intArg, boolArg, stringArg);
    }
}
