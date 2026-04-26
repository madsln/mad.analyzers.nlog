using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test.generated;

[TestClass]
public class diagnostics_MAD2017_iloggerextensions_Tests
{
    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_1PlaceholderWith2Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_1PlaceholderWith2Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var objectArray = new object[] { intArg, boolArg };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_1PlaceholderWith2Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_1PlaceholderWith2Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}""|}, intArg, boolArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_1PlaceholderWith5Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_1PlaceholderWith5Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_1PlaceholderWith5Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_1PlaceholderWith5Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}""|}, intArg, boolArg, stringArg, doubleArg, objectArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_1PlaceholderWithoutArgs_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_1PlaceholderWithoutArgs_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var objectArray = new object[] {  };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_1PlaceholderWithoutArgs_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_1PlaceholderWithoutArgs
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}""|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_2Placeholder2With3Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_2Placeholder2With3Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var objectArray = new object[] { intArg, boolArg, stringArg };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(2, 3);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_2Placeholder2With3Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_2Placeholder2With3Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}""|}, intArg, boolArg, stringArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(2, 3);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_3PlaceholdersWith2Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_3PlaceholdersWith2Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var objectArray = new object[] { intArg, boolArg };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_3PlaceholdersWith2Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_3PlaceholdersWith2Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, intArg, boolArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_3PlaceholdersWith4Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_3PlaceholdersWith4Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 4);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_3PlaceholdersWith4Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_3PlaceholdersWith4Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, intArg, boolArg, stringArg, doubleArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 4);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_3PlaceholdersWith5Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_3PlaceholdersWith5Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_3PlaceholdersWith5Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_3PlaceholdersWith5Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, intArg, boolArg, stringArg, doubleArg, objectArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_6PlaceholdersWith3Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_6PlaceholdersWith3Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var objectArray = new object[] { intArg, boolArg, stringArg };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 3);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_6PlaceholdersWith3Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_6PlaceholdersWith3Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, intArg, boolArg, stringArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 3);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_6PlaceholdersWith5Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_6PlaceholdersWith5Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_6PlaceholdersWith5Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_6PlaceholdersWith5Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, intArg, boolArg, stringArg, doubleArg, objectArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_6PlaceholdersWith7Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_6PlaceholdersWith7Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var decimalArg = 4.20m;
                var longArg = 8008135L;
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg, decimalArg, longArg };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 7);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_6PlaceholdersWith7Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_6PlaceholdersWith7Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var decimalArg = 4.20m;
                var longArg = 8008135L;
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, intArg, boolArg, stringArg, doubleArg, objectArg, decimalArg, longArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 7);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_6PlaceholdersWithNoArgs_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_6PlaceholdersWithNoArgs_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var objectArray = new object[] {  };
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_6PlaceholdersWithNoArgs_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_6PlaceholdersWithNoArgs
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                _logger.ConditionalTrace({|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_NoPlaceholderWith2Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_NoPlaceholderWith2Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var objectArray = new object[] { intArg, boolArg };
                _logger.ConditionalTrace({|#0:""Message without placeholder""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(0, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_NoPlaceholderWith2Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_NoPlaceholderWith2Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                _logger.ConditionalTrace({|#0:""Message without placeholder""|}, intArg, boolArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(0, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_NoPlaceholderWith5Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_NoPlaceholderWith5Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
                _logger.ConditionalTrace({|#0:""Message without placeholder""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(0, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_NoPlaceholderWith5Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_NoPlaceholderWith5Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                _logger.ConditionalTrace({|#0:""Message without placeholder""|}, intArg, boolArg, stringArg, doubleArg, objectArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(0, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_1PlaceholderWith2Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_1PlaceholderWith2Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var objectArray = new object[] { intArg, boolArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_1PlaceholderWith2Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_1PlaceholderWith2Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}""|}, intArg, boolArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_1PlaceholderWith5Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_1PlaceholderWith5Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_1PlaceholderWith5Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_1PlaceholderWith5Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}""|}, intArg, boolArg, stringArg, doubleArg, objectArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_1PlaceholderWithoutArgs_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_1PlaceholderWithoutArgs_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var objectArray = new object[] {  };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_1PlaceholderWithoutArgs_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_1PlaceholderWithoutArgs
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}""|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_2Placeholder2With3Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_2Placeholder2With3Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var objectArray = new object[] { intArg, boolArg, stringArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(2, 3);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_2Placeholder2With3Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_2Placeholder2With3Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}""|}, intArg, boolArg, stringArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(2, 3);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_3PlaceholdersWith2Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_3PlaceholdersWith2Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var objectArray = new object[] { intArg, boolArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_3PlaceholdersWith2Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_3PlaceholdersWith2Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, intArg, boolArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_3PlaceholdersWith4Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_3PlaceholdersWith4Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 4);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_3PlaceholdersWith4Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_3PlaceholdersWith4Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, intArg, boolArg, stringArg, doubleArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 4);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_3PlaceholdersWith5Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_3PlaceholdersWith5Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_3PlaceholdersWith5Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_3PlaceholdersWith5Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}""|}, intArg, boolArg, stringArg, doubleArg, objectArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(3, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_6PlaceholdersWith3Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_6PlaceholdersWith3Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var objectArray = new object[] { intArg, boolArg, stringArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 3);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_6PlaceholdersWith3Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_6PlaceholdersWith3Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, intArg, boolArg, stringArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 3);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_6PlaceholdersWith5Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_6PlaceholdersWith5Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_6PlaceholdersWith5Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_6PlaceholdersWith5Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, intArg, boolArg, stringArg, doubleArg, objectArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_6PlaceholdersWith7Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_6PlaceholdersWith7Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var decimalArg = 4.20m;
                var longArg = 8008135L;
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg, decimalArg, longArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 7);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_6PlaceholdersWith7Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_6PlaceholdersWith7Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var decimalArg = 4.20m;
                var longArg = 8008135L;
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, intArg, boolArg, stringArg, doubleArg, objectArg, decimalArg, longArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 7);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_6PlaceholdersWithNoArgs_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_6PlaceholdersWithNoArgs_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var objectArray = new object[] {  };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_6PlaceholdersWithNoArgs_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_6PlaceholdersWithNoArgs
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}""|});
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(6, 0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_NoPlaceholderWith2Args_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_NoPlaceholderWith2Args_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var objectArray = new object[] { intArg, boolArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message without placeholder""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(0, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_NoPlaceholderWith2Args_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_NoPlaceholderWith2Args
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message without placeholder""|}, intArg, boolArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(0, 2);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_NoPlaceholderWith5Params_ObjectArray_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_NoPlaceholderWith5Params_ObjectArray
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                var objectArray = new object[] { intArg, boolArg, stringArg, doubleArg, objectArg };
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message without placeholder""|}, objectArray);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(0, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }

    [TestMethod]
    [DataRow("Trace")]
    [DataRow("Debug")]
    [DataRow("Info")]
    [DataRow("Warn")]
    [DataRow("Error")]
    [DataRow("Fatal")]
    public async Task ILoggerExtensions_Log_WithException_NoPlaceholderWith5Params_ShouldRaiseDiagnostic_MAD2017(string logLevel)
    {
        var template = @"
        using System;
        using NLog;
        using System.Globalization;
        
        namespace analyzer.test;
        
        public class ILoggerExtensions_Log_WithException_NoPlaceholderWith5Params
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            public void ShouldRaiseDiagnostic_MAD2017()
            {
                var exception = new InvalidOperationException(""Demo exception"");
                var intArg = 42;
                var boolArg = true;
                var stringArg = ""foo"";
                var doubleArg = 3.14;
                var objectArg = new object();
                _logger.ConditionalTrace(exception, CultureInfo.InvariantCulture, {|#0:""Message without placeholder""|}, intArg, boolArg, stringArg, doubleArg, objectArg);
            }
        }
        ";
        var test = template.Replace("%LOGLEVEL%", logLevel);
    
        var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(0, 5);
        await VerifyCS.VerifyAnalyzerAsync(test, expected0);
    }
}