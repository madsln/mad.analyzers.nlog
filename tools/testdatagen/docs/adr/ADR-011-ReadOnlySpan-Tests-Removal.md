# ADR-011: ReadOnlySpan-Testvarianten aus der Testgeneration entfernen

**Status:** Accepted  
**Datum:** 2026-04-25  
**Kontext:** Analyzer-Tests – MAD2017-Testvarianten  
**Betrifft:** `diagnostics_MAD2017_logger_Tests`, `diagnostics_MAD2017_ilogger_Tests`, TestDataGen-Templates

---

## Kontext

Im Rahmen der automatisch generierten Unittests für den `LogMessageTemplateParameterCountMismatchAnalyzer`
(Diagnose MAD2017) existieren Testvarianten, die explizit eine lokale `ReadOnlySpan<object>`-Variable
konstruieren und an NLog-Log-Methoden übergeben:

```csharp
var objectSpan = new ReadOnlySpan<object>( new object[] { intArg, boolArg } );
_logger.Trace({|#0:"Message with {Placeholder1}"|}, objectSpan);
```

Diese Tests schlagen **alle** fehl. Anlass dieser ADR ist die Frage, ob die Tests
überhaupt notwendig sind – insbesondere auf Basis der These, dass `params ReadOnlySpan<T>`
lediglich eine Compiler-Optimierung sei und für Roslyn-Analyzer keine Rolle spiele.

---

## Analyse

### 1. Existenz der ReadOnlySpan-Überladung in NLog

Durch Reflexion des eingebundenen `NLog.dll` (Version 6.1.1) wurde bestätigt, dass NLog 6.x
eine **explizite** Überladung definiert:

```
Trace(string message, System.ReadOnlySpan[System.Object] args)
```

Es handelt sich **nicht** ausschließlich um eine unsichtbare Compiler-Indirektion.
Die Methode ist ein echter öffentlicher API-Einstiegspunkt.

### 2. Transparenz von `params ReadOnlySpan<T>` gegenüber Roslyn

Das C# 13-Feature `params ReadOnlySpan<T>` ist **für Roslyn-Analyzer vollständig transparent**.
Wenn ein Entwickler schreibt:

```csharp
_logger.Trace("Message with {Placeholder1}", intArg, boolArg);
```

wählt der Compiler zur Laufzeit ggf. die `ReadOnlySpan<object>`-Überladung –
im Roslyn-Semantikmodell sieht der Analyzer aber weiterhin **einzelne `IArgumentOperation`-Knoten**
für `intArg` und `boolArg`, unabhängig davon, welche Überladung der Compiler intern auflöst.
Die Überladungsauflösung ist für den statischen Analysebaum ohne Belang.

### 3. Das tatsächliche Fehlerbild

Der Analyser zählt die Argumente für `ReadOnlySpan` falsch:

| Testerwartet | Tatsächliches Ergebnis |
|---|---|
| `Expected 0 but got 5` (5 Elemente im Span) | `Expected 0 but got 1` (1 Konstruktor-Argument) |

Ursache: In `CountParamsElements` fällt die `IObjectCreationOperation`-Verzweigung
für `new ReadOnlySpan<object>(new object[] { ... })` auf den Fallback `obj.Arguments.Count()`.
Dieser gibt `1` zurück – die Array-Instanz als einzelnes Konstruktor-Argument –
statt der enthaltenen Elemente.

Technisch lösbar wäre dies durch eine zusätzliche Heuristik:
„Wenn das einzige Konstruktor-Argument eine `IArrayCreationOperation` ist, zähle deren Elemente."
Das wäre jedoch aufwändig, fehleranfällig bei verschachtelten Formen und hätte kaum Praxisnutzen.

### 4. Praxisrelevanz

Kein NLog-Entwickler schreibt in produktivem Code:

```csharp
var objectSpan = new ReadOnlySpan<object>(new object[] { a, b });
_logger.Trace("Message with {A}", objectSpan);
```

Wenn ein Entwickler `ReadOnlySpan<object>` *explizit* übergibt, handelt es sich um einen
äußerst ungewöhnlichen Stil. Der reguläre, von C# 13 intern genutzte Pfad –
der Compiler erzeugt intern einen `ReadOnlySpan<object>`-Aufruf aus `_logger.Trace("msg", a, b)` –
ist für den Analyzer, wie unter Punkt 2 erläutert, **unsichtbar**.

### 5. Bewertung der These

> „`params ReadOnlySpan<T>` ist nur eine Compiler-Optimierung und spielt in der IDE für den
> Analyzer vermutlich keine Rolle."

Die These ist **im Wesentlichen korrekt**, mit einer Präzisierung:

- Die Aussage gilt für den **transparenten** C# 13-Aufrufpfad (`_logger.Trace("msg", a, b)` → intern ReadOnlySpan).
- Die `ReadOnlySpan<object>`-Überladung existiert zwar explizit in der NLog-API,
  aber kein Entwickler übergibt jemals einen manuell konstruierten `ReadOnlySpan<object>`.
- Der Analyzer erhält für den transparenten Pfad dieselbe Operation-Repräsentation wie
  für den `object[]`-Pfad – die bestehenden `params object[]`-Tests decken diesen Fall ab.

---

## Entscheidung

Die `ReadOnlySpan`-Testvarianten werden aus der Testgeneration **entfernt**.

### Begründung

1. **Kein Mehrwert:** Der transparente `params ReadOnlySpan<T>`-Pfad (C# 13) ist für Roslyn unsichtbar
   und bereits durch die bestehenden Direktargument-Tests (`ShouldRaiseDiagnostic_MAD2017` ohne Suffix)
   abgedeckt.

2. **Unrealistisches Muster:** Explizit konstruierte `ReadOnlySpan<object>`-Variablen
   kommen in NLog-Produktivcode nicht vor. Ein Test, der ein Muster verifiziert,
   das kein Entwickler jemals schreibt, erhöht nicht das Vertrauen in den Analyzer.

3. **Unverhältnismäßiger Fixaufwand:** Das korrekte Auflösen von
   `new ReadOnlySpan<object>(new object[] { ... })` in `CountParamsElements` würde
   eine spezifische Sonderbehandlung erfordern, die fragil und schwer wartbar wäre.

4. **Keine falsch-negativen Diagnosen im Produktivbetrieb:** Da der C# 13-Compiler-Pfad
   transparent ist, erkennt der Analyzer Mismatch-Fälle bei `_logger.Trace("msg {A}", a, b)`
   korrekt – unabhängig davon, ob der Compiler intern `ReadOnlySpan` oder `object[]` verwendet.

### Abgrenzung: Was wird **nicht** entfernt

- Testvarianten, die `new object[] { ... }` (`ObjectArray`) explizit übergeben, bleiben bestehen.
  Dieses Muster ist praxisrelevant (ältere Codebasen, Migration von NLog 4.x).
- Alle Direktargument-Testvarianten (ohne Suffix, mit 2–7 typisierten Argumenten) bleiben bestehen.

---

## Umsetzung

### In TestDataGen

Innerhalb des TestDataGen-Generators werden alle Templates, die das `ReadOnlySpan`-Muster
erzeugen (`_ReadOnlySpan`-Variante), aus der Konfiguration der Call-Varianten entfernt.
Bereits generierte Testdateien (`diagnostics_MAD2017_*_Tests.cs`) werden neu generiert;
die `ReadOnlySpan`-Testmethoden fallen dabei weg.

### Sofortmaßnahme (optional, bis Neugenerierung erfolgt)

Bis zur nächsten Neugenerierung können die betroffenen Tests mit `[Ignore]` markiert werden,
um den CI-Build nicht zu blockieren. Dies ist **kein dauerhafter Zustand**.

---

## Konsequenzen

### Positiv

- Keine dauerhaft roten Tests in CI.
- Testcode reflektiert nur reale Nutzungsmuster.
- `CountParamsElements` muss nicht für einen Randfall erweitert werden.
- Die Abdeckungsaussage der Tests ist klarer: jede Testmethode entspricht einem
  Muster, das echte Entwickler tatsächlich schreiben.

### Negativ / Risiken

- Sollte NLog oder eine abgeleitete Bibliothek künftig eine API einführen, bei der
  `ReadOnlySpan<T>` **explizit** als API-Parameter dokumentiert und regelmäßig so
  verwendet wird, müsste dieses ADR revidiert werden.
- Der `ReadOnlySpan<object>`-Konstruktor-Pfad in `CountParamsElements` bleibt
  potentiell fehlerhaft. Dieses Risiko ist akzeptabel, weil der Pfad im Praxisbetrieb
  des Analyzers nie erreicht wird.

---

## Verwandte Entscheidungen

- **ADR-001** – ILoggerExtensions field type: legt den Grundsatz fest, welche NLog-Typen
  analysiert werden.
- **ADR-008** – LineEnding Metadata: orthogonale Entscheidung zur Testgenerierung.
