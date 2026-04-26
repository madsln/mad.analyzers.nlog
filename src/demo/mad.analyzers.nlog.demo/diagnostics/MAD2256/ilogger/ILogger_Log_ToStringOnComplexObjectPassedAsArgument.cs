// MAD2256=11:43:14;string=dto.ToString()
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2256.ilogger;

public class ILogger_Log_ToStringOnComplexObjectPassedAsArgument
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
    {
        _logger.Info("Processing: {Dto}", dto.ToString());
    }
}
