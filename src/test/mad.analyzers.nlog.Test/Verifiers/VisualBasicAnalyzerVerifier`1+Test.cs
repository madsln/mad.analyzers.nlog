using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Testing;

namespace mad.analyzers.nlog.Test.Verifiers
{
    public static partial class VisualBasicAnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public class Test : VisualBasicAnalyzerTest<TAnalyzer, MSTestVerifier>
        {
            public Test()
            {
            }
        }
    }
}
