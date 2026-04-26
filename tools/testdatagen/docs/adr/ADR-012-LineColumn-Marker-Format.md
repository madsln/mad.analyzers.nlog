# ADR-012: Umstellung des Marker-Formats von `startOffset:length` auf `line:column:length`

**Status:** Proposed  
**Datum:** 2026-04-25  
**Kontext:** WL-05 – Test Compiler  
**Folgt auf:** ADR-008  
**Ersetzt:** ADR-004, ADR-007, ADR-008 (Entscheidungsabschnitte)

---

## Kontext

ADR-004 führte eine offset-basierte Formel ein, um die Position eines Diagnostic-Markers im
Datei-Header zu kodieren:

```
// MAD1727=247:27
```

Das Format ist `startOffset:length`, wobei `startOffset` eine zeichenbasierte Relativposition
zum Beginn der `public class`-Deklaration ist. Diese Zahl hängt direkt vom Zeilenenden-Typ
(`\n` vs. `\r\n`) ab, da jeder `\r`-Anteil zum Index beiträgt.

ADR-007 erkannte das Problem für manuell geschriebene Demo-Dateien unter Windows (CRLF) und
löste es durch eine Dual-Formel mit Inhalts-Heuristik. ADR-008 versuchte, diese Heuristik
durch explizite `LineEndingKind`-Metadaten zu ersetzen.

**Beide Lösungen bekämpfen das Symptom, nicht die Ursache.**

Die fundamentale Ursache ist, dass ein Zeichen-Offset per Definition von der Anzahl der
Zeichen vor ihm abhängt – und `\r\n` belegt zwei Zeichen, `\n` nur eines. Jede
Offset-basierte Formel muss deshalb den Zeilenenden-Typ kennen und einrechnen. Das erzeugt
eine strukturelle Kopplung zwischen Header-Wert und Datei-Kodierung, die sich nicht stabil
abschneiden lässt.

### Konkrete Probleme

1. **Zeilenenden-Abhängigkeit:** Der korrekte `startOffset` unterscheidet sich je nach Datei
   um `crlfCount * 1` – eine Zahl, die zur Autorenzeit nicht trivial zu bestimmen ist.
2. **Autorenfreundlichkeit:** Ein Entwickler, der einen manuellen Demo-Test schreibt, muss
   eine nicht-triviale Offset-Berechnung durchführen, um den richtigen Header-Wert zu ermitteln.
   IDEs zeigen Zeile und Spalte an – keinen rohen Zeichenindex relativ zu einer Klassendeklaration.
3. **Fehlerfehleranfälligkeit beim Editieren:** Wird in der Datei vor der markierten Stelle
   eine Zeile eingefügt oder entfernt, muss der `startOffset` manuell nachgepflegt werden.
   Zeilennummer und Spalte ändern sich hingegen lokal vorhersehbar.
4. **Wachsende Komplexität der Formel:** ADR-007 → ADR-008 zeigen eine Eskalation der
   Komplexität in `ResolveAbsolutePosition`. Das Hinzufügen einer weiteren Konvention würde
   einen weiteren `case` erfordern.

---

## Entscheidung

Das Marker-Format im Datei-Header wird von `startOffset:length` auf `line:column:length`
umgestellt.

### 1. Neues Format

```
// MAD1727=12:28:27
```

- **`line`** – 1-basierte Zeilennummer der Zeile, in der der Marker beginnt.
- **`column`** – 1-basierter Spaltenindex des ersten Zeichens des Markers innerhalb dieser
  Zeile (entspricht der Cursor-Position in VS Code / Visual Studio).
- **`length`** – Länge des markierten Spans in Zeichen (unverändert gegenüber heute).

Mehrere Marker in einer Datei werden weiterhin als separate Kommentarzeilen angegeben
(eine Zeile pro Marker), entsprechend dem bestehenden Verhalten von `ParseHeader`:

```
// MAD1234=3:14:5
// MAD1234=7:22:5
```

### 2. Auflösung in `DiagnosticMarkerInserter`

`ResolveAbsolutePosition` wird durch `ResolveAbsolutePositionFromLineColumn` ersetzt:

```csharp
private static int ResolveAbsolutePosition(
    string normalizedBody,
    int line,
    int column)
{
    // normalizedBody hat ausschließlich \n-Zeilenenden (SourceCleaner-Invariante).
    // line und column sind 1-basiert.
    ReadOnlySpan<char> remaining = normalizedBody.AsSpan();
    for (int i = 1; i < line; i++)
    {
        int nl = remaining.IndexOf('\n');
        if (nl < 0)
            throw new InvalidOperationException($"Zeile {line} existiert nicht im Body.");
        remaining = remaining[(nl + 1)..];
    }
    // column ist 1-basiert → Index = column - 1
    return (normalizedBody.Length - remaining.Length) + (column - 1);
}
```

Diese Implementierung ist vollständig unabhängig vom ursprünglichen Zeilenenden-Typ.
`SourceCleaner` normalisiert den Body bereits vor dieser Stufe auf `\n` – die Methode
setzt das als Invariante voraus.

### 3. `DiagnosticMarker`-Modell

```csharp
public sealed record DiagnosticMarker
{
    public string DiagnosticId   { get; init; }
    public int    Line           { get; init; }   // 1-basiert
    public int    Column         { get; init; }   // 1-basiert
    public int    Length         { get; init; }
}
```

Das bisherige `StartOffset`-Feld entfällt.

### 4. Header-Parser

Der bestehende Parser wird erweitert, um das neue dreiteilige Format zu erkennen:

```csharp
// Altes Format (zweiteilig): "247:27"   → StartOffset:Length
// Neues Format (dreiteilig): "12:28:27" → Line:Column:Length
private static DiagnosticMarker ParseMarker(string diagnosticId, string positionPart)
{
    var segments = positionPart.Split(':');
    return segments.Length switch
    {
        3 => new DiagnosticMarker
        {
            DiagnosticId = diagnosticId,
            Line         = int.Parse(segments[0]),
            Column       = int.Parse(segments[1]),
            Length       = int.Parse(segments[2]),
        },
        _ => throw new FormatException(
            $"Ungültiges Marker-Format '{positionPart}'. Erwartet: line:column:length"),
    };
}
```

Das zweiteilige Altformat wird **nicht mehr unterstützt**. Alle bestehenden Demo-Dateien
müssen migriert werden (siehe Konsequenzen).

### 5. Anpassung des `MetadataHeaderBuilder` (generierte Dateien)

Der `MetadataHeaderBuilder` berechnet Zeile und Spalte aus der normalisierten Body-Position:

```csharp
private static (int line, int column) PositionToLineColumn(string normalizedBody, int absolutePos)
{
    // normalizedBody hat \n-Zeilenenden.
    int line   = 1;
    int lastNl = -1;
    for (int i = 0; i < absolutePos; i++)
    {
        if (normalizedBody[i] == '\n')
        {
            line++;
            lastNl = i;
        }
    }
    int column = absolutePos - lastNl; // 1-basiert: lastNl == -1 → column = absolutePos + 1
    return (line, column);
}
```

Die bisherige `startOffset`-Formel (ADR-004) und der `crlfCount`-Parameter entfallen für
neue Dateien vollständig.

---

## Begründung

| Eigenschaft               | `startOffset:length`     | `line:column:length`      |
|---------------------------|--------------------------|---------------------------|
| Zeilenenden-abhängig      | ✗ ja                     | ✓ nein                    |
| Von IDE ablesbar          | ✗ nein (Berechnung nötig)| ✓ ja (Statusleiste)       |
| Robust bei Zeileneinfügung| ✗ nein                   | ✓ lokal nachvollziehbar   |
| Formelkomplexität         | ✗ wächst (ADR-004→008)   | ✓ konstant                |
| Migrationsbedarf          | —                        | ✗ einmalig                |

Das einmalige Migrationsaufwand ist der einzige Nachteil. Er lässt sich durch ein
Migrationswerkzeug (siehe unten) automatisieren.

---

## Konsequenzen

### Pipeline-Änderungen

- `DiagnosticMarker` verliert `StartOffset`, erhält `Line` und `Column`.
- `DiagnosticMarkerInserter.ResolveAbsolutePosition` wird auf die `line`/`column`-basierte
  Implementierung umgestellt. Die offset-basierten Formeln aus ADR-004/007/008 entfallen.
- `MetadataHeaderBuilder` ersetzt die `startOffset`-Berechnung durch `PositionToLineColumn`.
- `SourceFile.OriginalLineEnding` (ADR-008) wird nicht mehr benötigt und kann entfernt werden,
  falls kein anderer Abnehmer existiert.
- `LineEndingKind.Detect` kann entfernt werden, sofern kein anderer Abnehmer existiert.
- Das optionale Header-Feld `line-ending` (ADR-008) entfällt.

### Bestehende Demo-Dateien (Migrationspflicht)

Alle Demo-Dateien mit zweiteiligem Header (`startOffset:length`) müssen auf das dreiteilige
Format (`line:column:length`) migriert werden. Die Migration kann durch ein einmaliges
Migrations-Skript oder durch Erweiterung des `TestDataGen`-Tools erfolgen:

```
TestDataGen migrate --source <demo-root>
```

Das Werkzeug liest den alten Header, löst die Absolutposition nach der bisherigen Formel auf,
berechnet Zeile und Spalte und schreibt den Header neu.

### Abwärtskompatibilität

Das zweiteilige Altformat wird nach der Migration **nicht mehr unterstützt**. Ein Parsing-Fehler
mit klarer Fehlermeldung weist Autoren auf das neue Format hin.

### Status der Vorgänger-ADRs

- ADR-004: bleibt als historische Dokumentation der ursprünglichen Formel erhalten.
- ADR-007: bleibt als historische Dokumentation der Dual-Formel-Heuristik erhalten.
- ADR-008: behält Status „Proposed", wird aber durch dieses ADR überholt und auf
  „Superseded" gesetzt, da die Grundursache des Problems hier anders gelöst wird.
