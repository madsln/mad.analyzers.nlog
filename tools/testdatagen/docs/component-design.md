# Komponentendesign: TestDataGen CLI

> Referenz: [architecture.md](architecture.md)

---

## 1. CLI Layer

### `CliRouter` (`Cli/CliRouter.cs`)

**Verantwortung:** Argument-Parsing und Routing zu den Subkommandos.

**Schnittstelle:**
```csharp
static class CliRouter
{
    static int Run(string[] args);
    static void PrintHelp();
    static void PrintHelp(string subcommand);
}
```

**Logik:**
- `args[0]` bestimmt das Subkommando (`demo` | `test` | `all` | `--help`).
- Unbekannte Subkommandos → `PrintHelp()` + Exit-Code 1.
- Fehlende Pflichtparameter → `PrintHelp(subcommand)` + Exit-Code 1.
- Delegiert an `DemoCommand.Run(...)`, `TestCommand.Run(...)`, `AllCommand.Run(...)`.

---

### `DemoCommand` (`Cli/Commands/DemoCommand.cs`)

**Parameter:** `--out <Pfad>`

**Logik:**
1. `--out` fehlt → Fehler.
2. Erstellt `DemoGenerator` mit konfiguriertem `OutputWriter`.
3. Ruft `DemoGenerator.Generate(outPath)` auf.
4. Gibt Zusammenfassung (Anzahl generierter Dateien) auf `stdout` aus.

---

### `TestCommand` (`Cli/Commands/TestCommand.cs`)

**Parameter:** `--source <Pfad>`, `[--out <Pfad>]`

**Logik:**
1. `--source` fehlt → Fehler.
2. `--source` existiert nicht → Exit-Code 2 + Hinweis auf `demo`-Subkommando.
3. `--source` enthält keine `.cs`-Dateien → Exit-Code 2 + Hinweis.
4. `--out` nicht angegeben → Standard: `../../test/mad.analyzers.nlog.Test/generated` relativ zu `--source`.
5. Erstellt `TestCompiler` und ruft `Compile(sourcePath, outPath)` auf.

---

### `AllCommand` (`Cli/Commands/AllCommand.cs`)

**Parameter:** `--demo-out <Pfad>`, `[--test-out <Pfad>]`

**Logik:** Delegiert sequenziell an `DemoCommand` und `TestCommand`.

---

## 2. Config Layer

### `LoggerConfig` (`Config/LoggerConfig.cs`)

Statische, unveränderliche Konfiguration:

```csharp
static class LoggerConfig
{
    // Logger-Typen
    static readonly string[] LoggerTypes = ["Logger", "ILogger", "ILoggerExtensions"];

    // Felddeklaration pro Logger-Typ
    static readonly IReadOnlyDictionary<string, string> FieldDeclarations;
}
```

**Daten:**
| Logger-Typ | Felddeklaration |
|---|---|
| `Logger` | `private readonly Logger _logger = LogManager.GetCurrentClassLogger();` |
| `ILogger` | `private readonly ILogger _logger = LogManager.GetCurrentClassLogger();` |
| `ILoggerExtensions` | `private readonly Logger _logger = LogManager.GetCurrentClassLogger();` |

---

### `CallTemplateConfig` (`Config/CallTemplateConfig.cs`)

```csharp
record CallTemplate(
    string LoggerType,
    string Pattern,         // enthält {MESSAGE_AND_ARGS}
    bool HasException,
    bool HasCultureInfo
);

static class CallTemplateConfig
{
    static readonly IReadOnlyList<CallTemplate> Templates;
}
```

Enthält alle Aufruf-Varianten für `Logger`, `ILogger`, `ILoggerExtensions` – mit und ohne Exception, mit und ohne CultureInfo.

---

### `TestCaseConfig` (`Config/TestCaseConfig.cs`)

```csharp
record TestCase(
    string Name,            // PascalCase, z. B. "1PlaceholderWith2Args"
    string MessageTemplate, // z. B. "{Placeholder0} foo {Placeholder1}"
    IReadOnlyList<string> ArgumentNames // z. B. ["arg0", "arg1"]
);

static class TestCaseConfig
{
    static readonly IReadOnlyList<TestCase> TestCases;
}
```

Enthält alle benannten Testfall-Kombinationen (0–6 Platzhalter, 0–7 Argumente).

---

### `ArgumentConfig` (`Config/ArgumentConfig.cs`)

```csharp
static class ArgumentConfig
{
    // z. B. "arg0" → "int arg0 = 42;"
    static readonly IReadOnlyDictionary<string, string> LocalVarDeclarations;
}
```

---

## 3. Demo Layer

### `VariantBuilder` (`Demo/VariantBuilder.cs`)

**Verantwortung:** Erzeugt für jede Kombination aus Logger-Typ × Call-Template × Testfall bis zu 3 `DemoVariant`-Objekte.

```csharp
static class VariantBuilder
{
    static IEnumerable<DemoVariant> Build(
        string loggerType,
        CallTemplate template,
        TestCase testCase);
}
```

**Varianten-Logik:**

| Variante | `VariantSuffix` | Überspringen wenn |
|---|---|---|
| Direct | `""` | – |
| ObjectArray | `"_ObjectArray"` | – |

> **Hinweis (ADR-011):** Die `ReadOnlySpan`-Variante wurde entfernt.

**Argument-Auflösung pro Variante:**

- **Direct:** Argumente direkt in Call-Template → `_logger.Info("{tmpl}", arg0, arg1);`
- **ObjectArray:** `object[] args = [arg0, arg1]; _logger.Info("{tmpl}", args);`

---

### `ClassNameBuilder` (`Demo/ClassNameBuilder.cs`)

```csharp
static class ClassNameBuilder
{
    static string Build(
        string loggerType,
        bool hasException,
        string testCaseName,
        string variantSuffix);
}
```

**Konvention:**
```
{LoggerType}_Log_[WithException_]{PascalCaseTestcaseName}[_ObjectArray]
```

---

### `MetadataHeaderBuilder` (`Demo/MetadataHeaderBuilder.cs`)

**Verantwortung:** Berechnet Zeile/Spalte des Message-Template-Strings und erzeugt den `// MAD2017=line:column:length;...`-Header (ADR-012).

```csharp
static class MetadataHeaderBuilder
{
    static string Build(DemoVariant variant, string classBodyCode);
}
```

**Berechnung (ADR-012):**
1. Erzeuge `fileBodyCode` (gesamten Datei-Code ohne Header, mit `\n` Zeilenenden).
2. Suche Position des Message-Template-Strings (`"..."`) in `fileBodyCode`.
3. Konvertiere absolute Zeichenposition in 1-basierte `line` und `column`.
4. Header-Format: `// MAD2017={line}:{column}:{Länge};int={PlaceholderCount};int={ArgumentCount}`

---

### `CodeFileBuilder` (`Demo/CodeFileBuilder.cs`)

**Verantwortung:** Erzeugt den vollständigen Dateiinhalt aus einem `DemoVariant`.

```csharp
static class CodeFileBuilder
{
    static string Build(DemoVariant variant);
}
```

**Ausgabeformat:**
```
{Metadaten-Header}
{using-Direktiven}

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.{loggertype};

public class {Klassenname}
{
    {LoggerFieldDeclaration}

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        {LocalVarDeclarations}
        {LoggingCallLine}
    }
}
```

---

### `DemoGenerator` (`Demo/DemoGenerator.cs`)

**Verantwortung:** Orchestriert die Demo-Generierung.

```csharp
class DemoGenerator(OutputWriter writer)
{
    int Generate(string outDir);
    // Gibt Anzahl geschriebener Dateien zurück
}
```

**Ablauf:**
1. Iteriert über alle `LoggerConfig.LoggerTypes`.
2. Für jeden Typ: alle passenden `CallTemplateConfig.Templates`.
3. Für jedes Template × Testfall: `VariantBuilder.Build(...)`.
4. Für jede Variante: `CodeFileBuilder.Build(...)` → `OutputWriter.Write(path, content)`.

---

## 4. Test Layer

### `TestCompiler` (`Test/TestCompiler.cs`)

**Verantwortung:** Orchestriert die gesamte Test-Pipeline.

```csharp
class TestCompiler
{
    int Compile(string sourceDir, string outDir);
    // Gibt Anzahl geschriebener Dateien zurück
}
```

**Ablauf:**
1. Lädt Usings-Tabelle aus `{sourceDir}/usings/`.
2. Verarbeitet `diagnostics/` und `nodiag/` separat.
3. Für jede Demo-Datei: Pipeline-Schritte 2–8.
4. Bündelt Methoden zu Testklassen (Schritt 9).
5. Schreibt Ausgabedateien (Schritt 10).

---

### Pipeline-Klassen (`Test/Pipeline/`)

Jede Pipeline-Klasse ist eine reine Transformation (kein I/O):

#### `SourceCleaner`
```csharp
static class SourceCleaner
{
    // Entfernt Kommentarzeilen (// ...) und Leerzeilen
    static string Clean(string source);
    // Gibt Header-Kommentare separat zurück
    static (string Header, string Body) SplitHeaderAndBody(string source);
}
```

#### `LogLevelParameterizer`
```csharp
static class LogLevelParameterizer
{
    static readonly string[] LogLevels = ["Trace", "Debug", "Info", "Warn", "Error", "Fatal"];
    // Ersetzt gefundenen Log-Level-Token durch %LOGLEVEL%
    static string Parameterize(string source);
}
```

#### `PreambleResolver`
```csharp
static class PreambleResolver
{
    // Sucht Symbole aus usingsTable im Quelltext, stellt gefundene Preambles voran
    static string Resolve(string source, IReadOnlyDictionary<string, string> usingsTable);
}
```

#### `DiagnosticMarkerInserter`
```csharp
static class DiagnosticMarkerInserter
{
    // Parst Header-Zeilen → DiagnosticDescriptor[]
    static IReadOnlyList<DiagnosticDescriptor> ParseHeader(string headerComment);
    // Fügt {|#N:...|}-Marker in Quelltext ein
    static string InsertMarkers(string source, IReadOnlyList<DiagnosticDescriptor> diagnostics);
    // Berechnet Positions-Indizes für WithLocation()-Aufrufe
    static int[] ComputeLocationIndices(IReadOnlyList<DiagnosticDescriptor> diagnostics);
}
```

**Header-Parse-Grammatik (ADR-012):**
```
"// " DiagId "=" Line ":" Column ":" Length (";" Type "=" Value)*
```

#### `UsingExtractor` (ADR-010)
```csharp
static class UsingExtractor
{
    // Extrahiert using-Direktiven aus dem Quelltext; gibt sie separat zurück.
    // Wird nach DiagnosticMarkerInserter ausgeführt, damit Zeilennummern gültig bleiben.
    static (IReadOnlyList<string> Usings, string Body) Extract(string source);
}
```

#### `CodeSampleWrapper`
```csharp
static class CodeSampleWrapper
{
    // Bettet bereinigten Test-Code in file-scoped namespace ein;
    // zusammengeführte using-Direktiven werden vorangestellt.
    static string Wrap(string cleanedSource, IReadOnlyList<string> sourceUsings);
}
```

**Ausgabe:**
```csharp
using System;
using NLog;
{weitere using-Direktiven aus Demo-Datei}

namespace analyzer.test;

{eingerückter cleanedSource}
```

#### `VerbatimEscaper`
```csharp
static class VerbatimEscaper
{
    static string Escape(string code); // " → ""
}
```

---

### Builder-Klassen (`Test/Builders/`)

#### `TestMethodBuilder`
```csharp
static class TestMethodBuilder
{
    static string BuildNoDiag(TestMethodSpec spec);
    static string BuildDiagnostics(TestMethodSpec spec);
}
```

#### `TestClassBundler`
```csharp
static class TestClassBundler
{
    // Leitet Testklassenname aus relativem Quell-Pfad ab
    static string ToClassName(string relativePath);

    // Rendert die vollständige .cs-Datei mit korrektem namespace und VerifyCS-Alias
    static string Render(TestClassSpec spec, bool isDiagnostics);
}
```

---

## 5. I/O Layer

### `OutputWriter` (`IO/OutputWriter.cs`)

```csharp
class OutputWriter
{
    // Schreibt UTF-8-kodierte Datei, erstellt Verzeichnisse bei Bedarf
    void Write(string filePath, string content);
}
```

---

## 6. Datenmodell-Übersicht

```
Config Layer          Domain Layer (Demo)           Domain Layer (Test)
────────────          ───────────────────           ───────────────────
LoggerConfig    ──▶   DemoVariant                   DiagnosticDescriptor
CallTemplate    ─►   (ClassName, LoggerType,        (Id, Line, Column,
TestCase        ──▶    Namespace, LoggerDecl,         Length, Parameters)
ArgumentConfig  ──▶    UsingDirectives,
                       LocalVarDecls,               TestMethodSpec
                       LoggingCallLine,              (MethodName,
                       PlaceholderCount,              ProcessedCode,
                       ArgumentCount)                Diagnostics)

                                                    TestClassSpec
                                                     (ClassName,
                                                      OutputPath,
                                                      Methods)
```
