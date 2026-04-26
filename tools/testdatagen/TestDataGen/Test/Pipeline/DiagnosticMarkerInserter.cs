namespace TestDataGen.Test.Pipeline;

/// <summary>
/// Parses the <c>// MAD2017=…</c> metadata header of a demo file and inserts
/// Roslyn test-markup markers (<c>{|#N:…|}</c>) at the declared positions.
/// </summary>
internal static class DiagnosticMarkerInserter
{
    /// <summary>
    /// Parses all header comment lines into <see cref="DiagnosticDescriptor"/> objects.
    /// </summary>
    /// <param name="headerComment">
    /// The raw header string (lines starting with <c>//</c>), as returned by
    /// <see cref="SourceCleaner.SplitHeaderAndBody"/>.
    /// </param>
    public static IReadOnlyList<DiagnosticDescriptor> ParseHeader(string headerComment)
    {
        if (string.IsNullOrWhiteSpace(headerComment))
            return [];

        var result = new List<DiagnosticDescriptor>();
        var lines = headerComment.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (!line.StartsWith("//", StringComparison.Ordinal))
                continue;

            // Strip leading "// "
            var content = line.TrimStart('/').Trim();

            // Split "MAD2017=12:28:27;int=1;int=2" by "="
            var eqIdx = content.IndexOf('=');
            if (eqIdx < 0) continue;

            var id = content[..eqIdx].Trim();
            var rest = content[(eqIdx + 1)..];

            var attributes = rest.Split(';');
            if (attributes.Length < 1) continue;

            // First attribute: "line:column:length" (ADR-012)
            var segments = attributes[0].Split(':');
            if (segments.Length != 3) continue;
            if (!int.TryParse(segments[0], out int markerLine)) continue;
            if (!int.TryParse(segments[1], out int column)) continue;
            if (!int.TryParse(segments[2], out int length)) continue;

            // Remaining attributes: "type=value"
            var parameters = new List<(string Type, string Value)>();
            for (int i = 1; i < attributes.Length; i++)
            {
                var kv = attributes[i].Split('=', 2);
                if (kv.Length == 2)
                    parameters.Add((kv[0].Trim(), kv[1].Trim()));
            }

            result.Add(new DiagnosticDescriptor(id, markerLine, column, length, parameters));
        }

        return result;
    }

    /// <summary>
    /// Computes a location-index for each diagnostic, where diagnostics that share the
    /// same <c>(Line, Column, Length)</c> position receive the same index.
    /// </summary>
    /// <remarks>
    /// The returned array has the same length as <paramref name="diagnostics"/>.
    /// Indices are assigned in encounter order: the first unique span gets index 0,
    /// the second unique span gets index 1, etc.
    /// Two diagnostics at identical positions therefore both get index 0 (same marker
    /// <c>{|#0:…|}</c>) and both call <c>.WithLocation(0)</c> in the generated test.
    /// </remarks>
    public static int[] ComputeLocationIndices(IReadOnlyList<DiagnosticDescriptor> diagnostics)
    {
        var result      = new int[diagnostics.Count];
        var spanToIndex = new Dictionary<(int Line, int Column, int Length), int>();
        int nextIndex   = 0;

        for (int i = 0; i < diagnostics.Count; i++)
        {
            var key = (diagnostics[i].Line, diagnostics[i].Column, diagnostics[i].Length);
            if (!spanToIndex.TryGetValue(key, out int idx))
            {
                idx = nextIndex++;
                spanToIndex[key] = idx;
            }
            result[i] = idx;
        }

        return result;
    }

    /// <summary>
    /// Inserts <c>{|#N:…|}</c> markers into <paramref name="source"/> at the positions
    /// declared by <paramref name="diagnostics"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>Line</c> and <c>Column</c> stored in each <see cref="DiagnosticDescriptor"/>
    /// are 1-based and resolved against the normalised (<c>\n</c>) body (ADR-012).
    /// </para>
    /// <para>
    /// Markers are inserted in reverse order (highest absolute position first) so that
    /// earlier offsets remain valid throughout the process.
    /// </para>
    /// </remarks>
    public static string InsertMarkers(string source, IReadOnlyList<DiagnosticDescriptor> diagnostics)
    {
        if (diagnostics.Count == 0)
            return source;

        var locationIndices = ComputeLocationIndices(diagnostics);
        var seenSpans = new HashSet<(int Line, int Column, int Length)>();
        var indexed = diagnostics
            .Select((d, i) => (Diag: d, LocationIndex: locationIndices[i]))
            .OrderByDescending(t => ResolveAbsolutePosition(source, t.Diag.Line, t.Diag.Column))
            .ToList();

        foreach (var (diag, n) in indexed)
        {
            var spanKey = (diag.Line, diag.Column, diag.Length);
            if (!seenSpans.Add(spanKey)) continue;

            int absPos = ResolveAbsolutePosition(source, diag.Line, diag.Column);
            if (absPos < 0 || absPos + diag.Length > source.Length) continue;

            var before = source[..absPos];
            var marked = source[absPos..(absPos + diag.Length)];
            var after  = source[(absPos + diag.Length)..];
            source = $"{before}{{|#{n}:{marked}|}}{after}";
        }

        return source;
    }

    /// <summary>
    /// Resolves the absolute character position for a 1-based <paramref name="line"/>
    /// and <paramref name="column"/> within a string that uses exclusively <c>\n</c>
    /// line endings (SourceCleaner invariant, ADR-012).
    /// </summary>
    private static int ResolveAbsolutePosition(string normalizedBody, int line, int column)
    {
        ReadOnlySpan<char> remaining = normalizedBody.AsSpan();
        for (int i = 1; i < line; i++)
        {
            int nl = remaining.IndexOf('\n');
            if (nl < 0)
                throw new InvalidOperationException($"Line {line} does not exist in body.");
            remaining = remaining[(nl + 1)..];
        }
        // column is 1-based → index = column - 1
        return (normalizedBody.Length - remaining.Length) + (column - 1);
    }
}
