using System.Text.RegularExpressions;
using TestDataGen.IO;
using TestDataGen.Test.Builders;
using TestDataGen.Test.Pipeline;

namespace TestDataGen.Test;

/// <summary>
/// Orchestrates the full Test Compiler pipeline.
/// Reads demo files from <paramref name="sourceDir"/> (which must follow the
/// <c>usings/ | diagnostics/ | nodiag/</c> structure) and writes MSTest
/// test-class <c>.cs</c> files to <paramref name="outDir"/>.
/// </summary>
/// <remarks>
/// Pipeline per demo file (steps follow requirements §4.4):
/// <list type="number">
///   <item>Load usings table from <c>{source}/usings/</c>.</item>
///   <item>Split header / body (<see cref="SourceCleaner.SplitHeaderAndBody"/>).</item>
///   <item>Parse header → <see cref="DiagnosticDescriptor"/>s
///         (<see cref="DiagnosticMarkerInserter.ParseHeader"/>).</item>
///   <item><em>(diagnostics only)</em> Insert <c>{|#N:…|}</c> markers on the raw body
///         (<see cref="DiagnosticMarkerInserter.InsertMarkers"/>).</item>
///   <item>Clean body (<see cref="SourceCleaner.Clean"/>).</item>
///   <item>Parameterise log level (<see cref="LogLevelParameterizer.Parameterize"/>).</item>
///   <item>Resolve preamble (<see cref="PreambleResolver.Resolve"/>).</item>
///   <item>Wrap in code sample (<see cref="CodeSampleWrapper.Wrap"/>).</item>
///   <item>Verbatim-escape (<see cref="VerbatimEscaper.Escape"/>).</item>
///   <item>Build <see cref="TestMethodSpec"/>.</item>
///   <item>Bundle methods into <see cref="TestClassSpec"/> per sub-directory.</item>
///   <item>Render and write via <see cref="OutputWriter"/>.</item>
/// </list>
/// </remarks>
internal class TestCompiler(OutputWriter writer)
{
    // Regex to extract class name and method name for the test-method title.
    private static readonly Regex ClassNameRegex  = new(@"public\s+class\s+(\w+)", RegexOptions.Compiled);
    private static readonly Regex MethodNameRegex = new(@"public\s+void\s+(\w+)\s*\(", RegexOptions.Compiled);

    /// <summary>
    /// Compiles all demo files found under <paramref name="sourceDir"/> into test files
    /// written to <paramref name="outDir"/>.
    /// </summary>
    /// <returns>Total number of test-class files written.</returns>
    public int Compile(string sourceDir, string outDir)
    {
        // ── Step 1: load usings table ──────────────────────────────────────────
        var usingsTable = LoadUsingsTable(Path.Combine(sourceDir, "usings"));

        int count = 0;

        // ── Process diagnostics ────────────────────────────────────────────────
        count += ProcessGroup(
            sourceDir    : sourceDir,
            subDir       : "diagnostics",
            outSubDir    : Path.Combine(outDir, "diagnostics"),
            isDiagnostics : true,
            usingsTable  : usingsTable);

        // ── Process nodiag ────────────────────────────────────────────────────
        count += ProcessGroup(
            sourceDir    : sourceDir,
            subDir       : "nodiag",
            outSubDir    : Path.Combine(outDir, "nodiag"),
            isDiagnostics : false,
            usingsTable  : usingsTable);

        return count;
    }

    // =========================================================================

    private int ProcessGroup(
        string sourceDir,
        string subDir,
        string outSubDir,
        bool   isDiagnostics,
        IReadOnlyDictionary<string, string> usingsTable)
    {
        var groupRoot = Path.Combine(sourceDir, subDir);
        if (!Directory.Exists(groupRoot))
            return 0;

        var files = Directory.GetFiles(groupRoot, "*.cs", SearchOption.AllDirectories);

        // Group files by their containing sub-directory (relative to sourceDir).
        var groups = files.GroupBy(f =>
            Path.GetRelativePath(sourceDir, Path.GetDirectoryName(f)!)
                .Replace('\\', '/')          // normalise for class-name derivation
        );

        int written = 0;

        foreach (var group in groups)
        {
            var methods = new List<TestMethodSpec>();

            foreach (var filePath in group.OrderBy(f => f))
            {
                var spec = ProcessFile(filePath, isDiagnostics, usingsTable);
                if (spec is not null)
                    methods.Add(spec);
            }

            if (methods.Count == 0) continue;

            var relativePath = group.Key;             // e.g. "diagnostics/MAD2017/logger"
            var className    = TestClassBundler.ToClassName(relativePath);
            var outputPath   = Path.Combine(outSubDir, $"{className}.cs");

            var classSpec = new TestClassSpec(className, outputPath, methods);
            var rendered  = TestClassBundler.Render(classSpec, isDiagnostics);

            writer.Write(outputPath, rendered);
            written++;
        }

        return written;
    }

    // ── Per-file pipeline ────────────────────────────────────────────────────

    private TestMethodSpec? ProcessFile(
        string filePath,
        bool   isDiagnostics,
        IReadOnlyDictionary<string, string> usingsTable)
    {
        var rawContent = File.ReadAllText(filePath);

        // Step 2: split header and body
        var (header, body) = SourceCleaner.SplitHeaderAndBody(rawContent);

        // Step 3: parse diagnostic descriptors from header
        var diagnostics = isDiagnostics
            ? DiagnosticMarkerInserter.ParseHeader(header)
            : (IReadOnlyList<DiagnosticDescriptor>)[];

        // Step 4 (diagnostics): insert {|#N:…|} markers on the full body BEFORE using
        // extraction, because line:column values (ADR-012) are relative to the full body.
        var bodyWithMarkers = isDiagnostics
            ? DiagnosticMarkerInserter.InsertMarkers(body, diagnostics)
            : body;

        // Step 2a (ADR-010): extract using directives after marker insertion so that
        // line numbers used above remain valid.
        var (sourceUsings, bodyWithoutUsings) = UsingExtractor.Extract(bodyWithMarkers);

        // Step 5: clean (remove comment lines and empty lines)
        var cleanedBody = SourceCleaner.Clean(bodyWithoutUsings);

        // Step 6: parameterise log level
        var parameterized = LogLevelParameterizer.Parameterize(cleanedBody);

        // Step 7: resolve preamble
        var withPreamble = PreambleResolver.Resolve(parameterized, usingsTable);

        // Step 8: wrap in code-sample scaffolding (also adds 4-space indent)
        var wrapped = CodeSampleWrapper.Wrap(withPreamble, sourceUsings);

        // Step 9: verbatim-escape for embedding in @"…"
        var escaped = VerbatimEscaper.Escape(wrapped);

        // Step 10: derive test-method name from the cleaned (unmarked) body
        var cleanedForTitle = SourceCleaner.Clean(body);
        var methodName = GetTestTitle(cleanedForTitle);
        if (methodName is null) return null;

        return new TestMethodSpec(methodName, escaped, diagnostics);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Derives the test-method name as <c>{ClassName}_{VoidMethodName}</c>.
    /// Returns <c>null</c> when the class or method name cannot be found.
    /// </summary>
    private static string? GetTestTitle(string cleanedSource)
    {
        var classMatch  = ClassNameRegex.Match(cleanedSource);
        var methodMatch = MethodNameRegex.Match(cleanedSource);
        if (!classMatch.Success || !methodMatch.Success) return null;
        return $"{classMatch.Groups[1].Value}_{methodMatch.Groups[1].Value}";
    }

    /// <summary>
    /// Loads all <c>.cs</c> files from <paramref name="usingsDir"/> into a dictionary
    /// keyed by base file name (= symbol name). Returns an empty dictionary when the
    /// directory does not exist.
    /// </summary>
    private static IReadOnlyDictionary<string, string> LoadUsingsTable(string usingsDir)
    {
        var table = new Dictionary<string, string>(StringComparer.Ordinal);
        if (!Directory.Exists(usingsDir))
            return table;

        foreach (var file in Directory.GetFiles(usingsDir, "*.cs"))
        {
            var symbol  = Path.GetFileNameWithoutExtension(file);
            var content = File.ReadAllText(file);
            table[symbol] = SourceCleaner.Clean(content);
        }

        return table;
    }
}
