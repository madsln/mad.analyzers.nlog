using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test
{
    [TestClass]
    public class MadAnalyzerExplorationTests
    {
        private static readonly string _validLogMessageWithOneParameter = @"
using NLog;

public class TypeName {
    public static readonly %LOGGERTYPE% _logger = LogManager.GetCurrentClassLogger();

    public void DoSomething(int value)
    {
        _logger.%LOGLEVEL%(""Doing something with value: {Value}"", value);
    }
}
";
        private static readonly string _validLogMessageWithoutParameter = @"
using NLog;

public class FooBar {
    public static readonly %LOGGERTYPE% _logger = LogManager.GetCurrentClassLogger();

    public void DoSomething()
    {
        _logger.%LOGLEVEL%(""Doing something without parameters"");
    }
}
";
        private static readonly string _invalidLogMessageWithTwoParametersButOnlyOneSupplied = @"
using NLog;

public class FooBar {
    public static readonly %LOGGERTYPE% _logger = LogManager.GetCurrentClassLogger();

    public void DoSomething(int value)
    {
        _logger.%LOGLEVEL%({|MAD2017:""Doing something with multiple values: {Value1}, {Value2}""|}, value);
    }
}
";

        private static string GetFormattedCode(string template, string loggerType, string logLevel)
        {
            var result = template.Replace("%LOGGERTYPE%", loggerType);
            result = result.Replace("%LOGLEVEL%", logLevel);
            return result;
        }

        //No diagnostics expected to show up
        [TestMethod]
        [DataRow("Trace")]
        [DataRow("Debug")]
        [DataRow("Info")]
        [DataRow("Warn")]
        [DataRow("Error")]
        [DataRow("Fatal")]
        public async Task ValidLogMessageWithOneParameter_ShouldNotShowDiagnostics(string logLevel)
        {
            var test = GetFormattedCode(_validLogMessageWithOneParameter, "Logger", logLevel);

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //No diagnostics expected to show up
        [TestMethod]
        [DataRow("Trace")]
        [DataRow("Debug")]
        [DataRow("Info")]
        [DataRow("Warn")]
        [DataRow("Error")]
        [DataRow("Fatal")]
        public async Task ValidLogMessageWithoutParameter_ShouldNotShowDiagnostics(string logLevel)
        {
            var test = GetFormattedCode(_validLogMessageWithoutParameter, "Logger", logLevel);

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //No diagnostics expected to show up
        [TestMethod]
        [DataRow("Trace")]
        [DataRow("Debug")]
        [DataRow("Info")]
        [DataRow("Warn")]
        [DataRow("Error")]
        [DataRow("Fatal")]
        public async Task InvalidLogMessageWithTwoParametersButOnlyOneSupplied_ShouldShowDiagnostics(string logLevel)
        {
            var test = GetFormattedCode(_invalidLogMessageWithTwoParametersButOnlyOneSupplied, "Logger", logLevel);

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        [Ignore("Reference test")]
        public async Task ReferenceTest()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("mad.analyzers.common.LogMessageTemplateParameterCountMismatch").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}