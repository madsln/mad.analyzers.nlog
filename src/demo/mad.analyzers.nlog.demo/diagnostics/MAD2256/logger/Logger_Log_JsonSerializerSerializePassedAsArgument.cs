// MAD2256=12:43:29;string=JsonSerializer.Serialize(dto)
using NLog;
using System.Text.Json;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2256.logger;

public class Logger_Log_JsonSerializerSerializePassedAsArgument
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
    {
        _logger.Info("Processing: {Dto}", JsonSerializer.Serialize(dto));
    }
}
