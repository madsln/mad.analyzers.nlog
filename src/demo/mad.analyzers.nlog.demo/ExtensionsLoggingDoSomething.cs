using Microsoft.Extensions.Logging;

namespace mad.analyzers.nlog.demo
{
    public class ExtensionsLoggingDoSomething
    {
        private readonly ILogger<ExtensionsLoggingDoSomething> _logger;

        public ExtensionsLoggingDoSomething(ILogger<ExtensionsLoggingDoSomething> logger)
        {
            _logger = logger;
        }

        public void DoSomething(int value)
        {
            _logger.LogInformation("Doing something with value: {Value}", value);

            _logger.LogInformation("Doing something without parameters");

            var foo = "bar";
            _logger.LogInformation($"Doing something with extra data: {foo}");

            _logger.LogInformation("Doing something with multiple values: {Value1}, {Value2}", value);

            var ex = new Exception("Sample exception");
            _logger.LogError(ex, "An error occurred while doing something with value: {Value}", value);

            _logger.LogError("An error occurred without exception {Exception}", ex);
        }
    }
}
