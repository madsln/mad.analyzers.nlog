// MAD2256=12:55:29;string=JsonSerializer.Serialize(dto)
using NLog;
using System.Text.Json;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2256.iloggerextensions;

public class ILoggerExtensions_Log_JsonSerializerSerializePassedAsArgument
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
    {
        _logger.ConditionalDebug("Processing: {Dto}", JsonSerializer.Serialize(dto));
    }
}
