using TestDataGen.Test.Pipeline;

namespace TestDataGen.Test.Builders;

/// <summary>
/// Renders individual test methods (<c>nodiag</c> and <c>diagnostics</c> variants)
/// from a <see cref="TestMethodSpec"/>.
/// </summary>
/// <remarks>
/// <para>
/// The returned method text is not yet indented at the class level; it is intended
/// to be collected by <see cref="TestClassBundler"/> which adds the 8-space
/// class-body indentation via <c>IndentLines 8</c>.
/// </para>
/// <para>
/// The processed code inside the <c>@"…"</c> verbatim string is additionally
/// indented by 4 spaces here (matching the PowerShell
/// <c>IndentTextBlock -indent_spaces 4</c> call inside
/// <c>GenerateNoDiagnosticTestMethod</c> / <c>GenerateDiagnosticsTestMethod</c>).
/// </para>
/// </remarks>
internal static class TestMethodBuilder
{
    private const string DataRows =
        "[DataRow(\"Trace\")]\n" +
        "[DataRow(\"Debug\")]\n" +
        "[DataRow(\"Info\")]\n" +
        "[DataRow(\"Warn\")]\n" +
        "[DataRow(\"Error\")]\n" +
        "[DataRow(\"Fatal\")]";

    // -------------------------------------------------------------------------

    /// <summary>
    /// Builds a <c>[DataTestMethod]</c>-annotated test method that expects no diagnostics.
    /// </summary>
    public static string BuildNoDiag(TestMethodSpec spec)
    {
        var indentedCode = IndentLines(spec.ProcessedCode, 4);

        return
            "[DataTestMethod]\n" +
            DataRows + "\n" +
            $"public async Task {spec.MethodName}(string logLevel)\n" +
            "{\n" +
            "    var template = @\"\n" +
            indentedCode + "\n" +
            "    \";\n" +
            "    var test = template.Replace(\"%LOGLEVEL%\", logLevel);\n" +
            "\n" +
            "    await VerifyCS.VerifyAnalyzerAsync(test);\n" +
            "}";
    }

    /// <summary>
    /// Builds a <c>[TestMethod]</c>-annotated test method that expects exactly one
    /// diagnostic. The diagnostic parameters are formatted as integer literals
    /// (<c>int</c>) or string literals (all other types).
    /// </summary>
    public static string BuildDiagnostics(TestMethodSpec spec)
    {
        var indentedCode = IndentLines(spec.ProcessedCode, 4);

        // Expected-diagnostic definitions
        var diagDefs        = new System.Text.StringBuilder();
        var diagVars        = new List<string>();
        var locationIndices = DiagnosticMarkerInserter.ComputeLocationIndices(spec.Diagnostics);

        for (int i = 0; i < spec.Diagnostics.Count; i++)
        {
            var d   = spec.Diagnostics[i];
            var varName = $"expected{i}";
            diagVars.Add(varName);

            var paramList = string.Join(", ", d.Parameters.Select(p =>
                p.Type == "int" ? p.Value : $"\"{p.Value}\""));

            diagDefs.Append(
                $"    var {varName} = VerifyCS.Diagnostic(\"{d.Id}\")"+
                $".WithLocation({locationIndices[i]})"+
                (paramList.Length > 0 ? $".WithArguments({paramList})" : "") +
                ";\n");
        }

        var diagCallArgs = string.Join(", ", Enumerable.Prepend(diagVars, "test"));

        return
            "[TestMethod]\n" +
            DataRows + "\n" +
            $"public async Task {spec.MethodName}(string logLevel)\n" +
            "{\n" +
            "    var template = @\"\n" +
            indentedCode + "\n" +
            "    \";\n" +
            "    var test = template.Replace(\"%LOGLEVEL%\", logLevel);\n" +
            "\n" +
            diagDefs.ToString() +
            $"    await VerifyCS.VerifyAnalyzerAsync({diagCallArgs});\n" +
            "}";
    }

    // -------------------------------------------------------------------------

    private static string IndentLines(string text, int spaces)
    {
        var prefix = new string(' ', spaces);
        var lines  = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        return string.Join("\n", lines.Select(l => prefix + l));
    }
}
