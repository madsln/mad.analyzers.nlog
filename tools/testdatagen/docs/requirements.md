# Anforderungsdokument: TestDataGen CLI

## Übersicht

`TestDataGen` ist ein C#-Kommandozeilenprogramm, das Testdaten für den NLog-Analyzer **MAD2017** generiert. Es erzeugt C#-Demo-Quelldateien sowie daraus abgeleitete MSTest-Unittest-Dateien.

---

## 1. Allgemeine Anforderungen

| ID | Anforderung |
|---|---|
| REQ-01 | Das Programm ist eine ausführbare .NET-Konsolenanwendung (`TestDataGen.exe`). |
| REQ-02 | Das Programm wird über Kommandozeilenargumente gesteuert. |
| REQ-03 | Das Programm gibt bei Fehlern eine verständliche Fehlermeldung auf `stderr` aus und beendet sich mit einem Fehler-Exit-Code (≠ 0). |
| REQ-04 | Das Programm gibt bei erfolgreichem Abschluss eine Zusammenfassung der generierten Dateien auf `stdout` aus. |

---

## 2. Kommandozeilenschnittstelle

### 2.1 Unterkommandos

Das Programm unterstützt folgende Unterkommandos:

| Unterkommando | Beschreibung |
|---|---|
| `demo` | Generiert ausschließlich die C#-Demo-Quelldateien (Datengrundlage). |
| `test` | Generiert ausschließlich die MSTest-Unittest-Dateien. Setzt voraus, dass die Demo-Quelldateien bereits vorhanden sind. |
| `all` | Führt `demo` und anschließend `test` in einem Schritt aus. |

### 2.2 Parameter

#### Unterkommando `demo`

```
TestDataGen demo --out <Pfad>
```

| Parameter | Pflicht | Beschreibung |
|---|---|---|
| `--out` | Ja | Ausgabeverzeichnis für die generierten Demo-C#-Dateien. |

#### Unterkommando `test`

```
TestDataGen test --source <Pfad> [--out <Pfad>]
```

| Parameter | Pflicht | Beschreibung |
|---|---|---|
| `--source` | Ja | Verzeichnis, das die Demo-Quelldateien enthält (Ausgabe von `demo`). |
| `--out` | Nein | Ausgabeverzeichnis für die generierten Unittest-Dateien. Standard: `../../test/mad.analyzers.nlog.Test/generated` relativ zu `--source`. |

#### Unterkommando `all`

```
TestDataGen all --demo-out <Pfad> [--test-out <Pfad>]
```

| Parameter | Pflicht | Beschreibung |
|---|---|---|
| `--demo-out` | Ja | Ausgabeverzeichnis für die Demo-Dateien. |
| `--test-out` | Nein | Ausgabeverzeichnis für die Unittest-Dateien. Standard: wie bei `test`. |

### 2.3 Fehlerverhalten

| Bedingung | Verhalten |
|---|---|
| Unterkommando `test` und `--source`-Verzeichnis existiert nicht | Fehler mit Hinweis, zuerst `demo` auszuführen. |
| Unterkommando `test` und `--source`-Verzeichnis enthält keine `.cs`-Dateien | Fehler mit Hinweis, zuerst `demo` auszuführen. |
| Pflichtparameter fehlt | Fehler mit Anzeige der Hilfe für das betreffende Unterkommando. |
| `--out`-Verzeichnis existiert nicht | Verzeichnis wird automatisch erstellt. |

---

## 3. Unterkommando `demo` – Demo-Datei-Generator

### 3.1 Zweck

Erzeugt C#-Quelldateien (Demo-Klassen), die als Eingabedaten für den Analyzer-Test dienen. Jede Datei enthält eine Klasse mit einer Methode, die genau einen NLog-Logging-Aufruf mit einer bestimmten Kombination aus Message-Template und Argumenten enthält.

### 3.2 Eingebettete Konfiguration

Die folgenden Konfigurationsdaten sind fest im Programm eingebettet (keine externe Konfigurationsdatei erforderlich):

#### Subjects Under Test (Logger-Typen)

- `Logger`
- `ILogger`
- `ILoggerExtensions`

#### Logger-Deklarationen

Für jeden Logger-Typ wird eine feste Felddeklaration im generierten Code verwendet:

| Logger-Typ | Deklaration |
|---|---|
| `Logger` | `private readonly Logger _logger = LogManager.GetCurrentClassLogger();` |
| `ILogger` | `private readonly ILogger _logger = LogManager.GetCurrentClassLogger();` |
| `ILoggerExtensions` | `private readonly Logger _logger = LogManager.GetCurrentClassLogger();` |

#### Call-Templates

Für jeden Logger-Typ existiert eine Liste von Aufruf-Mustern mit dem Platzhalter `{MESSAGE_AND_ARGS}`:

- Aufrufe mit und ohne `Exception`-Parameter
- `ILoggerExtensions` enthält zusätzlich einen `CultureInfo`-Parameter

#### Testfälle

Benannte Paare aus (Message-Template-String, Argument-Liste):

- Message-Templates enthalten 0–6 strukturierte Platzhalter (`{Placeholder0}`–`{Placeholder5}`)
- Argumentlisten enthalten 0–7 typisierte Argumente

#### Argument-Deklarationen

Für jeden Argument-Namen wird eine lokale Variablendeklaration im generierten Code hinterlegt.

### 3.3 Verarbeitungslogik

#### 3.3.1 Varianten-Generierung

Für jede Kombination aus **Logger-Typ × Call-Template × Testfall** werden bis zu 3 Varianten erzeugt:

| Variante | Beschreibung | Ausschlusskriterium |
|---|---|---|
| **Direkte Argumente** | Argumente werden direkt im Logging-Aufruf übergeben. | – |
| **Object-Array** | Argumente werden in einem `object[]`-Array gesammelt und als Array übergeben. | – |

> **Hinweis (ADR-011):** Die `ReadOnlySpan<object>`-Variante wurde entfernt, da das explizite Pattern in NLog-Produktionscode nicht vorkommt.

#### 3.3.2 Klassenname-Konvention

```
{LoggerType}_Log_[WithException_]{PascalCaseTestcaseName}[_ObjectArray]
```

Beispiel: `ILogger_Log_WithException_1PlaceholderWith2Args_ObjectArray`

#### 3.3.3 Datei-Header (Metadaten)

Jede erzeugte Datei beginnt mit einem Kommentarheader:

```
// MAD2017={line}:{column}:{Länge};int={AnzahlPlatzhalter};int={AnzahlArgumente}
```

| Feld | Beschreibung |
|---|---|
| `line` | 1-basierte Zeilennummer der Zeile, in der der Message-String beginnt. |
| `column` | 1-basierter Spaltenindex des ersten Zeichens des Message-Strings. |
| `Länge` | Länge des Message-Strings in Zeichen. |
| `AnzahlPlatzhalter` | Anzahl der `{PlaceholderN}`-Ausdrücke im Message-Template. |
| `AnzahlArgumente` | Anzahl der übergebenen Argumente. |

#### 3.3.4 Dateistruktur des generierten Codes

```csharp
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

- `using NLog;` ist immer enthalten.
- `using System.Globalization;` wird bedingt eingefügt (nur bei `ILoggerExtensions` mit `CultureInfo`).

### 3.4 Ausgabe

- Verzeichnisstruktur: `{OutDir}/diagnostics/MAD2017/{loggertype}/{Klassenname}.cs`
- Unterverzeichnisse werden automatisch erstellt.
- Eine Datei pro Variante.

---

## 4. Unterkommando `test` – Unittest-Compiler

### 4.1 Zweck

Liest die vom Demo-Generator erzeugten C#-Quelldateien und transformiert sie in C#-Unittest-Dateien (MSTest + Roslyn Analyzer Verifier). Die generierten Unittest-Klassen sind über alle NLog-Log-Level parameterisiert.

### 4.2 Eingabe-Verzeichnisstruktur

Das `--source`-Verzeichnis muss folgende Unterverzeichnisse enthalten:

```
{source}/
  usings/       ← .cs-Dateien mit Typdefinitionen/Preambles (Dateiname = Symbolname)
  diagnostics/   ← Demo-Dateien, bei denen ein Diagnostic erwartet wird
  nodiag/       ← Demo-Dateien, bei denen kein Diagnostic erwartet wird
```

### 4.3 Ausgabe-Verzeichnisstruktur

```
{out}/
  diagnostics/   ← Generierte Testklassen für Fälle mit erwartetem Diagnostic
  nodiag/       ← Generierte Testklassen für Fälle ohne erwartetes Diagnostic
```

### 4.4 Verarbeitungslogik

#### Schritt 1: Usings-Tabelle laden

- Alle `.cs`-Dateien aus `{source}/usings/` werden gelesen.
- Kommentarzeilen und Leerzeilen werden entfernt.
- Es wird eine Tabelle `Symbolname → bereinigter Inhalt` aufgebaut (Dateiname ohne Erweiterung = Symbolname).

#### Schritt 2: Header und Body trennen

- Die Kommentarzeilen am Dateianfang (beginnen mit `//`) bilden den **Header**.
- Der Rest der Datei bildet den **Body**.

#### Schritt 3 (nur `diagnostics`): Metadaten-Header parsen und Diagnostic-Marker einfügen

**Header-Format:**

```
// {DiagnosticId}={line}:{column}:{Länge};{TypA}={WertA};{TypB}={WertB};...
```

Jede Kommentarzeile ergibt ein Diagnostic-Objekt mit:

| Feld | Beschreibung |
|---|---|
| `Id` | Diagnostic-Bezeichner (z. B. `MAD2017`) |
| `line` | 1-basierte Zeilennummer des betroffenen Ausdrucks im Body |
| `column` | 1-basierter Spaltenindex des ersten Zeichens des Ausdrucks |
| `Länge` | Länge des betroffenen Ausdrucks in Zeichen |
| `Parameters` | Liste typisierter Argumente (`int` → Integer, sonst String) |

**Marker-Einfügung:**

An den per Metadaten definierten Positionen wird der betroffene Textbereich eingebettet:

```
{|#N:OriginalText|}
```

`N` ist der Positions-Index des Diagnostics.

#### Schritt 4: Using-Direktiven extrahieren (ADR-010)

- Alle `using`-Direktiven werden aus dem Body extrahiert und separat gespeichert.
- Duplikate werden entfernt.
- Der verbleibende Body enthält keine `using`-Zeilen mehr.

#### Schritt 5: Quelldatei-Bereinigung

1. Kommentarzeilen (beginnend mit `//`) entfernen.
2. Leerzeilen entfernen.

#### Schritt 6: Log-Level-Parametrisierung

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

Da der Test-Code als Verbatim-String (`@"..."`) in die Testmethode eingebettet wird, werden alle `"`-Zeichen im Code durch `""` ersetzt.

#### Schritt 10: Testmethoden-Generierung

**Für `nodiag`-Dateien:**

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

**Für `diagnostics`-Dateien:**

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
    var expected0 = VerifyCS.Diagnostic("{DiagId}").WithLocation(0).WithArguments({Param1}, {Param2});
    await VerifyCS.VerifyAnalyzerAsync(test, expected0);
}
```

#### Schritt 11: Testklassen-Bündelung

- Alle Testmethoden aus Dateien im selben Quell-Unterverzeichnis werden zu einer Testklasse zusammengefasst.
- Klassenname: Relativer Pfad des Unterverzeichnisses (Pfadtrenner durch `_` ersetzt) + `_Tests`.

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
- Ausgabepfad: `{out}/diagnostics/{Klassenname}.cs` bzw. `{out}/nodiag/{Klassenname}.cs`.
- Fehlende Verzeichnisse werden automatisch erstellt.

---

## 5. Abhängigkeiten zwischen den Unterkommandos

```
demo  ──►  test
           (--source muss auf Ausgabe von demo zeigen)

all   ≡  demo + test (intern verkettet, kein manueller Zwischenschritt)
```

| Szenario | Voraussetzung |
|---|---|
| Nur `demo` ausführen | Keine |
| Nur `test` ausführen | Demo-Dateien müssen unter `--source` bereits vorhanden sein |
| `all` ausführen | Keine (Demo wird automatisch vor Test ausgeführt) |

---

## 6. Nicht-funktionale Anforderungen

| ID | Anforderung |
|---|---|
| NFR-01 | Das Programm soll als eigenständige .NET-Konsolenanwendung (Single-File-Executable oder standard `dotnet run`) ausgeführt werden können. |
| NFR-02 | Alle generierten Dateien werden UTF-8-kodiert geschrieben. |
| NFR-03 | Bestehende Ausgabedateien werden ohne Nachfrage überschrieben. |
| NFR-04 | Das Programm gibt bei `--help` bzw. fehlendem Unterkommando eine strukturierte Hilfe aus. |
| NFR-05 | Die eingebetteten Konfigurationsdaten (Logger-Typen, Testfälle, Call-Templates) sind im Quellcode klar von der Verarbeitungslogik getrennt (z. B. als separate Klasse oder Datei). |
