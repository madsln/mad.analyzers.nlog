namespace TestDataGen.Test.Builders;

/// <summary>
/// Collects <see cref="TestMethodSpec"/> objects into <see cref="TestClassSpec"/> instances
/// (one per source sub-directory) and renders them as complete <c>.cs</c> test-class files.
/// </summary>
internal static class TestClassBundler
{
    private const string ClassTemplate =
        "using Microsoft.VisualStudio.TestTools.UnitTesting;\n" +
        "using System.Threading.Tasks;\n" +
        "using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<\n" +
        "    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,\n" +
        "    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;\n" +
        "\n" +
        "namespace mad.analyzers.nlog.Test.generated;\n" +
        "\n" +
        "[TestClass]\n" +
        "public class {CLASSNAME}\n" +
        "{\n" +
        "{CLASSBODY}\n" +
        "}";

    // -------------------------------------------------------------------------

    /// <summary>
    /// Converts a relative source sub-directory path to the test-class name.
    /// Path separators (<c>\</c> and <c>/</c>) are replaced by <c>_</c>;
    /// the suffix <c>_Tests</c> is appended.
    /// </summary>
    public static string ToClassName(string relativePath) =>
        relativePath.Replace('\\', '_').Replace('/', '_') + "_Tests";

    /// <summary>
    /// Renders a <see cref="TestClassSpec"/> into a complete C# source file string.
    /// </summary>
    public static string Render(TestClassSpec spec, bool isDiagnostics)
    {
        // Build each method with the appropriate builder, then indent 8 spaces for class body.
        var methodBlocks = spec.Methods.Select(m =>
        {
            var methodText = isDiagnostics
                ? TestMethodBuilder.BuildDiagnostics(m)
                : TestMethodBuilder.BuildNoDiag(m);
            return IndentLines(methodText, 4);
        });

        var classBody = string.Join("\n\n", methodBlocks);

        return ClassTemplate
            .Replace("{CLASSNAME}", spec.ClassName)
            .Replace("{CLASSBODY}", classBody);
    }

    // -------------------------------------------------------------------------

    private static string IndentLines(string text, int spaces)
    {
        var prefix = new string(' ', spaces);
        var lines  = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        return string.Join("\n", lines.Select(l => prefix + l));
    }
}
