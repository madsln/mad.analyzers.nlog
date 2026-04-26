# ADR-009: Deduplizierung von `using`-Direktiven in `CodeSampleWrapper`

**Status:** Accepted  
**Datum:** 2026-04-25  
**Kontext:** TestDataGen – Test-Pipeline  
**Folgt auf:** ADR-008

---

## Kontext

`CodeSampleWrapper.Wrap()` baut den Preamble-Block, der jedem generierten Test-Template
vorangestellt wird:

```csharp
"using System;\n" +
"using NLog;\n" +
"\n" +
"namespace analyzer.test;\n" +
"\n" +
strippedSource
```

Die Demo-Quelldateien enthalten selbst ebenfalls `using`-Direktiven (z. B. `using NLog;`),
damit sie eigenständig kompilierbar sind. `Wrap()` hat deren Inhalt bislang unverändert
übernommen, sodass in den generierten Test-Templates dieselben `using`-Direktiven doppelt
auftraten:

```csharp
// Preamble (von Wrap() erzeugt)
using System;
using NLog;

namespace analyzer.test;

// Inhalt aus Demo-Datei – NLog bereits oben vorhanden
using NLog;         // ← Duplikat
public class ...
```

Doppelte `using`-Direktiven sind zwar für den Roslyn-Analyzer-Test-Compiler kein harter
Fehler, erzeugen jedoch CS0105-Warnungen, sind irreführend und verstoßen gegen den
Grundsatz, dass generierter Code aufgeräumt sein soll.

---

## Entscheidung

`CodeSampleWrapper.Wrap()` bereinigt den Quelltext **vor** dem Zusammensetzen: Alle
`using`-Direktiven, die bereits im fest codierten Preamble enthalten sind, werden aus dem
Demo-Quelltext entfernt.

### Implementierung

```csharp
var preambleUsings = new HashSet<string>(StringComparer.Ordinal)
{
    "using System;",
    "using NLog;",
};

var deduplicatedLines = strippedSource
    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
    .Where(l => !preambleUsings.Contains(l.Trim()));
var deduplicatedSource = string.Join("\n", deduplicatedLines);
```

Der Vergleich erfolgt nach dem `Trim()` der Zeile, um führende Leerzeichen (z. B. durch
Einrückung) tolerant zu behandeln.

---

## Konsequenzen

### Positiv
- Generierter Test-Code enthält keine doppelten `using`-Direktiven mehr.
- Demo-Quelldateien bleiben eigenständig kompilierbar (ihre `using`-Direktiven werden
  nicht aus dem Quelltext entfernt, sondern nur im zusammengesetzten Template
  unterdrückt).
- Die Menge der Preamble-Usings ist an einer einzigen Stelle (`preambleUsings`) definiert
  und leicht erweiterbar.

### Negativ / Risiken
- Wird der fest codierte Preamble künftig um weitere `using`-Direktiven erweitert, muss
  `preambleUsings` synchron gepflegt werden. Wird dies vergessen, entstehen erneut
  Duplikate.
- `using`-Aliase (z. B. `using X = Foo.Bar;`) oder `global using`-Direktiven werden von
  der aktuellen Implementierung nicht dedupliziert; bei Bedarf ist die Logik zu erweitern.
