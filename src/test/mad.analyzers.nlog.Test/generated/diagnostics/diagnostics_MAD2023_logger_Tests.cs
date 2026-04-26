using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test.generated;

[TestClass]
public class diagnostics_MAD2023_logger_Tests
{
    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_MessageWith_1Placeholder_MissingLeftBrace_ShouldRaiseDiagnostic_MAD2023(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_MessageWith_1Placeholder_MissingLeftBrace
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2023()
            {
                int value = 42;
                _logger.%LOGLEVEL%({|#0:""Doing something with value: Value}""|}, value);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2023").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_MessageWith_1Placeholder_MissingRightBrace_ShouldRaiseDiagnostic_MAD2023(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_MessageWith_1Placeholder_MissingRightBrace
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2023()
            {
                int value = 42;
                _logger.%LOGLEVEL%({|#0:""Doing something with value: {Value""|}, value);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2023").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }
}