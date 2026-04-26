# ADR-001: Feldtyp für ILoggerExtensions

**Status:** Accepted  
**Datum:** 2026-04-25  
**Kontext:** WL-02 – Config Layer

## Kontext

In `LoggerConfig.FieldDeclarations` muss für den Logger-Typ `ILoggerExtensions` eine Felddeklaration hinterlegt werden. Laut Workload-Beschreibung wäre `Logger` als Feldtyp naheliegend, da `ILoggerExtensions`-Methoden auf dem konkreten `Logger`-Typ definiert sind.

## Entscheidung

Der Feldtyp für `ILoggerExtensions` wird als **`ILogger`** deklariert:

```csharp
"ILoggerExtensions" → "private readonly ILogger _logger = LogManager.GetCurrentClassLogger();"
```

## Begründung

Das Python-Referenzskript (`scripts/gen_MAD2017_demo_files.py`) verwendet für `ILoggerExtensions` explizit `ILogger` als Feldtyp – identisch mit dem `ILogger`-Logger-Typ. Die C#-Implementierung folgt dem Referenzskript 1:1, um Byte-for-Byte-identische Ausgabe sicherzustellen (Kriterium WL-07).

## Konsequenzen

- Alle drei Logger-Typen haben eine ähnliche Felddeklaration (`ILogger` oder `Logger`).
- Die Demo-Dateien für `ILoggerExtensions` deklarieren das Feld als `ILogger`-Referenz.
