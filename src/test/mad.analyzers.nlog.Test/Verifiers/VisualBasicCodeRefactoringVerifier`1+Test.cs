using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.VisualBasic.Testing;

namespace mad.analyzers.nlog.Test.Verifiers
{
    public static partial class VisualBasicCodeRefactoringVerifier<TCodeRefactoring>
        where TCodeRefactoring : CodeRefactoringProvider, new()
    {
        public class Test : VisualBasicCodeRefactoringTest<TCodeRefactoring, MSTestVerifier>
        {
        }
    }
}
