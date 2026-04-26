# ADR-010: `UsingExtractor` als eigenständiger Pipeline-Schritt

**Status:** Proposed  
**Datum:** 2026-04-25  
**Kontext:** TestDataGen – Test-Pipeline  
**Folgt auf:** ADR-009  
**Ersetzt:** ADR-009 (Entscheidungsabschnitt)

---

## Kontext

ADR-009 löste das Problem doppelter `using`-Direktiven im generierten Test-Template durch
eine **nachträgliche Deduplizierung** in `CodeSampleWrapper.Wrap()`: Eine fest codierte
`HashSet<string> preambleUsings` filtert beim Zusammensetzen alle Zeilen heraus, die bereits
im Preamble vorkommen.

Diese Lösung funktioniert, hat jedoch strukturelle Schwächen:

1. **Zwei Wahrheitsquellen:** Die Preamble-Usings stehen einmal als konkatenier­ter String am
   Ende von `Wrap()` und einmal als `HashSet` am Anfang – sie müssen synchron gepflegt
   werden. Wird das vergessen, entstehen erneut Duplikate.
2. **Falsche Abstraktionsebene:** `Wrap()` hat die Aufgabe, einen Preamble-Block
   *zusammenzusetzen*, nicht Quelltexte zu *parsen*. Zeilen­filterung gegen eine
   Whitelist ist konzeptuell Parsing und gehört woanders hin.
3. **Brüchige Zeilenvergleiche:** Der Vergleich `preambleUsings.Contains(l.Trim())`
   erkennt nur exakte Direktiven. `using`-Aliase (`using X = Foo.Bar;`) oder
   `global using`-Direktiven werden nicht behandelt – das ADR dokumentiert dies selbst
   als bekannte Lücke.
4. **Falscher Zeitpunkt:** Die Demo-Datei behält ihre `using`-Direktiven durch den
   gesamten Pipeline bis kurz vor dem Ende. Alle Schritte (Cleaner, Parameterizer,
   PreambleResolver) erhalten einen Quelltext, der noch überflüssige Direktiven enthält,
   die ohnehin verworfen werden.

Ein analoges Muster existiert bereits in der Pipeline: `SourceCleaner.SplitHeaderAndBody()`
trennt den Kommentar-Header einmalig vom Body – der Header wird separat weitergegeben und
später in `DiagnosticMarkerInserter.ParseHeader()` ausgewertet. Dasselbe Prinzip
(**frühes Extrahieren, saubere Weitergabe**) sollte auch für `using`-Direktiven gelten.

---

## Entscheidung

Ein neuer Pipeline-Schritt `UsingExtractor` extrahiert die `using`-Direktiven **vor** dem
Reinigen des Body. Der Body wird ohne diese Direktiven weitergegeben; `CodeSampleWrapper.Wrap()`
erhält die extrahierten Direktiven als Parameter und fügt sie – nach Deduplizierung gegen den
fest codierten Preamble – sauber ein.

Die Whitelist `preambleUsings` in `Wrap()` entfällt. Stattdessen entscheidet ein
mengenbasierter Vergleich zwischen extrahierten Usings und dem (unveränderten) Preamble, welche
Direktiven zusätzlich aufgenommen werden.

### 1. Neuer Pipeline-Schritt `UsingExtractor`

```csharp
// Test/Pipeline/UsingExtractor.cs
internal static class UsingExtractor
{
    private static readonly Regex UsingLineRegex = new(
        @"^\s*(global\s+)?using\s+",
        RegexOptions.Compiled);

    /// <summary>
    /// Splits all leading and scattered <c>using</c> directives from
    /// <paramref name="source"/> and returns them separately.
    /// </summary>
    /// <returns>
    /// <c>Usings</c>: the extracted directive lines (trimmed, original order);
    /// <c>Body</c>: the remaining source without any <c>using</c> lines.
    /// </returns>
    public static (IReadOnlyList<string> Usings, string Body) Extract(string source)
    {
        var lines = source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var usings = new List<string>();
        var rest   = new List<string>();

        foreach (var line in lines)
        {
            if (UsingLineRegex.IsMatch(line))
                usings.Add(line.Trim());
            else
                rest.Add(line);
        }

        return (usings, string.Join("\n", rest));
    }
}
```

**Regex-Wahl:** Das Muster `^\s*(global\s+)?using\s+` erkennt sowohl einfache
(`using NLog;`), alias- (`using X = Foo.Bar;`) als auch `global using`-Direktiven.
Es ist dieselbe Regex-Strategie, die `LogLevelParameterizer` und `PreambleResolver`
bereits verwenden – keine neue Abhängigkeit.

### 2. Angepasste Signatur `CodeSampleWrapper.Wrap()`

```csharp
public static string Wrap(string source, IReadOnlyList<string> sourceUsings)
```

Intern wird die Vereinigungsmenge aus fest codierten Preamble-Usings und
`sourceUsings` gebildet; Reihenfolge: fest codierte Direktiven zuerst, danach
zusätzliche Direktiven aus der Demo-Datei (alphabetisch sortiert):

```csharp
private static readonly string[] FixedPreambleUsings =
[
    "using System;",
    "using NLog;",
];

public static string Wrap(string source, IReadOnlyList<string> sourceUsings)
{
    // Merge: fixed preamble + any additional usings from the demo file.
    var fixedSet   = new HashSet<string>(FixedPreambleUsings, StringComparer.Ordinal);
    var additional = sourceUsings
        .Where(u => !fixedSet.Contains(u))
        .OrderBy(u => u, StringComparer.Ordinal);

    var allUsings = FixedPreambleUsings.Concat(additional);
    var usingBlock = string.Join("\n", allUsings);

    // Strip file-scoped namespace (unchanged logic).
    var lines = source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
    var strippedSource = string.Join("\n", lines.Where(l =>
    {
        var t = l.Trim();
        return !(t.StartsWith("namespace ", StringComparison.Ordinal) && t.EndsWith(";"));
    }));

    return usingBlock + "\n" +
           "\n" +
           "namespace analyzer.test;\n" +
           "\n" +
           strippedSource;
}
```

### 3. Integration in `TestCompiler.ProcessFile()`

`UsingExtractor.Extract()` wird unmittelbar nach `SplitHeaderAndBody()` aufgerufen –
bevor der Body `SourceCleaner.Clean()` durchläuft:

```
Schritt 2a (neu): UsingExtractor.Extract(body)
                  → (sourceUsings, bodyWithoutUsings)
```

Der angepasste Ausschnitt aus `TestCompiler.ProcessFile()`:

```csharp
// Step 2: split header and body
var (header, body) = SourceCleaner.SplitHeaderAndBody(rawContent);

// Step 2a (new): extract using directives before any further processing
var (sourceUsings, bodyWithoutUsings) = UsingExtractor.Extract(body);

// (Steps 3–7 unchanged, but use bodyWithoutUsings instead of body)

// Step 8: wrap – now receives the extracted usings explicitly
var wrapped = CodeSampleWrapper.Wrap(withPreamble, sourceUsings);
```

---

## Weitere Hinweise

- usings kommen immer nur vor der namespace Deklaration vor
- usings sollten vor dem Insert noch einmal de-dupliziert werden

---

## Konsequenzen

### Positiv

- **Eine Wahrheitsquelle:** Die Preamble-Usings sind ausschließlich in
  `CodeSampleWrapper.FixedPreambleUsings` definiert. Kein paralleles `HashSet` mehr.
- **Klare Verantwortlichkeiten:** `UsingExtractor` parst, `Wrap()` fügt zusammen –
  Single Responsibility.
- **Frühes Extrahieren:** Alle nachgelagerten Pipeline-Schritte erhalten einen Quelltext
  ohne `using`-Direktiven; das entspricht dem Muster von `SplitHeaderAndBody`.
- **Robustere Erkennung:** Die Regex erkennt `using`-Aliase und `global using` korrekt;
  die frühere Zeilen-Whitelist konnte das nicht.
- **Erweiterbar:** Sollen Demo-Dateien künftig beliebige zusätzliche Usings einbringen
  (z. B. `using Microsoft.Extensions.Logging;`), werden diese automatisch übernommen –
  ohne Anpassung an `Wrap()`.

### Negativ / Risiken

- **Breaking Change an `Wrap()`:** Alle Aufrufstellen müssen auf die neue Signatur
  umgestellt werden. Laut aktuellem Stand gibt es genau eine (`TestCompiler.ProcessFile`).
- **Scattered Usings:** `UsingExtractor` entfernt `using`-Zeilen unabhängig von ihrer
  Position im Quelltext. Befänden sich `using`-Direktiven mitten im Code (nicht
  kompilierbar, aber theoretisch denkbar), würden auch sie entfernt. Dieses Risiko
  ist bei den vorliegenden Demo-Dateien vernachlässigbar.

---

## Abgrenzung zu ADR-009

ADR-009 bleibt als historisches Dokument bestehen. Die dort getroffene Entscheidung
(nachträgliche Deduplizierung per Whitelist in `Wrap()`) wird durch diese ADR
vollständig ersetzt.
