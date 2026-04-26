using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test.generated;

[TestClass]
public class diagnostics_MAD2255_logger_Tests
{
    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_ExceptionMessagePassedAsArgument_ShouldRaiseDiagnostic_MAD2255(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_ExceptionMessagePassedAsArgument
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2255()
            {
                var ex = new InvalidOperationException(""operation failed"");
                _logger.%LOGLEVEL%(""Operation failed: {ErrorMessage}"", {|#0:ex.Message|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2255").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_ExceptionPassedAsArgument_ShouldRaiseDiagnostic_MAD2255(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_ExceptionPassedAsArgument
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2255()
            {
                var ex = new InvalidOperationException(""operation failed"");
                _logger.%LOGLEVEL%(""Operation failed: {Ex}"", {|#0:ex|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2255").WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_ExceptionToStringPassedAsArgument_ShouldRaiseDiagnostic_MAD2255(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_ExceptionToStringPassedAsArgument
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2255()
            {
                var ex = new InvalidOperationException(""operation failed"");
                _logger.%LOGLEVEL%(""Unexpected issue: {Details}"", {|#0:ex.ToString()|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2255").WithLocation(0);
        var expected1 = VerifyCS.Diagnostic("MAD2256").WithLocation(0).WithArguments("ex.ToString()");
        await VerifyCS.VerifyAnalyzerAsync(test, expected0, expected1);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_ParamCountMismatchExceptionPassedAsParam_ShouldRaiseDiagnostic_MAD2255(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_ParamCountMismatchExceptionPassedAsParam
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2255()
            {
                var ex = new InvalidOperationException(""operation failed"");
                int someValue = 42;
                _logger.%LOGLEVEL%({|#1:""Done. Exception was: {Ex}""|}, someValue, {|#0:ex|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2255").WithLocation(0);
        var expected1 = VerifyCS.Diagnostic("MAD2017").WithLocation(1).WithArguments(1, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0, expected1);
    }
}