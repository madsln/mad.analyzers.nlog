# ADR-002: Log-Level-Platzhalter in CallTemplateConfig

**Status:** Accepted  
**Datum:** 2026-04-25  
**Kontext:** WL-02 – Config Layer

## Kontext

Das Python-Referenzskript speichert die Call-Templates mit hartkodiertem `Info` als Log-Level (z. B. `_logger.Info({MESSAGE_AND_ARGS});`). Die Log-Level-Parametrisierung findet erst im Unittest-Compiler statt (WL-05, `LogLevelParameterizer`), der `Info` durch `%LOGLEVEL%` ersetzt und dann durch die sechs konkreten Level iteriert.

## Entscheidung

In `CallTemplateConfig` werden die Patterns **direkt mit `%LOGLEVEL%`** als Platzhalter gespeichert (statt mit `Info`):

```
_logger.%LOGLEVEL%({MESSAGE_AND_ARGS});
```

## Begründung

- Die Workload-Beschreibung (WL-02 §2.2) spezifiziert explizit das `%LOGLEVEL%`-Muster.
- Durch direkte Verwendung von `%LOGLEVEL%` entfällt die nachträgliche Suche-und-Ersetzen-Phase im Demo-Generator; der Generator kann das Muster direkt verwenden.
- `LogLevelParameterizer` (WL-05) wird für Demo-Dateien **nicht** benötigt – er wirkt nur im Unittest-Compiler-Pfad auf bereits generierte Dateien.

## Konsequenzen

- `DemoGenerator` (WL-04) ersetzt `%LOGLEVEL%` beim Schreiben der Demo-Dateien durch einen festen Wert (z. B. `Info`), bevor die Datei als Eingabe für den Unittest-Compiler dient.
- `LogLevelParameterizer` im Unittest-Compiler ersetzt das im Demo-File gespeicherte `Info` wieder durch `%LOGLEVEL%` – dies ist der designierte Workflow.
