using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test.generated;

[TestClass]
public class diagnostics_MAD2256_ilogger_Tests
{
    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILogger_Log_JsonSerializerSerializePassedAsArgument_ShouldRaiseDiagnostic_MAD2256(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Text.Json;
        
        namespace analyzer.test;
        
        public class Dto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        
        public class ILogger_Log_JsonSerializerSerializePassedAsArgument
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
            {
                _logger.%LOGLEVEL%(""Processing: {Dto}"", {|#0:JsonSerializer.Serialize(dto)|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2256").WithLocation(0).WithArguments("JsonSerializer.Serialize(dto)");
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILogger_Log_ToStringOnComplexObjectPassedAsArgument_ShouldRaiseDiagnostic_MAD2256(string logLevel)
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
        
        public class ILogger_Log_ToStringOnComplexObjectPassedAsArgument
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
            {
                _logger.%LOGLEVEL%(""Processing: {Dto}"", {|#0:dto.ToString()|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2256").WithLocation(0).WithArguments("dto.ToString()");
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }
}