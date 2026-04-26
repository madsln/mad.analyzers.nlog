using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test.generated;

[TestClass]
public class diagnostics_MAD1727_logger_Tests
{
    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_UsePascalCaseForPlaceholders_Splat_ShouldRaiseDiagnostic_MAD1727(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_UsePascalCaseForPlaceholders_Splat
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD1727()
            {
                int value = 42;
                _logger.%LOGLEVEL%({|#0:""Processing value {@value}""|}, value);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD1727").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_UsePascalCaseForPlaceholders_ShouldRaiseDiagnostic_MAD1727(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_UsePascalCaseForPlaceholders
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD1727()
            {
                int value = 42;
                _logger.%LOGLEVEL%({|#0:""Processing value {value}""|}, value);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD1727").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }
}