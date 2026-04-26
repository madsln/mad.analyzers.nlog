// MAD2256=12:43:29;string=JsonSerializer.Serialize(dto)
using NLog;
using System.Text.Json;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2256.ilogger;

public class ILogger_Log_JsonSerializerSerializePassedAsArgument
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
    {
        _logger.Info("Processing: {Dto}", JsonSerializer.Serialize(dto));
    }
}
