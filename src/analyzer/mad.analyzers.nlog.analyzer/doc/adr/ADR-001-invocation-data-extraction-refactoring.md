# ADR-001 – Vorab-Extraktion von Invocation-Daten in `LogMessageTemplateParameterCountMismatchAnalyzer`

| Feld      | Wert                                   |
|-----------|----------------------------------------|
| Status    | **Proposed**                           |
| Datum     | 2026-04-25                             |
| Autor     | –                                      |
| Kontext   | `LogMessageTemplateParameterCountMismatchAnalyzer.cs` |

---

## Kontext

Der Analyzer führt pro erkannter NLog-Invocation drei unabhängige Analysen durch:

| Methode                        | Diagnostics          |
|-------------------------------|----------------------|
| `AnalyzeForMisplacedExceptions` | MAD2255              |
| `AnalyzeForInlineSerialization` | MAD2256              |
| `AnalyzeFormatArgument`         | MAD1727, MAD2017, MAD2023, MAD2253, MAD2254 |

Dabei werden mehrere kostspielige Operationen redundant – d.h. mehrfach auf denselben Daten – ausgeführt. Der Analyzer wird vom Roslyn-Host für **jede** qualifizierte Methodeninvocation im zu analysierenden Quellcode aufgerufen. Bei großen Codebasen mit vielen Log-Aufrufen summieren sich selbst kleine Überheads.

---

## Beobachtete Redundanzen

### R-1 · `invocation.Arguments` wird 3–4× iteriert

| Stelle                          | Zweck                                                      |
|---------------------------------|------------------------------------------------------------|
| `AnalyzeInvocation`             | `formatExpression` finden + `paramsCount` berechnen        |
| `AnalyzeForMisplacedExceptions` | Argumente auf Exception-Typen prüfen                       |
| `AnalyzeForInlineSerialization` | **Erst** `formatExpression` suchen (early-break), **dann** nochmal alle Argumente für Serialisierungs-Check |

Die Argumentliste bei NLog-Methoden ist zwar kurz (typisch 2–5 Elemente), aber das Muster ist klar: dieselbe Struktur wird ohne gemeinsame Vorarbeit mehrfach traversiert.

### R-2 · `HasParamCollectionAttribute(parameter)` wird bis zu 3× auf demselben Parameter aufgerufen

`HasParamCollectionAttribute` ruft intern `parameter.GetAttributes()` auf, was in Roslyn einen Symbol-Lookup über die Metadaten der Compilation auslöst. Es ist kein trivialer Property-Zugriff. Derselbe `params`-Parameter der Methode wird in allen drei Analyse-Pfaden neu geprüft:

```csharp
// AnalyzeInvocation
if (parameter.IsParams || HasParamCollectionAttribute(parameter)) { ... }

// AnalyzeForMisplacedExceptions
if (parameter.IsParams || HasParamCollectionAttribute(parameter)) { ... }

// AnalyzeForInlineSerialization
if (parameter.IsParams || HasParamCollectionAttribute(parameter)) { ... }
```

### R-3 · `TryGetFormatText(formatExpression)` wird 2× aufgerufen

`TryGetFormatText` traversiert rekursiv den Roslyn-Operationsbaum (z.B. bei String-Konkatenation via `+`). Sie wird aufgerufen:

1. In `AnalyzeForInlineSerialization` – als Guard-Bedingung (`== null → return`)
2. In `AnalyzeFormatArgument` – um den Text tatsächlich auszuwerten

Beide Male auf dem identischen `formatExpression`-Objekt.

### R-4 · `context.Compilation.GetTypeByMetadataName("System.Exception")` pro Invocation

In `AnalyzeForMisplacedExceptions` wird bei **jeder einzelnen Invocation** der Exception-Typ aus der Compilation nachgeschlagen. `GetTypeByMetadataName` ist intern gecacht in Roslyn, aber der Aufruf selbst erzeugt dennoch Overhead durch Dictionary-Lookups und ggf. Lock-Contention bei `EnableConcurrentExecution`. Dieser Wert ändert sich für die gesamte Compilation nie und sollte einmalig im `RegisterCompilationStartAction`-Callback abgerufen und gecacht werden.

### R-5 · `methodMetadata.LogParameters.Any(p => SymbolEqualityComparer.Default.Equals(p, parameter))` – lineare Suche mit Equality-Vergleich

Diese Suche wird in `AnalyzeInvocation` und `AnalyzeForInlineSerialization` pro Argument ausgeführt. `SymbolEqualityComparer` ist ein struktureller Vergleich. Für NLog mit wenigen Parametern ist der Schaden gering, aber ein `HashSet<IParameterSymbol>` in `LogMethodMetadata` würde dies auf O(1) reduzieren.

### R-6 · `GetParamElements` und `CountParamsElements` duplizieren Traversal-Logik

Beide Methoden traversieren denselben Operationsbaum (Array/Collection):
- `CountParamsElements` → zählt Elemente (für MAD2017)
- `GetParamElements` → enumeriert Elemente (für MAD2255, MAD2256)

Die Logik ist strukturell ähnlich, aber getrennt implementiert. Da der Baum nur einmal traversiert werden müsste, um sowohl Anzahl als auch Elemente zu liefern, entsteht hier doppelte Arbeit.

---

## Zusammenfassung der Redundanzen

| ID  | Redundanz                                       | Kosten        | Häufigkeit              |
|-----|-------------------------------------------------|---------------|-------------------------|
| R-1 | 4× Iteration über `invocation.Arguments`        | Niedrig       | Pro Log-Invocation      |
| R-2 | 3× `HasParamCollectionAttribute` (GetAttributes) | **Mittel**   | Pro params-Parameter    |
| R-3 | 2× `TryGetFormatText` (Operationsbaum-Walk)     | **Mittel**    | Pro Log-Invocation      |
| R-4 | `GetTypeByMetadataName` pro Invocation          | **Hoch**      | Pro Log-Invocation      |
| R-5 | Lineare Suche in `LogParameters`                | Niedrig       | Pro Argument pro Call   |
| R-6 | Doppelte Traversal für Count vs. Enumerate      | Niedrig       | Nur bei params-Argumenten |

**Kritischster Punkt: R-4** — der Compilation-Lookup des Exception-Typs gehört in den CompilationStart-Scope.  
**Zweitwichtigster Punkt: R-3** — TryGetFormatText ist ein rekursiver Tree-Walk.

---

## Entscheidungsoptionen

### Option A – Status quo beibehalten

Keine Änderungen. Der Analyzer ist korrekt und die Überheads sind bei typischen Codebasen nicht messbar wahrnehmbar.

**Pro:** Kein Refactoring-Aufwand, kein Risiko.  
**Contra:** Unnötige Redundanz bleibt bestehen; schlechtes Vorbild für zukünftige Regeln.

---

### Option B – Gezielte Einzelfixes (minimaler Eingriff)

Nur die zwei teuersten Redundanzen beheben, ohne große Strukturänderungen:

1. **R-4 lösen:** `exceptionType`-Lookup in den `CompilationStartAction`-Closure ziehen:

   ```csharp
   context.RegisterCompilationStartAction(startAnalysisContext =>
   {
       // ... bestehende Typ-Lookups ...
       var exceptionType = startAnalysisContext.Compilation
           .GetTypeByMetadataName("System.Exception");

       context.RegisterOperationAction(context =>
       {
           AnalyzeInvocation(context, loggerType!, iLoggerType!, loggerExtensionsType!, exceptionType);
       }, OperationKind.Invocation);
   });
   ```

2. **R-3 lösen:** `TryGetFormatText` einmalig in `AnalyzeInvocation` aufrufen und Ergebnis (`string? formatText`) an beide Methoden weitergeben.

**Pro:** Minimaler Eingriff, gezielte Verbesserung der teuersten Stellen.  
**Contra:** Löst nicht alle Redundanzen; Signaturänderungen an einzelnen Methoden.

---

### Option C – `InvocationData`-Record als gemeinsame Datenbasis (empfohlen)

Alle Invocation-relevanten Daten werden **einmalig** in `AnalyzeInvocation` extrahiert und als `InvocationData`-Record an alle Sub-Analysen übergeben. Dies löst R-1 bis R-5 auf einmal.

```csharp
private readonly record struct InvocationData
{
    /// <summary>Die formatExpression-Operation (das message-Argument).</summary>
    public IOperation? FormatExpression { get; init; }

    /// <summary>Ergebnis von TryGetFormatText – null wenn nicht statisch.</summary>
    public string? FormatText { get; init; }

    /// <summary>Ob das Template statisch auflösbar ist.</summary>
    public bool IsStaticTemplate => FormatText != null;

    /// <summary>Anzahl der tatsächlich übergebenen Log-Argumente.</summary>
    public int ParamsCount { get; init; }

    /// <summary>
    /// Flach aufgelöste Liste aller Log-Argumente (params bereits expandiert).
    /// Verwendet von MAD2255 und MAD2256.
    /// </summary>
    public ImmutableArray<IOperation> LogArgumentValues { get; init; }

    /// <summary>Ob die Argumente als params-Array übergeben wurden.</summary>
    public bool IsParamsArgument { get; init; }
}
```

Die Extraktion erfolgt in einer neuen Methode `ExtractInvocationData(...)`, die einmalig iteriert:

```csharp
private static InvocationData ExtractInvocationData(
    IInvocationOperation invocation,
    LogMethodMetadata metadata)
{
    IOperation? formatExpr = null;
    int paramsCount = 0;
    bool isParamsArgument = false;
    var logArgValues = ImmutableArray.CreateBuilder<IOperation>();

    foreach (var argument in invocation.Arguments)
    {
        var parameter = argument.Parameter;
        if (parameter == null) continue;

        if (SymbolEqualityComparer.Default.Equals(parameter, metadata.MessageParameter))
        {
            formatExpr = argument.Value;
            continue;
        }

        if (!metadata.LogParameterSet.Contains(parameter)) continue;

        bool isParams = parameter.IsParams || HasParamCollectionAttribute(parameter); // einmalig!
        if (isParams)
        {
            isParamsArgument = true;
            var elements = GetParamElements(argument.Value).ToImmutableArray();
            paramsCount = elements.Length;
            logArgValues.AddRange(elements);
        }
        else
        {
            paramsCount++;
            logArgValues.Add(argument.Value);
        }
    }

    var formatText = formatExpr != null ? TryGetFormatText(formatExpr) : null;

    return new InvocationData
    {
        FormatExpression = formatExpr,
        FormatText = formatText,
        ParamsCount = paramsCount,
        IsParamsArgument = isParamsArgument,
        LogArgumentValues = logArgValues.ToImmutable()
    };
}
```

`AnalyzeForMisplacedExceptions` und `AnalyzeForInlineSerialization` erhalten dann `InvocationData` statt `IInvocationOperation` + `LogMethodMetadata` und iterieren nur noch über `data.LogArgumentValues`.

Zusätzlich wird `LogMethodMetadata.LogParameters` als `HashSet<IParameterSymbol>` (`LogParameterSet`) gehalten (löst R-5).

**Pro:**
- Jede redundante Operation wird eliminiert
- Klare Trennung: Daten-Extraktion vs. Diagnose-Logik
- Einfacher testbar (InvocationData kann direkt konstruiert werden)
- Zukunftssicher: neue Regeln können `InvocationData` direkt nutzen

**Contra:**
- Größerer Refactoring-Aufwand (~80–120 Zeilen Änderung)
- `GetParamElements` liefert jetzt immer eine materialisierte `ImmutableArray` (minimaler Alloc-Overhead bei params-losen Calls – dann bleibt sie leer)

---

## Empfehlung

**Option B als Sofortmaßnahme, Option C mittelfristig.**

R-4 (`GetTypeByMetadataName` pro Invocation) ist das einzige wirklich messbare Problem bei großen Projekten und sollte sofort behoben werden (2 Zeilen Änderung). R-3 (`TryGetFormatText` 2×) ist bei String-Konkatenation im Template ebenfalls real.

Option C macht Sinn sobald eine weitere Regel hinzukommt, da der Break-even dann bereits erreicht ist. Der aktuelle Zustand mit 3 Regeln, die über `invocation.Arguments` iterieren, ist genau der Punkt, an dem das Muster anfängt sich zu lohnen.

---

## Konsequenzen bei Umsetzung von Option C

- `LogMethodMetadata` bekommt ein zusätzliches Feld `LogParameterSet: HashSet<IParameterSymbol>`
- `AnalyzeForMisplacedExceptions` und `AnalyzeForInlineSerialization` werden von `IInvocationOperation` auf `InvocationData` umgestellt
- `exceptionType` wird aus `AnalyzeForMisplacedExceptions`-Signatur herausgezogen und im CompilationStart-Scope gecacht
- Tests bleiben unverändert (öffentliche Analyzer-API ändert sich nicht)
