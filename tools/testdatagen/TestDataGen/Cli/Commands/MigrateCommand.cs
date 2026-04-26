using TestDataGen.Demo;
using TestDataGen.Test.Pipeline;

namespace TestDataGen.Cli.Commands;

/// <summary>
/// Migrates demo files from the old <c>startOffset:length</c> header format (ADR-004/007/008)
/// to the new <c>line:column:length</c> format (ADR-012).
/// </summary>
internal static class MigrateCommand
{
    public static int Run(string[] args)
    {
        if (args.Contains("--help"))
        {
            HelpTexts.PrintMigrate();
            return 0;
        }

        string? sourcePath = ParseArg(args, "--source");
        if (sourcePath is null)
        {
            Console.Error.WriteLine("Fehler: Parameter '--source' ist erforderlich.");
            Console.Error.WriteLine();
            HelpTexts.PrintMigrate();
            return 1;
        }

        if (!Directory.Exists(sourcePath))
        {
            Console.Error.WriteLine($"Fehler: Quellverzeichnis '{sourcePath}' existiert nicht.");
            return 2;
        }

        var files = Directory.GetFiles(sourcePath, "*.cs", SearchOption.AllDirectories);
        int migrated = 0;
        int skipped  = 0;

        foreach (var file in files)
        {
            var result = MigrateFile(file);
            if (result == MigrateResult.Migrated) migrated++;
            else skipped++;
        }

        Console.WriteLine($"migrate: {migrated} Dateien migriert, {skipped} unverändert in {Path.GetFullPath(sourcePath)}");
        return 0;
    }

    private enum MigrateResult { Migrated, Skipped }

    private static MigrateResult MigrateFile(string filePath)
    {
        var rawContent = File.ReadAllText(filePath);

        // Detect original line-ending for offset resolution
        var lineEnding = DetectLineEnding(rawContent);

        var (header, body) = SourceCleaner.SplitHeaderAndBody(rawContent);
        if (string.IsNullOrWhiteSpace(header))
            return MigrateResult.Skipped;

        // Check whether the header already uses the new 3-part format
        bool needsMigration = false;
        var headerLines = header.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in headerLines)
        {
            if (!line.StartsWith("//", StringComparison.Ordinal)) continue;
            var content = line.TrimStart('/').Trim();
            var eqIdx = content.IndexOf('=');
            if (eqIdx < 0) continue;
            var rest = content[(eqIdx + 1)..];
            var firstAttr = rest.Split(';')[0];
            var parts = firstAttr.Split(':');
            if (parts.Length == 2)
            {
                needsMigration = true;
                break;
            }
        }

        if (!needsMigration)
            return MigrateResult.Skipped;

        // Normalise body to \n for position calculation (mirrors SourceCleaner invariant).
        var normalizedBody = body.Replace("\r\n", "\n");

        // Rebuild header lines with converted positions
        var newHeaderLines = new List<string>();
        foreach (var line in headerLines)
        {
            if (!line.StartsWith("//", StringComparison.Ordinal))
            {
                newHeaderLines.Add(line);
                continue;
            }

            var content = line.TrimStart('/').Trim();
            var eqIdx = content.IndexOf('=');
            if (eqIdx < 0)
            {
                newHeaderLines.Add(line);
                continue;
            }

            var id   = content[..eqIdx].Trim();
            var rest = content[(eqIdx + 1)..];
            var attributes = rest.Split(';');
            var firstAttr  = attributes[0];
            var parts      = firstAttr.Split(':');

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int startOffset) ||
                !int.TryParse(parts[1], out int length))
            {
                // Already new format or unrecognised – keep as-is
                newHeaderLines.Add(line);
                continue;
            }

            // Resolve absolute position using the old ADR-004/CRLF formula
            int classStart = normalizedBody.IndexOf("public class", StringComparison.Ordinal);
            if (classStart < 0)
            {
                newHeaderLines.Add(line);
                continue;
            }

            int absPos = ResolveOldAbsolutePosition(normalizedBody, classStart, startOffset, length, lineEnding);
            if (absPos < 0)
            {
                Console.Error.WriteLine($"Warnung: Konnte Position für '{id}' in '{filePath}' nicht auflösen – übersprungen.");
                newHeaderLines.Add(line);
                continue;
            }

            var (newLine, newColumn) = MetadataHeaderBuilder.PositionToLineColumn(normalizedBody, absPos);

            // Rebuild the attribute list with the new format
            var suffix = attributes.Length > 1
                ? ";" + string.Join(";", attributes[1..])
                : "";
            newHeaderLines.Add($"// {id}={newLine}:{newColumn}:{length}{suffix}");
        }

        // Reconstruct file with original line endings
        string lineEnding2 = lineEnding == OldLineEndingKind.CRLF ? "\r\n" : "\n";
        var newHeader = string.Join(lineEnding2, newHeaderLines);
        var newContent = newHeader + lineEnding2 + body;

        File.WriteAllText(filePath, newContent, System.Text.Encoding.UTF8);
        return MigrateResult.Migrated;
    }

    private enum OldLineEndingKind { LF, CRLF }

    private static OldLineEndingKind DetectLineEnding(string content)
    {
        bool hasCRLF = content.Contains("\r\n");
        return hasCRLF ? OldLineEndingKind.CRLF : OldLineEndingKind.LF;
    }

    /// <summary>
    /// Resolves absolute position using the old ADR-004/ADR-008 formula.
    /// </summary>
    private static int ResolveOldAbsolutePosition(
        string normalizedBody, int classStart, int startOffset, int length, OldLineEndingKind lineEnding)
    {
        int upperBound = Math.Min(classStart + startOffset + 10, normalizedBody.Length);
        int crlfCount  = 0;
        for (int i = classStart + 1; i < upperBound; i++)
            if (normalizedBody[i] == '\n') crlfCount++;

        int absPos = lineEnding == OldLineEndingKind.CRLF
            ? classStart + (startOffset - crlfCount)
            : classStart + (startOffset - crlfCount + 2);

        if (absPos < 0 || absPos + length > normalizedBody.Length) return -1;
        return absPos;
    }

    private static string? ParseArg(string[] args, string key)
    {
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == key) return args[i + 1];
        return null;
    }
}
