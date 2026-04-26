# Workload-Plan: TestDataGen CLI

> Referenz: [architecture.md](architecture.md) | [component-design.md](component-design.md)

Die Workloads sind **sequenziell** zu bearbeiten. Spätere Workloads setzen die Ergebnisse der vorhergehenden voraus.

---

## Übersicht

| Workload | Titel | Abhängigkeit |
|---|---|---|
| WL-01 | Projektgerüst & Build-Setup | – |
| WL-02 | Config Layer (Eingebettete Konfiguration) | WL-01 |
| WL-03 | Datenmodelle & I/O Layer | WL-01 |
| WL-04 | Demo Generator | WL-02, WL-03 |
| WL-05 | Test Compiler Pipeline | WL-02, WL-03 |
| WL-06 | CLI Layer & Entry Point | WL-04, WL-05 |
| WL-07 | Manuelle Verifikation & Feinschliff | WL-06 |

---

## WL-01 – Projektgerüst & Build-Setup

**Ziel:** Lauffähiges, leeres .NET-Konsolenprojekt mit der definierten Ordnerstruktur.

**Aufgaben:**

1. Erstelle `tools/testdatagen/TestDataGen/TestDataGen.csproj`:
   - Target: `net9.0` (oder aktuellste LTS-Version)
   - `<Nullable>enable</Nullable>`
   - `<ImplicitUsings>enable</ImplicitUsings>`
   - `<Nullable>enable</Nullable>`
   - Ausgabe: `TestDataGen.exe` (Self-Contained optional, via Publish-Profil)

2. Erstelle leere Ordnerstruktur gemäß [architecture.md §2](architecture.md):
   - `Cli/Commands/`
   - `Config/`
   - `Demo/`
   - `Test/Pipeline/`
   - `Test/Builders/`
   - `IO/`

3. Erstelle `Program.cs` mit minimalem Einstiegspunkt:
   ```csharp
   // TODO: WL-06
   Console.WriteLine("TestDataGen – not yet implemented");
   return 1;
   ```

4. Stelle sicher, dass `dotnet build` fehlerfrei durchläuft.

**Abnahmekriterium:** `dotnet build` → kein Fehler, `dotnet run` → gibt Platzhaltertext aus.

---

## WL-02 – Config Layer (Eingebettete Konfiguration)

**Ziel:** Alle eingebetteten Konfigurationsdaten als statische, unveränderliche Klassen.

> Referenz: [component-design.md §2](component-design.md)

**Aufgaben:**

### 2.1 `Config/LoggerConfig.cs`

- Array `LoggerTypes`: `["Logger", "ILogger", "ILoggerExtensions"]`
- Dictionary `FieldDeclarations`: Logger-Typ → Felddeklarations-String (siehe [requirements.md §3.2](../requirements.md))

### 2.2 `Config/CallTemplateConfig.cs`

Definiere den Record `CallTemplate(string LoggerType, string Pattern, bool HasException, bool HasCultureInfo)`.

Befülle `Templates` mit allen Aufruf-Mustern für alle drei Logger-Typen:
- Muster ohne Exception: `_logger.%LOGLEVEL%({MESSAGE_AND_ARGS});`
- Muster mit Exception: `_logger.%LOGLEVEL%(ex, {MESSAGE_AND_ARGS});`
- `ILoggerExtensions`-Muster mit CultureInfo zusätzlich:
  `NLog.LogManager.GetLogger("x").%LOGLEVEL%(CultureInfo.CurrentCulture, {MESSAGE_AND_ARGS});`

> Die genauen Methoden-Signaturen sind aus [spec.md](../spec.md) und dem Python-Referenzskript `scripts/gen_MAD2017_demo_files.py` zu entnehmen.

### 2.3 `Config/TestCaseConfig.cs`

Definiere den Record `TestCase(string Name, string MessageTemplate, IReadOnlyList<string> ArgumentNames)`.

Befülle `TestCases` mit allen relevanten Kombinationen:
- 0–6 Platzhalter (`{Placeholder0}`–`{Placeholder5}`)
- 0–7 Argumente
- Alle benannten Varianten aus dem Python-Referenzskript

> Exakte Testfall-Daten aus `scripts/gen_MAD2017_demo_files.py` übernehmen (1:1-Portierung).

### 2.4 `Config/ArgumentConfig.cs`

- Dictionary `LocalVarDeclarations`: Argument-Name → Variablendeklarations-String
  - z. B. `"arg0"` → `"int arg0 = 42;"`
  - Alle Argument-Namen, die in `TestCaseConfig.TestCases` verwendet werden

**Abnahmekriterium:** Alle Config-Klassen kompilieren, Unit-Test prüft stichprobenartig Werte.

---

## WL-03 – Datenmodelle & I/O Layer

**Ziel:** Zentrale Records/Klassen für den Datentransport sowie der `OutputWriter`.

> Referenz: [architecture.md §6](architecture.md), [component-design.md §5](component-design.md)

**Aufgaben:**

### 3.1 Datenmodelle (z. B. `Models.cs` oder einzelne Dateien)

Implementiere die Records:

```csharp
record DemoVariant(
    string ClassName,
    string LoggerType,
    string Namespace,
    string LoggerDeclaration,
    IReadOnlyList<string> UsingDirectives,
    IReadOnlyList<string> LocalVarDeclarations,
    string LoggingCallLine,
    string MessageTemplate,   // für MetadataHeaderBuilder-Berechnung
    int PlaceholderCount,
    int ArgumentCount
);

record DiagnosticDescriptor(
    string Id,
    int Line,
    int Column,
    int Length,
    IReadOnlyList<(string Type, string Value)> Parameters
);

record TestMethodSpec(
    string MethodName,
    string ProcessedCode,
    IReadOnlyList<DiagnosticDescriptor> Diagnostics
);

record TestClassSpec(
    string ClassName,
    string OutputPath,
    IReadOnlyList<TestMethodSpec> Methods
);
```

### 3.2 `IO/OutputWriter.cs`

```csharp
class OutputWriter
{
    void Write(string filePath, string content);
    // Directory.CreateDirectory für übergeordnete Verzeichnisse
    // File.WriteAllText(..., Encoding.UTF8)
}
```

**Abnahmekriterium:** `OutputWriter` schreibt eine Testdatei in ein nicht existentes Verzeichnis.

---

## WL-04 – Demo Generator

**Ziel:** Vollständige Implementierung der Demo-Generierungspipeline.

> Referenz: [component-design.md §3](component-design.md), [requirements.md §3](../requirements.md)

**Aufgaben:**

### 4.1 `Demo/ClassNameBuilder.cs`

Implementiere:
```
{LoggerType}_Log_[WithException_]{PascalCaseTestcaseName}[_ObjectArray]
```

### 4.2 `Demo/VariantBuilder.cs`

Implementiere die zwei Varianten (Direct, ObjectArray) (ADR-011: ReadOnlySpan wurde entfernt):
- Argument-Strings pro Variante korrekt zusammenstellen

### 4.3 `Demo/MetadataHeaderBuilder.cs`

Implementiere (ADR-012):
1. Erzeuge den Klassen-Body (ohne Header) mit `\n`-Zeilenenden
2. Suche Position des Message-Template-Strings, konvertiere in 1-basierte `line` und `column`
3. Erzeuge `// MAD2017={line}:{column}:{length};int={PlaceholderCount};int={ArgumentCount}`

### 4.4 `Demo/CodeFileBuilder.cs`

Kombiniere alle Teile zur vollständigen Datei:
- Bedingt `using System.Globalization;` (nur wenn `HasCultureInfo`)
- Namespace: `mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.{loggertype}`

### 4.5 `Demo/DemoGenerator.cs`

Orchestriere: iteriere über `LoggerTypes × Templates × TestCases`, rufe `VariantBuilder` und `CodeFileBuilder` auf, schreibe via `OutputWriter`.

Ausgabepfad-Konvention: `{outDir}/{loggertype}/{ClassName}.cs`

**Abnahmekriterium:**
- `dotnet run -- demo --out ./tmp/demo` erzeugt eine sinnvolle Dateistruktur
- Stichproben-Check: Header-Offsets sind korrekt (manueller Vergleich mit Python-Skript-Ausgabe)

---

## WL-05 – Test Compiler Pipeline

**Ziel:** Vollständige Implementierung der Test-Kompilierungspipeline.

> Referenz: [component-design.md §4](component-design.md), [requirements.md §4](../requirements.md)

**Aufgaben:**

### 5.1 `Test/Pipeline/SourceCleaner.cs`

- `Clean(string source)`: entfernt `//`-Zeilen und Leerzeilen
- `SplitHeaderAndBody(string source)`: trennt führende Kommentarzeilen vom Rest

### 5.2 `Test/Pipeline/LogLevelParameterizer.cs`

- Erkennt Log-Level-Bezeichner (`Trace`, `Debug`, `Info`, `Warn`, `Error`, `Fatal`) im Quelltext
- Ersetzt ersten gefundenen Treffer durch `%LOGLEVEL%`
- Methode `Parameterize(string source) → string`

### 5.3 `Test/Pipeline/PreambleResolver.cs`

- Erhält `usingsTable: IReadOnlyDictionary<string, string>`
- Durchsucht Quelltext nach Schlüsselwörtern der Tabelle
- Stellt gefundene Preamble-Blöcke dem Code voran

### 5.4 `Test/Pipeline/DiagnosticMarkerInserter.cs`

**Header-Parser (ADR-012):**
```
"// MAD2017=" Line ":" Column ":" Length ";int=" PlaceholderCount ";int=" ArgumentCount
```
→ `DiagnosticDescriptor` mit `Id="MAD2017"`, `Line`, `Column`, `Length`, `Parameters=[("int", count1), ("int", count2)]`

**Marker-Einfügung:**
- Für jedes Diagnostic: Absolute Position aus `Line`/`Column` berechnen, `source[pos..pos+Length]` → `{|#N:...|}`
- Mehrere Diagnostics: rückwärts einfügen (höhere Position zuerst), damit Offsets valide bleiben

### 5.5 `Test/Pipeline/CodeSampleWrapper.cs`

Wickelt bereinigten Code in:
```csharp
using System;
using NLog;
{weitere using-Direktiven aus Demo-Datei}

namespace analyzer.test;

{eingerückter Code}
```

### 5.6 `Test/Pipeline/VerbatimEscaper.cs`

`Escape(string code)`: `"` → `""`

### 5.7 `Test/Builders/TestMethodBuilder.cs`

Implementiere `BuildNoDiag` und `BuildDiagnostics` gemäß den Templates aus [requirements.md §4.4 Schritt 8](../requirements.md).

- `[DataTestMethod]` + 6 `[DataRow("...")]`
- `var template = @"..."` + `template.Replace("%LOGLEVEL%", logLevel)`
- Bei `diagnostics`: `expected0`-Variable + `WithArguments`

**Parameterformatierung für `WithArguments`:**
- `int`-Parameter → direkt als Integer-Literal
- Sonstige → als `"string"`-Literal

### 5.8 `Test/Builders/TestClassBundler.cs`

- Sammelt `TestMethodSpec`-Objekte nach Unterverzeichnis
- Klassenname: Unterverzeichnis-Pfadtrenner `\` und `/` → `_`, suffix `_Tests`
- Rendert vollständige Klasse gemäß Template aus [requirements.md §4.4 Schritt 9](../requirements.md)

### 5.9 `Test/TestCompiler.cs`

Orchestriere alle Pipeline-Schritte:
1. Usings-Tabelle laden
2. `diagnostics/` und `nodiag/` getrennt verarbeiten
3. Für jede Datei: Schritte 2–8
4. Schritt 9: Bündeln
5. Schritt 10: Schreiben via `OutputWriter`

**Abnahmekriterium:**
- `dotnet run -- test --source ./tmp/demo --out ./tmp/tests` erzeugt `.cs`-Testdateien
- Stichproben-Check: `{|#N:...|}`-Marker sind korrekt positioniert
- Vergleich mit Ausgabe des PowerShell-Referenzskripts `CompileTestData.ps1`

---

## WL-06 – CLI Layer & Entry Point

**Ziel:** Vollständiges CLI mit Argument-Parsing, Routing, Fehlerbehandlung und Hilfetext.

> Referenz: [component-design.md §1](component-design.md), [requirements.md §2](../requirements.md)

**Aufgaben:**

### 6.1 `Cli/CliRouter.cs`

- Routing auf `demo` | `test` | `all` | `--help` | `help`
- Unbekannte Subkommandos: Hilfetext + Exit-Code 1
- Exceptions: Catch-all in `Program.cs`, Ausgabe auf `stderr`, Exit-Code 3

### 6.2 `Cli/Commands/DemoCommand.cs`

- Parst `--out`
- Fehlende Pflichtparameter: `PrintHelp("demo")` + Exit-Code 1
- Erfolgsmeldung: `$"demo: {n} Dateien generiert in {outPath}"`

### 6.3 `Cli/Commands/TestCommand.cs`

- Parst `--source`, `[--out]`
- Prüft Existenz und Inhalt von `--source`
- Berechnet Standard-`--out` wenn nicht angegeben

### 6.4 `Cli/Commands/AllCommand.cs`

- Parst `--demo-out`, `[--test-out]`
- Delegiert an `DemoGenerator` und `TestCompiler`

### 6.5 Hilfetexte

Implementiere strukturierte Hilfetexte für:
- Globale Hilfe (`TestDataGen --help`)
- `demo`-Subkommando
- `test`-Subkommando
- `all`-Subkommando

### 6.6 `Program.cs`

```csharp
try
{
    return CliRouter.Run(args);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Fehler: {ex.Message}");
    return 3;
}
```

**Abnahmekriterium:**
- `TestDataGen --help` zeigt vollständigen Hilfetext
- `TestDataGen demo` (ohne `--out`) zeigt Hilfe + Exit-Code 1
- `TestDataGen test --source /nonexistent` zeigt Fehlermeldung + Exit-Code 2
- `TestDataGen all --demo-out ./tmp/demo` läuft durch

---

## WL-07 – Manuelle Verifikation & Feinschliff

**Ziel:** Sicherstellung der funktionalen Korrektheit durch Vergleich mit den Referenzskripten.

**Aufgaben:**

### 7.1 Vergleich Demo-Ausgabe

1. Führe Python-Referenzskript aus: `python scripts/gen_MAD2017_demo_files.py --out ./tmp/ref-demo`
2. Führe `TestDataGen demo --out ./tmp/new-demo` aus
3. Vergleiche Ausgaben (Dateianzahl, Dateinamen, Dateiinhalte)
4. Korrigiere Abweichungen in WL-02–WL-04

### 7.2 Vergleich Test-Ausgabe

1. Führe PowerShell-Referenzskript aus: `CompileTestData.ps1` auf Referenz-Demo-Ausgabe
2. Führe `TestDataGen test --source ./tmp/ref-demo` aus
3. Vergleiche `.cs`-Testdateien (Methodennamen, Marker-Positionen, Diagnostics)
4. Korrigiere Abweichungen in WL-05

### 7.3 End-to-End-Test mit bestehendem Testprojekt

1. Führe `TestDataGen all --demo-out src/demo/.../generated` aus
2. Baue das Testprojekt `mad.analyzers.nlog.Test`
3. Führe alle generierten Tests aus
4. Alle Tests müssen grün sein

### 7.4 Optionaler Feinschliff

- Ausgabeformat der Erfolgs-/Fehlermeldungen vereinheitlichen
- Exit-Code-Behandlung verifizieren (via `$LASTEXITCODE` in PowerShell)
- `--help`-Ausgaben finalisieren
