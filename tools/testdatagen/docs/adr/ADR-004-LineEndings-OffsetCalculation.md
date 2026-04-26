# ADR-004: Zeilenenden & Offset-Berechnung im MetadataHeaderBuilder

**Status:** Accepted  
**Datum:** 2026-04-25  
**Kontext:** WL-04 – Demo Generator

---

## Kontext

Der `MetadataHeaderBuilder` berechnet den `StartOffset` des Message-Template-Strings relativ zum
Beginn der `public class`-Deklaration. Das Python-Referenzskript enthält dabei eine auffällige
Korrektur:

```python
offset_start_idx = message_start_idx - class_start_idx + crlf_count - 2  # i dunno why -2 but it works
```

Der `crlf_count` zählt `\n`-Zeichen im Substring `[class_start+1 .. message_end]`. Das `-2` ist
unkommentiert, aber empirisch korrekt für das Python-Template (das mit `\n` arbeitet und eine
führende Leerzeile nach dem Namespace enthält).

---

## Entscheidung

Der `CodeFileBuilder` erzeugt den Datei-Body mit **`\n`-Zeilenenden** (kein `\r\n`).  
Der `MetadataHeaderBuilder` portiert die Python-Formel **1:1**:

```csharp
int crlfCount = /* Anzahl '\n' in fileBodyCode[classStart+1 .. messageEnd] */;
int startOffset = messageStart - classStart + crlfCount - 2;
```

Der `OutputWriter` schreibt mit `File.WriteAllText` und dem von .NET verwendeten
`UTF8Encoding(false)`. Plattformspezifische Zeilenenden werden **nicht** normalisiert –
die Datei enthält `\n` so wie sie gebaut wurde.

---

## Begründung

- Die Python-Formel wurde als Black-Box übernommen (`i dunno why -2 but it works`).  
  Solange der Body dieselbe Struktur hat wie das Python-Template, liefert die Formel korrekte Offsets.
- Konsistente `\n`-Zeilenenden im Body sind notwendig, damit `crlfCount` und die String-Indizes
  übereinstimmen – ein gemischtes `\r\n` würde die Rechnung verfälschen.
- Das Abnahmekriterium WL-07 (Byte-for-Byte-Vergleich mit Python-Ausgabe) validiert die Korrektheit.

---

## Konsequenzen

- `CodeFileBuilder.BuildBody()` darf ausschließlich `\n` als Zeilenumbruch verwenden.
- `MetadataHeaderBuilder.Build()` erwartet einen Body mit `\n`-Zeilenenden.
- Beim Vergleich mit der Python-Ausgabe unter Windows müssen `\r\n`-Normalisierungen
  berücksichtigt werden (Python `open(..., "w")` schreibt unter Windows `\r\n`).
