# Testdaten Generierung

Testdatengenerierung

## MAD2017

Hier wird die Generierung für den Analyzer-Code MAD2017 beschrieben.

- Testdaten beschränken sich auf relevante NLog ILogger, Logger und ILoggerExtensions Methoden und deren Überladungen
  - Trace, Debug, Info, Warn, Error, Fatal, Log
  - Message als String Argument
- Testdaten liegen in einem Demo-Projekt vor, so dass der Analyzer live in der IDE getestet werden können
  - Demo Projekt: `src/demo/mad.analyzers.nlog.demo`
- Testdaten aus Demo Projekt sind Datengrundlage für Unittests
- Unittests entsprechen den Testdaten aus dem Demo-Projekt, bzw. werden daraus erzeugt
  - Metadaten im Dateiheader der Demo Projekte liefert die Informationen die für Asserts notwendig sind
    - Anzahl der gefundenen Analyzer Meldungen, deren Position, Text und Argumente

### Ist-Stand

- Testdaten werden generiert durch python Skript unter `scripts/gen_MAD2017_demo_files.py`
- Unittests werden durch Skript `src/demo/mad.analyzers.nlog.common.demo/CompileTestData.ps1` erzeugt

### Spezifikation Funktionalität zum einheitlichen Generieren der Testdaten

---

## Skript 1: Demo-Datei-Generator (`gen_MAD2017_demo_files.py`)

### Zweck

Erzeugt C#-Quelldateien (Demo-Klassen), die als Eingabedaten für den Analyzer-Test dienen. Jede Datei enthält eine Klasse mit einer Methode, die genau einen NLog-Logging-Aufruf mit einer bestimmten Kombination aus Message-Template und Argumenten enthält.

### Eingaben

- **Ausgabeverzeichnis** (Pflichtparameter): Pfad, unter dem die generierten Dateien abgelegt werden.

### Konfigurationsdaten (eingebettet)

| Konfiguration | Beschreibung |
|---|---|
| **Subjects Under Test** | Liste der zu testenden Logger-Typen: `Logger`, `ILogger`, `ILoggerExtensions` |
| **Logger-Deklarationen** | Für jeden Logger-Typ: die Felddeklaration im generierten Code |
| **Call-Templates** | Für jeden Logger-Typ: Liste von Aufruf-Mustern mit dem Platzhalter `{MESSAGE_AND_ARGS}`. Deckt Aufrufe mit und ohne Exception-Parameter ab. `ILoggerExtensions` enthält zusätzlich einen `CultureInfo`-Parameter. |
| **Testfälle** | Benannte Paare aus (Message-Template-String, Argument-Liste). Die Message-Templates enthalten 0–6 strukturierte Platzhalter `{PlaceholderN}`, die Argumentlisten 0–7 typisierte Argumente. |
| **Argument-Deklarationen** | Für jeden Argument-Namen eine lokale Variablendeklaration im generierten Code |

### Verarbeitungslogik

#### Für jeden Logger-Typ × Call-Template × Testfall werden bis zu 2 Varianten erzeugt:

1. **Direkte Argumente**: Argumente werden direkt im Logging-Aufruf übergeben.
2. **Object-Array**: Argumente werden in einem `object[]`-Array gesammelt und als Array übergeben.

> **Hinweis (ADR-011):** Die `ReadOnlySpan<object>`-Variante wurde entfernt, da das explizite Pattern in NLog-Produktionscode nicht vorkommt und der transparente C# 13 `params ReadOnlySpan<T>`-Compilerpfad für Roslyn unsichtbar ist.

#### Für jede Variante wird ein `DemoInfo`-Datensatz erstellt mit:

- Titel (Klassenname, deterministisch aus Logger-Typ, Testfall-Name, Exception-Flag, Variante zusammengesetzt)
- Logger-Felddeklaration
- Methodenrumpf-Zeilen (Variablendeklarationen + Logging-Aufruf)
- Anzahl der Platzhalter im Message-Template
- Anzahl der übergebenen Argumente
- Benötigte `using`-Direktiven (immer `using NLog;`, bedingt `using System.Globalization;`)

#### Klassenname-Konvention

`{LoggerType}_Log_[WithException_]{PascalCaseTestcaseName}[_ObjectArray]`

Beispiel: `ILogger_Log_WithException_1PlaceholderWith2Args_ObjectArray`

#### Datei-Header (Metadaten für den Unittest-Compiler)

Jede erzeugte Datei beginnt mit einem Kommentarheader in folgendem Format:

```
// MAD2017={line}:{column}:{Länge};int={AnzahlPlatzhalter};int={AnzahlArgumente}
```

- `line`: 1-basierte Zeilennummer der Zeile, in der der Message-String beginnt.
- `column`: 1-basierter Spaltenindex des ersten Zeichens des Message-Strings.
- `Länge`: Länge des Message-Strings in Zeichen.
- `AnzahlPlatzhalter`: Anzahl der `{PlaceholderN}`-Ausdrücke im Message-Template.
- `AnzahlArgumente`: Anzahl der übergebenen Argumente.

#### Dateistruktur des generierten Codes

```
{using-Direktiven}

namespace mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.{loggertype};

public class {Klassenname}
{
    private readonly {LoggerTyp} _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        // Variablendeklarationen
        // Logging-Aufruf
    }
}
```

### Ausgabe

- Verzeichnisstruktur: `{OutDir}/diagnostics/MAD2017/{loggertype}/{Klassenname}.cs`
- Unterverzeichnisse werden automatisch erstellt.
- Eine Datei pro Variante.

---

## Skript 2: Unittest-Compiler (`CompileTestData.ps1`)

### Zweck

Liest die vom Demo-Generator erzeugten C#-Quelldateien und transformiert sie in C#-Unittest-Dateien (MSTest + Roslyn Analyzer Verifier). Die generierten Unittest-Klassen sind parameterisiert über alle NLog-Log-Level.

### Parameter

| Parameter | Beschreibung | Standard |
|---|---|---|
| `SourceDirectory` | Verzeichnis mit den Demo-Quelldateien | Aktuelles Verzeichnis |
| `OutDirectory` | Verzeichnis für die generierten Unittest-Dateien | `../../test/.../generated` |

### Eingabe-Verzeichnisstruktur

Das `SourceDirectory` muss folgende Unterverzeichnisse enthalten:

```
{SourceDirectory}/
  usings/          ← .cs-Dateien, die Typdefinitionen/Preambles enthalten (Dateiname = Symbolname)
  diagnostics/     ← .cs-Demo-Dateien, bei denen ein Diagnostic erwartet wird
  nodiag/          ← .cs-Demo-Dateien, bei denen kein Diagnostic erwartet wird
```

### Ausgabe-Verzeichnisstruktur

```
{OutDirectory}/
  diagnostics/     ← Generierte Testklassen für Fälle mit erwartetem Diagnostic
  nodiag/          ← Generierte Testklassen für Fälle ohne erwartetes Diagnostic
```

### Verarbeitungslogik

#### Schritt 1: Usings-Tabelle laden

- Alle `.cs`-Dateien aus `{SourceDirectory}/usings/` werden gelesen.
- Dateiinhalt wird bereinigt (Kommentarzeilen und Leerzeilen entfernt).
- Eine Tabelle `Symbolname → bereinigter Inhalt` wird aufgebaut (Dateiname ohne Erweiterung = Symbolname).

#### Schritt 2: Header und Body trennen (angewendet auf alle Demo-Dateien)

- Die Kommentarzeilen am Dateianfang (beginnen mit `//`) bilden den **Header**.
- Der Rest der Datei bildet den **Body**.

#### Schritt 3 (nur `diagnostics`): Metadaten-Header parsen und Diagnostic-Marker einfügen

**Header-Format:**

Der Datei-Header wird nach folgendem Schema geparst (ADR-012):

```
// {DiagnosticId}={line}:{column}:{Länge};{TypA}={WertA};{TypB}={WertB};...
```

Jede Kommentarzeile ergibt ein Diagnostic-Objekt mit:
- `Id`: Diagnostic-Bezeichner (z. B. `MAD2017`)
- `line`: 1-basierte Zeilennummer des betroffenen Ausdrucks im Body
- `column`: 1-basierter Spaltenindex des ersten Zeichens des Ausdrucks
- `Länge`: Länge des betroffenen Ausdrucks in Zeichen
- `Parameters`: Liste typisierter Argumente (`int` → Integer, sonst String)

**Marker-Einfügung:**

An den per Metadaten definierten Positionen wird der betroffene Textbereich in folgendes Marker-Format eingebettet:

```
{|#N:OriginalText|}
```

Dabei ist `N` der Positions-Index des Diagnostics (mehrere Diagnostics an gleicher Position teilen denselben Index).

#### Schritt 4: Using-Direktiven extrahieren (ADR-010)

- Alle `using`-Direktiven werden aus dem Body extrahiert und separat gespeichert.
- Duplikate werden entfernt.
- Der verbleibende Body enthält keine `using`-Zeilen mehr.

#### Schritt 5: Quelldatei-Bereinigung

1. Kommentarzeilen (beginnend mit `//`) entfernen.
2. Leerzeilen entfernen.

#### Schritt 6: Daten-getriebene Token-Ersetzung

- Der Log-Level-Bezeichner (z. B. `Info`) wird durch den Platzhalter `%LOGLEVEL%` ersetzt.
- Dadurch werden alle Log-Level-Varianten durch eine einzige parameterisierte Testmethode abgedeckt.

#### Schritt 7: Preamble-Erkennung und -Einfügung

- Im bereinigten Code wird nach Symbolnamen gesucht, die in der Usings-Tabelle vorhanden sind.
- Gefundene Preamble-Blöcke werden dem Test-Code vorangestellt.

#### Schritt 8: Code-Sample-Wrapping

Der bereinigte Test-Code wird in ein Standard-C#-Grundgerüst eingebettet. Die `using`-Direktiven aus der Demo-Datei werden dabei zusammengeführt:

```csharp
using System;
using NLog;
{weitere using-Direktiven aus Demo-Datei}

namespace analyzer.test;

// bereinigter, eingerückter Test-Code
```

#### Schritt 9: Escaping für C#-Verbatim-Strings

Da der Test-Code als C#-Verbatim-String (`@"..."`) in die Testmethode eingebettet wird, werden alle `"`-Zeichen im Code durch `""` ersetzt.

#### Schritt 10: Testmethoden-Generierung

**Für `nodiag`-Dateien** → `GenerateNoDiagnosticTestMethod`:

```csharp
[DataTestMethod]
[DataRow("Trace")] [DataRow("Debug")] [DataRow("Info")]
[DataRow("Warn")]  [DataRow("Error")] [DataRow("Fatal")]
public async Task {Methodenname}(string logLevel)
{
    var template = @"
        {TestCode}
    ";
    var test = template.Replace("%LOGLEVEL%", logLevel);
    await VerifyCS.VerifyAnalyzerAsync(test);
}
```

**Für `diagnostics`-Dateien** → `BuildDiagnostics`:

Wie oben, jedoch mit zusätzlichen `expected`-Variablen und deren Übergabe an `VerifyAnalyzerAsync`:

```csharp
[TestMethod]
[DataRow("Trace")] ...
public async Task {Methodenname}(string logLevel)
{
    ...
    var expected0 = VerifyCS.Diagnostic("{DiagId}").WithLocation(0).WithArguments({Param1}, {Param2});
    await VerifyCS.VerifyAnalyzerAsync(test, expected0);
}
```

#### Schritt 11: Testklassen-Bündelung

- Alle Testmethoden aus Dateien im selben Quell-Unterverzeichnis werden zu einer Testklasse zusammengefasst.
- Klassenname: Relativer Pfad des Unterverzeichnisses (Pfadtrenner durch `_` ersetzt) + `_Tests`.
- Klassen-Template:

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = mad.analyzers.nlog.Test.Verifiers.CSharpCodeFixVerifier<
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchAnalyzer,
    mad.analyzers.nlog.LogMessageTemplateParameterCountMismatchProvider>;

namespace mad.analyzers.nlog.Test.generated;

[TestClass]
public class {Klassenname}
{
    // Testmethoden
}
```

#### Schritt 12: Ausgabe

- Eine `.cs`-Datei pro Testklasse, UTF-8-kodiert.
- Ausgabepfad: `{OutDirectory}/diagnostics/{Klassenname}.cs` bzw. `.../nodiag/{Klassenname}.cs`.
- Fehlende Verzeichnisse werden automatisch erstellt.