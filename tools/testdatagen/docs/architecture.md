# Architektur: TestDataGen CLI

## 1. Systemübersicht

`TestDataGen` ist eine .NET-Konsolenanwendung, die in zwei klar getrennten Pipelines arbeitet:

```
┌──────────────────────────────────────────────────────────────────┐
│                        TestDataGen.exe                           │
│                                                                  │
│  ┌─────────┐   ┌───────────────────┐   ┌──────────────────────┐ │
│  │   CLI   │──▶│  Demo Generator   │──▶│   Test Compiler      │ │
│  │  Layer  │   │  (Subcommand demo)│   │  (Subcommand test)   │ │
│  └─────────┘   └───────────────────┘   └──────────────────────┘ │
│       │                 ▲                        ▲               │
│       │         ┌───────┴────────────────────────┘              │
│       └────────▶│         Config Layer                          │
│                 │  (Eingebettete Konfiguration)                  │
│                 └───────────────────────────────────────────────┘│
└──────────────────────────────────────────────────────────────────┘
```

### Subkommando-Routing

```
TestDataGen demo  ──▶  DemoCommand  ──▶  DemoGenerator
TestDataGen test  ──▶  TestCommand  ──▶  TestCompiler
TestDataGen all   ──▶  AllCommand   ──▶  DemoGenerator ──▶ TestCompiler
```

---

## 2. Projektstruktur

```
tools/testdatagen/
├── docs/
│   ├── architecture.md          ← dieses Dokument
│   ├── component-design.md      ← Detaildesign der Komponenten
│   └── workloads.md             ← Implementierungsplan
└── TestDataGen/
    ├── TestDataGen.csproj
    ├── Program.cs
    ├── Cli/
    │   ├── CliRouter.cs          ← Argument-Parsing & Routing
    │   └── Commands/
    │       ├── DemoCommand.cs
    │       ├── TestCommand.cs
    │       └── AllCommand.cs
    ├── Config/
    │   ├── LoggerConfig.cs       ← Logger-Typen & Deklarationen
    │   ├── CallTemplateConfig.cs ← Call-Templates pro Logger-Typ
    │   ├── TestCaseConfig.cs     ← Testfälle (Name, Template, Args)
    │   └── ArgumentConfig.cs    ← Argument-Variablendeklarationen
    ├── Demo/
    │   ├── DemoGenerator.cs      ← Orchestriert Demo-Generierung
    │   ├── VariantBuilder.cs     ← Erzeugt Varianten (Direct/Array/Span)
    │   ├── ClassNameBuilder.cs   ← Klassenname-Konvention
    │   ├── MetadataHeaderBuilder.cs ← // MAD2017=... Header
    │   └── CodeFileBuilder.cs    ← Erzeugt vollständigen Dateiinhalt
    ├── Test/
    │   ├── TestCompiler.cs       ← Orchestriert Test-Kompilierung
    │   ├── Pipeline/
    │   │   ├── SourceCleaner.cs         ← Header/Body-Split + Bereinigung
    │   │   ├── DiagnosticMarkerInserter.cs ← Schritt 3: Header-Parse + Marker-Einfügung
    │   │   ├── UsingExtractor.cs        ← Schritt 4: Using-Direktiven extrahieren
    │   │   ├── LogLevelParameterizer.cs ← Schritt 6: %LOGLEVEL%-Ersetzung
    │   │   ├── PreambleResolver.cs      ← Schritt 7: Usings-Preamble
    │   │   ├── CodeSampleWrapper.cs     ← Schritt 8: namespace-Wrapping
    │   │   └── VerbatimEscaper.cs       ← Schritt 9: `"` → `""`
    │   └── Builders/
    │       ├── TestMethodBuilder.cs     ← Schritt 10: Testmethode
    │       └── TestClassBundler.cs      ← Schritt 11: Testklasse
    └── IO/
        └── OutputWriter.cs             ← UTF-8-Ausgabe, Dir-Erstellung
```

---

## 3. Schichtenmodell

```
┌─────────────────────────────────────────┐
│         CLI Layer (Cli/)                │  Argument-Parsing, Routing,
│  CliRouter, DemoCommand, TestCommand,   │  Fehlerausgabe, Exit-Codes
│  AllCommand                             │
├─────────────────────────────────────────┤
│       Application Layer                 │  Orchestrierung der Pipelines
│  DemoGenerator, TestCompiler            │
├─────────────────────────────────────────┤
│       Domain Layer                      │  Reine Transformationslogik,
│  Demo/*, Test/Pipeline/*, Test/Builders │  keine I/O-Abhängigkeiten
├─────────────────────────────────────────┤
│       Config Layer (Config/)            │  Eingebettete statische Daten
│  LoggerConfig, CallTemplateConfig,      │
│  TestCaseConfig, ArgumentConfig         │
├─────────────────────────────────────────┤
│       I/O Layer (IO/)                   │  Dateisystem-Zugriffe
│  OutputWriter                           │
└─────────────────────────────────────────┘
```

---

## 4. Datenfluss: Demo-Pipeline

```
Config Layer
  LoggerConfig ──┐
  CallTemplates ──┼──▶  VariantBuilder ──▶ (LoggerType × Template × Testfall × Variante)
  TestCases ──────┘           │
  ArgumentConfig ─────────────▼
                        CodeFileBuilder
                              │
                    ┌─────────┴──────────┐
                    │  MetadataHeader    │  ClassNameBuilder
                    │  using-Direktiven  │
                    │  namespace         │
                    │  class { ... }     │
                    └─────────┬──────────┘
                              ▼
                        OutputWriter
                   {OutDir}/diagnostics/MAD2017/{loggertype}/{ClassName}.cs
```

---

## 5. Datenfluss: Test-Pipeline

```
{source}/usings/*.cs ──▶ [Schritt 1] UsingsTable (Symbol → bereinigter Inhalt)

{source}/diagnostics/**/*.cs ─┐
{source}/nodiag/**/*.cs ───────┼──▶ für jede Datei:
                               │
                    [Schritt 2] SourceCleaner
                               │  Header / Body trennen
                    [Schritt 3*] DiagnosticMarkerInserter  (* nur diagnostics)
                               │  Header parsen, {|#N:...|} einfügen
                    [Schritt 4] UsingExtractor
                               │  Using-Direktiven extrahieren
                    [Schritt 5] SourceCleaner
                               │  Kommentare + Leerzeilen entfernen
                    [Schritt 6] LogLevelParameterizer
                               │  LogLevel-Token → %LOGLEVEL%
                    [Schritt 7] PreambleResolver
                               │  Preamble-Blöcke voranstellen
                    [Schritt 8] CodeSampleWrapper
                               │  namespace analyzer.test; (file-scoped)
                    [Schritt 9] VerbatimEscaper
                               │  " → ""
                               ▼
                   [Schritt 10] TestMethodBuilder
                               │  DataTestMethod mit logLevel-Parameter
                               ▼
                   [Schritt 11] TestClassBundler
                               │  Methoden gleicher Unterverzeichnisse bündeln
                               ▼
                        OutputWriter
               {out}/diagnostics/{ClassName}_Tests.cs
               {out}/nodiag/{ClassName}_Tests.cs
```

---

## 6. Zentrale Datenmodelle

### `DemoVariant`
```csharp
record DemoVariant(
    string ClassName,
    string LoggerType,
    string Namespace,
    string LoggerDeclaration,
    IReadOnlyList<string> UsingDirectives,
    IReadOnlyList<string> LocalVarDeclarations,
    string LoggingCallLine,
    int PlaceholderCount,
    int ArgumentCount
);
```

### `DiagnosticDescriptor`
```csharp
record DiagnosticDescriptor(
    string Id,
    int Line,
    int Column,
    int Length,
    IReadOnlyList<(string Type, string Value)> Parameters
);
```

### `TestMethodSpec`
```csharp
record TestMethodSpec(
    string MethodName,
    string ProcessedCode,
    IReadOnlyList<DiagnosticDescriptor> Diagnostics
);
```

### `TestClassSpec`
```csharp
record TestClassSpec(
    string ClassName,
    string OutputPath,
    IReadOnlyList<TestMethodSpec> Methods
);
```

---

## 7. Fehlerbehandlung

| Situation | Mechanismus |
|---|---|
| Pflichtparameter fehlt | `CliRouter` gibt Hilfetext + Exit-Code 1 aus |
| `--source` nicht vorhanden | `TestCommand` prüft vor Ausführung, Exit-Code 2 |
| `--source` ohne `.cs`-Dateien | `TestCommand` prüft vor Ausführung, Exit-Code 2 |
| I/O-Fehler beim Schreiben | Exception wird abgefangen, Meldung auf `stderr`, Exit-Code 3 |
| Unbekanntes Subkommando | `CliRouter` gibt Hilfetext + Exit-Code 1 aus |

---

## 8. Exit-Codes

| Code | Bedeutung |
|---|---|
| 0 | Erfolgreich |
| 1 | CLI-Fehler (falsche Argumente, unbekanntes Subkommando) |
| 2 | Eingabe-Fehler (Verzeichnis nicht gefunden, keine Quelldateien) |
| 3 | I/O-Fehler beim Lesen/Schreiben |
