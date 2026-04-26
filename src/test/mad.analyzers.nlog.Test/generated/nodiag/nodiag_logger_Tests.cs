using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test.generated;

[TestClass]
public class nodiag_logger_Tests
{
    [DataTestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task Logger_Log_1PlaceholderWith1Arg_ObjectArray_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_1PlaceholderWith1Arg_ObjectArray
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value = 1;
                object[] values = new object[] { value };
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}"", values);
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
    public async Task Logger_Log_1PlaceholderWith1Arg_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_1PlaceholderWith1Arg
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value = 1;
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}"", value);
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
    public async Task Logger_Log_2PlaceholdersWith2Args_ObjectArray_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_2PlaceholdersWith2Args_ObjectArray
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 1;
                double value2 = 2.0;
                object[] values = new object[] { value1, value2 };
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}"", values);
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
    public async Task Logger_Log_2PlaceholdersWith2Args_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_2PlaceholdersWith2Args
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 1;
                double value2 = 2.0;
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}"", value1, value2);
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
    public async Task Logger_Log_3PlaceholdersWith3Args_ObjectArray_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_3PlaceholdersWith3Args_ObjectArray
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 42;
                double value2 = 43.0;
                string value3 = ""foo"";
                object[] values = new object[] { value1, value2, value3 };
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}{Value3}"", values);
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
    public async Task Logger_Log_3PlaceholdersWith3Args_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_3PlaceholdersWith3Args
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 42;
                double value2 = 43.0;
                string value3 = ""foo"";
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}{Value3}"", value1, value2, value3);
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
    public async Task Logger_Log_4PlaceholdersWith4Args_ObjectArray_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_4PlaceholdersWith4Args_ObjectArray
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 1;
                double value2 = 2.0;
                string value3 = ""foo"";
                bool value4 = false;
                object[] values = new object[] { value1, value2, value3, value4 };
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}{Value3}{Value4}"", values);
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
    public async Task Logger_Log_4PlaceholdersWith4Args_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_4PlaceholdersWith4Args
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 1;
                double value2 = 2.0;
                string value3 = ""foo"";
                bool value4 = false;
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}{Value3}{Value4}"", value1, value2, value3, value4);
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
    public async Task Logger_Log_5PlaceholdersWith5Args_ObjectArray_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_5PlaceholdersWith5Args_ObjectArray
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 1;
                double value2 = 2.0;
                string value3 = ""foo"";
                bool value4 = false;
                int value5 = 5;
                object[] values = new object[] { value1, value2, value3, value4, value5 };
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}{Value3}{Value4}{Value5}"", values);
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
    public async Task Logger_Log_5PlaceholdersWith5Args_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_5PlaceholdersWith5Args
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 1;
                double value2 = 2.0;
                string value3 = ""foo"";
                bool value4 = false;
                int value5 = 5;
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}{Value3}{Value4}{Value5}"", value1, value2, value3, value4, value5);
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
    public async Task Logger_Log_7PlaceholdersWith7Params_ObjectArray_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        
        namespace analyzer.test;
        
        public class Logger_Log_7PlaceholdersWith7Params_ObjectArray
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 1;
                double value2 = 2.0;
                string value3 = ""foo"";
                bool value4 = false;
                int value5 = 5;
                int value6 = 10;
                int value7 = 11;
                object[] values = new object[] { value1, value2, value3, value4, value5, value6, value7 };
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}{Value3}{Value4}{Value5}{Value6}{Value7}"", values);
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
    public async Task Logger_Log_7PlaceholdersWith7Params_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        
        namespace analyzer.test;
        
        public class Logger_Log_7PlaceholdersWith7Params
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value1 = 1;
                double value2 = 2.0;
                string value3 = ""foo"";
                bool value4 = false;
                int value5 = 5;
                int value6 = 10;
                int value7 = 11;
                _logger.%LOGLEVEL%(""Doing something with value: {Value1}{Value2}{Value3}{Value4}{Value5}{Value6}{Value7}"", value1, value2, value3, value4, value5, value6, value7);
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
    public async Task Logger_Log_JsonObject_ObjectArray_ShouldFindNoDiag(string logLevel)
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
        
        public class Logger_Log_JsonObject_ObjectArray
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                var jsonSample = new Dto { Id = 1, Name = ""Test"" };
                object[] values = new object[] { jsonSample };
                _logger.%LOGLEVEL%(""Here comes some json {@Json}"", values);
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
    public async Task Logger_Log_JsonObject_ShouldFindNoDiag(string logLevel)
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
        
        public class Logger_Log_JsonObject
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                var jsonSample = new Dto { Id = 1, Name = ""Test"" };
                _logger.%LOGLEVEL%(""Here comes some json {@Json}"", jsonSample);
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
    public async Task Logger_Log_NoParameters_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_NoParameters
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                _logger.%LOGLEVEL%(""Doing something without parameters"");
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
    public async Task Logger_Log_WithException_1PlaceholderWith1Arg_ObjectArray_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_WithException_1PlaceholderWith1Arg_ObjectArray
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value = 42;
                var exception = new InvalidOperationException(""invalid operation"");
                object[] values = new object[] { value };
                _logger.%LOGLEVEL%(exception, ""An error occurred while doing something with value: {Value}"", values);
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
    public async Task Logger_Log_WithException_1PlaceholderWith1Arg_ShouldFindNoDiag(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        
        namespace analyzer.test;
        
        public class Logger_Log_WithException_1PlaceholderWith1Arg
        {
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldFindNoDiag()
            {
                int value = 42;
                var exception = new InvalidOperationException(""invalid operation"");
                _logger.%LOGLEVEL%(exception, ""An error occurred while doing something with value: {Value}"", value);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}