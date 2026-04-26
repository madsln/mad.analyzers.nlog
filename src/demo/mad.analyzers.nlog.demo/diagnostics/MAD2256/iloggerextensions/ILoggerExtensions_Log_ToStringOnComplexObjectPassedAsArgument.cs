// MAD2256=11:55:14;string=dto.ToString()
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2256.iloggerextensions;

public class ILoggerExtensions_Log_ToStringOnComplexObjectPassedAsArgument
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
    {
        _logger.ConditionalDebug("Processing: {Dto}", dto.ToString());
    }
}
