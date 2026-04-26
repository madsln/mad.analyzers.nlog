// MAD2256=12:43:32;string=JsonConvert.SerializeObject(dto)
using NLog;
using Newtonsoft.Json;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2256.logger;

public class Logger_Log_JsonConvertSerializeObjectPassedAsArgument
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
    {
        _logger.Info("Processing: {Dto}", JsonConvert.SerializeObject(dto));
    }
}
