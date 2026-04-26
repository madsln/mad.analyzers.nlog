# ADR-013 — Mehrere Diagnosen pro Demo-Datei

**Status:** Angenommen  
**Datum:** 2026-04-25  
**Kontext:** Einführung von MAD2255 erzwingt eine Entscheidung über die Handhabung koinzidierender Diagnosen

---

## Kontext

Mit der Einführung von MAD2255 (_Exception should be passed as the first argument_) können auf einem einzigen NLog-Logging-Aufruf **gleichzeitig mehrere Diagnosen** ausgelöst werden. Konkretes Beispiel:

```csharp
// ILoggerExtensions mit Exception-first-Overload, aber 2 Argumente statt 1:
_logger.ConditionalTrace(exception, culture, "Message with {Placeholder1}", intArg, boolArg);
```

→ Auslöser: **MAD2017** (Parameteranzahl-Mismatch) + **kein MAD2255** (Exception korrekt vorne)

```csharp
// Kein Exception-first-Overload, Exception als letztes Argument:
_logger.ConditionalTrace("Done: {Value}", someValue, ex);
```

→ Auslöser: **MAD2255** (Exception an falscher Stelle) + **MAD2017** (Parameteranzahl-Mismatch, da 2 Args aber 1 Placeholder)

Das bisherige TestDataGen-Modell geht davon aus, dass eine Demo-Datei **exakt eine** Diagnose produziert (ein Metadata-Header → ein `{|#0:...|}`-Marker → eine `expected0`-Assertion). Dieses Modell bricht sobald zwei Diagnosen koexistieren.

---

## Betrachtete Optionen

### Option A — MAD2017 unterdrücken, wenn MAD2255 feuert _(verworfener Ansatz)_

Die `AnalyzeFormatArgument`-Methode erhält ein `suppressCountMismatch`-Flag und überspringt die MAD2017-Prüfung, sobald eine Exception-Fehlverwendung erkannt wurde.

**Vorteile:**
- Keine Änderung an TestDataGen nötig.

**Nachteile:**
- Beide Diagnosen sind für den Entwickler wertvoll und unabhängig voneinander korrigierbar.
- Unterdrückung einer richtigen Diagnose ist semantisch inkorrekt.
- Verletzt das Prinzip der minimalen Überraschung: Der Nutzer sieht MAD2017 nicht, obwohl der Parametercount tatsächlich falsch ist.
- Erhöht die Kopplung zwischen zwei logisch unabhängigen Analysen.

→ **Verworfen.**

---

### Option B — Mehrere Diagnosen pro Demo-Datei zulassen _(empfohlener Ansatz)_

Der Metadaten-Header einer Demo-Datei kann **mehrere Kommentarzeilen** enthalten, eine pro Diagnose:

```csharp
// MAD2255=200:2
// MAD2017=195:29;int=1;int=2
using NLog;
...
```

TestDataGen verarbeitet alle Header-Zeilen und erzeugt für jede einen eigenen `expectedN`-Eintrag sowie einen `{|#N:...|}`-Marker. Die generierte Testmethode übergibt alle `expected`-Objekte an `VerifyAnalyzerAsync`.

**Vorteile:**
- Beide Diagnosen sind vollständig sichtbar und testbar.
- Kein künstliches Unterdrücken in der Analyzer-Implementierung.
- Skaliert auf beliebige weitere Diagnose-Kombinationen ohne Anpassung der Analyzer-Logik.
- Demo-Dateien dokumentieren das tatsächliche Verhalten transparent.

**Nachteile:**
- Änderungen in TestDataGen erforderlich (Pipeline-Klassen + Testmethoden-Generator).

→ **Empfohlen.**

---

## Entscheidung

**Option B** wird umgesetzt.

---

## Umsetzungsdetails

### 1. Demo-Datei-Format (keine Breaking Change)

Das bestehende Format für eine einzelne Diagnose bleibt unverändert kompatibel. Mehrere Diagnosen werden durch zusätzliche Headerzeilen ausgedrückt:

```
// {DiagId}={line}:{column}:{Länge}[;{Typ}={Wert}]*
// {DiagId}={line}:{column}:{Länge}[;{Typ}={Wert}]*   ← zweite Diagnose, optional
```

Reihenfolge der Headerzeilen = Reihenfolge der `#N`-Marker (nullbasiert).

**Beispiel:**

```csharp
// MAD2255=9:5:2
// MAD2017=7:14:29;int=1;int=2
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2255.logger;

public class Logger_Log_ParamCountMismatchExceptionPassedAsParam
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic()
    {
        var ex = new InvalidOperationException("operation failed");
        int someValue = 42;
        _logger.Info("Done. Exception was: {Ex}", someValue, ex);
    }
}
```

### 2. Änderungen in TestDataGen

#### `SourceCleaner.SplitHeaderAndBody`

Bereits heute gibt diese Methode alle führenden Kommentarzeilen als `Header` zurück. Keine Änderung nötig.

#### `DiagnosticMarkerInserter.ParseHeader`

Aktuell: parst **eine** Headerzeile.  
Neu: parst **alle** Headerzeilen und gibt eine geordnete Liste von `DiagnosticDescriptor`-Objekten zurück.

```csharp
// Vorher
static DiagnosticDescriptor ParseHeader(string headerComment);

// Nachher — bereits im Modell als IReadOnlyList<DiagnosticDescriptor> definiert, fertig
static IReadOnlyList<DiagnosticDescriptor> ParseHeader(string headerComment);
```

Die Methode iteriert über jede Zeile des Headerblocks. Zeilen, die nicht dem `DiagId=...`-Muster entsprechen, werden übersprungen.

#### `DiagnosticMarkerInserter.InsertMarkers`

Bereits als Listenverarbeitung spezifiziert, iteriert über `IReadOnlyList<DiagnosticDescriptor>`. Keine strukturelle Änderung nötig, lediglich sicherstellen, dass **alle** Einträge korrekt mit steigendem `#N` eingesetzt werden.

Da mehrere Marker in denselben Quelltext eingefügt werden, müssen die **Offsets rückwärts** (vom Ende nach vorne) verarbeitet werden, um keine Verschiebungen durch bereits eingefügte Marker zu erzeugen.

#### `TestMethodBuilder.BuildDiagnostics`

Aktuell: erzeugt genau `expected0` und übergibt es an `VerifyAnalyzerAsync`.  
Neu: erzeugt `expected0 … expectedN` für alle Diagnosen und übergibt sie als variadic-Argumente:

```csharp
// Vorher
var expected0 = VerifyCS.Diagnostic("MAD2017").WithLocation(0).WithArguments(1, 2);
await VerifyCS.VerifyAnalyzerAsync(test, expected0);

// Nachher (2 Diagnosen)
var expected0 = VerifyCS.Diagnostic("MAD2255").WithLocation(0);
var expected1 = VerifyCS.Diagnostic("MAD2017").WithLocation(1).WithArguments(1, 2);
await VerifyCS.VerifyAnalyzerAsync(test, expected0, expected1);
```

### 3. Änderungen in der Analyzer-Implementierung

- Das `suppressCountMismatch`-Flag in `AnalyzeFormatArgument` wird **entfernt**.
- `hasMisplacedException` out-Parameter in `AnalyzeForMisplacedExceptions` wird **entfernt**.
- MAD2017 und MAD2255 laufen wieder vollständig **unabhängig**.

### 4. Betroffene Demo-Dateien

Demo-Dateien, bei denen sowohl MAD2255 als auch MAD2017 ausgelöst wird (Case 4: `ParamCountMismatchExceptionPassedAsParam`), erhalten einen zweiten Headerkommentar:

| Datei | Header bisher | Header nach ADR |
|---|---|---|
| `Logger_Log_ParamCountMismatchExceptionPassedAsParam.cs` | `// MAD2255=…` | `// MAD2255=…` + `// MAD2017=…` |
| `ILogger_Log_ParamCountMismatchExceptionPassedAsParam.cs` | `// MAD2255=…` | `// MAD2255=…` + `// MAD2017=…` |
| `ILoggerExtensions_Log_ParamCountMismatchExceptionPassedAsParam.cs` | `// MAD2255=…` | `// MAD2255=…` + `// MAD2017=…` |

Die anderen MAD2255-Cases (1, 2, 3) lösen kein MAD2017 aus (Parameteranzahl stimmt überein) und benötigen keinen zweiten Header.

---

## Konsequenzen

- TestDataGen wird um Multi-Diagnosen-Unterstützung erweitert.
- Die Analyzer-Implementierung wird vereinfacht (keine Suppression-Logik).
- Demo-Dateien sind nun auch für Mehrfach-Diagnose-Szenarien die Single Source of Truth.
- Das Format bleibt rückwärtskompatibel (bestehende Einzel-Header-Dateien funktionieren unverändert).
