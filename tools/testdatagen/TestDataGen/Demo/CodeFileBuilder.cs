namespace TestDataGen.Demo;

/// <summary>
/// Builds the complete C# file content for a <see cref="DemoVariant"/>.
/// </summary>
/// <remarks>
/// Output format (replicates Python <c>template_class_body</c>):
/// <code>
/// // MAD2017={StartOffset}:{Length};int={PlaceholderCount};int={ArgumentCount}
/// {using directives}
///
/// namespace {Namespace};
///
/// public class {ClassName}
/// {
///     {LoggerDeclaration}
///
///     public void ShouldRaiseDiagnostic_MAD2017()
///     {
///         {local var declarations}
///         {logging call}
///     }
/// }
/// </code>
/// </remarks>
internal static class CodeFileBuilder
{
    private const string Indent1 = "    ";
    private const string Indent2 = "        ";

    /// <summary>
    /// Builds the full file content (header + body) for the given variant.
    /// The file body uses <c>\n</c> line endings; the header is computed from the body.
    /// See ADR-004 for the line-ending strategy.
    /// </summary>
    public static string Build(DemoVariant variant)
    {
        var body = BuildBody(variant);
        var header = MetadataHeaderBuilder.Build(variant, body);
        return header + "\n" + body;
    }

    // ---------------------------------------------------------------------------

    private static string BuildBody(DemoVariant variant)
    {
        // Usings section – joined by \n, matches Python "linefeed.join(spec.usings)"
        var usingsSection = string.Join("\n", variant.UsingDirectives);

        // Logger field – indented 1 level
        var loggerField = Indent1 + variant.LoggerDeclaration;

        // Method body lines: all local var declarations + the logging call, each indented 2 levels
        var allMethodLines = variant.LocalVarDeclarations
                                    .Append(variant.LoggingCallLine);
        var methodBody = string.Join("\n", allMethodLines.Select(l => Indent2 + l));

        // Replicate Python's template_class_body exactly (raw string with \n).
        // The template starts with "%USINGS%\n" and ends with "}\n".
        return
            usingsSection + "\n" +
            "\n" +
            $"namespace {variant.Namespace};\n" +
            "\n" +
            $"public class {variant.ClassName}\n" +
            "{\n" +
            loggerField + "\n" +
            "\n" +
            Indent1 + "public void ShouldRaiseDiagnostic_MAD2017()\n" +
            Indent1 + "{\n" +
            methodBody + "\n" +
            Indent1 + "}\n" +
            "}\n";
    }
}
