using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test.generated;

[TestClass]
public class nodiag_MAD2256_Tests
{
    [DataTestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_DestructuringOperatorNodiag_ShouldNotRaiseDiagnostic(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Dto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        
        public class Logger_Log_DestructuringOperatorNodiag
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldNotRaiseDiagnostic(Dto dto)
            {
                _logger.%LOGLEVEL%(""Processing: {@Dto}"", dto);  // ✅ correct — NLog destructures the object
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [DataTestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_EnumToStringNodiag_ShouldNotRaiseDiagnostic(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public enum OrderStatus { Pending, Processing, Completed }
        public class Logger_Log_EnumToStringNodiag
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldNotRaiseDiagnostic(OrderStatus status)
            {
                _logger.%LOGLEVEL%(""Status: {Status}"", status.ToString());
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [DataTestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_GuidToStringNodiag_ShouldNotRaiseDiagnostic(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_GuidToStringNodiag
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldNotRaiseDiagnostic(Guid id)
            {
                _logger.%LOGLEVEL%(""Id: {Id}"", id.ToString());
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [DataTestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_PrimitiveToStringNodiag_ShouldNotRaiseDiagnostic(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_PrimitiveToStringNodiag
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldNotRaiseDiagnostic(int value)
            {
                _logger.%LOGLEVEL%(""Value: {Value}"", value.ToString());
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [DataTestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_StringArgumentNodiag_ShouldNotRaiseDiagnostic(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_StringArgumentNodiag
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldNotRaiseDiagnostic(string message)
            {
                _logger.%LOGLEVEL%(""Message: {Message}"", message);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}