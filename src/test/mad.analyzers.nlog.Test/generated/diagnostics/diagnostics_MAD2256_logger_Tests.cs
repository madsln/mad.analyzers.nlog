using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test.generated;

[TestClass]
public class diagnostics_MAD2256_logger_Tests
{
    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_JsonConvertSerializeObjectPassedAsArgument_ShouldRaiseDiagnostic_MAD2256(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using Newtonsoft.Json;
        
        namespace analyzer.test;
        
        public class Dto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        
        public class Logger_Log_JsonConvertSerializeObjectPassedAsArgument
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
            {
                _logger.%LOGLEVEL%(""Processing: {Dto}"", {|#0:JsonConvert.SerializeObject(dto)|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2256").WithLocation(0).WithArguments("JsonConvert.SerializeObject(dto)");
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_JsonSerializerSerializePassedAsArgument_ShouldRaiseDiagnostic_MAD2256(string logLevel)
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
        
        public class Logger_Log_JsonSerializerSerializePassedAsArgument
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
    public async Task Logger_Log_ToStringOnComplexObjectPassedAsArgument_ShouldRaiseDiagnostic_MAD2256(string logLevel)
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
        
        public class Logger_Log_ToStringOnComplexObjectPassedAsArgument
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
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