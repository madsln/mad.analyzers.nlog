# ADR-008: LineEnding-Metadatum als Ersatz für die IsStringLiteralStart-Heuristik

**Status:** Proposed  
**Datum:** 2026-04-25  
**Kontext:** WL-05 – Test Compiler  
**Folgt auf:** ADR-007  
**Ersetzt:** ADR-007 (Entscheidungsabschnitt)

---

## Kontext

ADR-007 löste das Problem abweichender `startOffset`-Werte in manuell geschriebenen Demo-Dateien
(CRLF-Zeilenenden) gegenüber generierten Demo-Dateien (LF-Zeilenenden) durch eine
**Dual-Formel mit Inhalts-Heuristik**:

```csharp
int absPosGen = classStart + (startOffset - crlfCount + 2);
int absPosMan = absPosGen - 2;

if (absPosMan >= 0 && absPosMan + length <= body.Length && IsStringLiteralStart(body[absPosMan]))
    return absPosMan;

return absPosGen;

static bool IsStringLiteralStart(char c) => c is '"' or '$' or '@';
```

Diese Lösung ist funktional korrekt, hat jedoch strukturelle Schwächen:

1. **Indirekte Evidenz:** Die Formelwahl basiert auf dem *Inhalt* der Zieldatei, nicht auf der
   tatsächlichen Ursache (dem ursprünglichen Zeilenenden-Typ der Quelldatei).
2. **Brüchige Annahme:** „Generierte Nachrichten beginnen nie mit `"`, `$` oder `@` an
   Position `absPosMan`" ist zufällig wahr, nicht strukturell garantiert. Eine zukünftige
   Nachrichtenvorlage könnte diese Annahme brechen.
3. **Keine Behandlung von Mixed-Dateien:** Dateien mit gemischten Zeilenenden sind im Modell
   nicht vorgesehen.
4. **Erweiterbarkeits-Warnung im ADR selbst:** ADR-007 hält explizit fest, dass bei einer
   dritten Konvention die Heuristik durch ein explizites Header-Flag ersetzt werden müsste.

---

## Entscheidung

Der ursprüngliche **Zeilenenden-Typ** einer Demo-Datei wird beim Einlesen einmalig erkannt,
als `LineEndingKind`-Wert am `SourceFile`-Modell gespeichert und durch die Pipeline
weitergereicht. `ResolveAbsolutePosition` wählt die Formel ausschließlich anhand dieses
Metadatums – die `IsStringLiteralStart`-Heuristik entfällt.

### 1. Enum `LineEndingKind`

```csharp
public enum LineEndingKind
{
    LF,     // ausschließlich \n  – generierte Dateien (ADR-004)
    CRLF,   // ausschließlich \r\n – manuell geschriebene Dateien unter Windows
    Mixed   // gemischte Zeilenenden – explizite Header-Angabe erforderlich
}
```

### 2. Erkennung beim Einlesen

Die Erkennung erfolgt in der Lade-Schicht, bevor `SourceCleaner` die Zeilenenden normalisiert:

```csharp
public static LineEndingKind Detect(string rawContent)
{
    bool hasCRLF = rawContent.Contains("\r\n");
    bool hasLF   = rawContent.Replace("\r\n", "").Contains('\n');

    return (hasCRLF, hasLF) switch
    {
        (true,  true)  => LineEndingKind.Mixed,
        (true,  false) => LineEndingKind.CRLF,
        _              => LineEndingKind.LF,
    };
}
```

### 3. Metadatum am `SourceFile`-Modell

```csharp
public sealed record SourceFile
{
    // ... bestehende Felder ...
    public LineEndingKind OriginalLineEnding { get; init; }
}
```

Das Feld wird beim Konstruieren des `SourceFile`-Objekts einmalig gesetzt und ist danach
unveränderlich. Alle nachgelagerten Pipeline-Stufen greifen lesend darauf zu.

### 4. Optionales Header-Feld `line-ending`

Für `Mixed`-Dateien oder zur expliziten Überschreibung kann der Datei-Header ein optionales
Feld angeben:

```
line-ending: crlf
```

Erlaubte Werte: `lf`, `crlf`, `mixed`. Fehlt das Feld, gilt der automatisch erkannte Wert.
Das Header-Feld hat **höhere Priorität** als die Auto-Erkennung.

### 5. Formel in `ResolveAbsolutePosition`

```csharp
int absPos = file.OriginalLineEnding == LineEndingKind.CRLF
    ? classStart + (startOffset - crlfCount)      // CRLF-Quelle: kein − 2 (ADR-007)
    : classStart + (startOffset - crlfCount + 2); // LF-Quelle:   ADR-004-Formel

return absPos;
```

`IsStringLiteralStart` und die Dual-Kandidaten-Logik werden vollständig entfernt.

---

## Begründung

- **Direkte Kausalität:** Die Formelwahl folgt der Ursache (Zeilenenden-Typ der Quelldatei),
  nicht einem Symptom im Inhalt.
- **Erweiterbarkeit:** Eine dritte Konvention erfordert lediglich einen neuen `LineEndingKind`-Wert
  und einen weiteren `case` in der Formel – kein Eingriff in Inhalts-Checks.
- **Selbstdokumentation:** `OriginalLineEnding` macht die Intention für jeden Leser sofort
  verständlich, ohne Kommentar-Erklärung.
- **Rückwärtskompatibilität:** Alle bestehenden manuellen Demo-Dateien haben CRLF-Zeilenenden
  und werden durch die Auto-Erkennung korrekt klassifiziert – **kein Header-Update nötig**.
  Generierte Dateien haben LF und werden ebenfalls automatisch korrekt behandelt.
- **Mixed-Sicherheitsnetz:** Dateien mit gemischten Zeilenenden lösen entweder eine Warnung aus
  oder werden durch das optionale Header-Feld explizit behandelt, statt still falsch zu rechnen.

---

## Konsequenzen

- `LineEndingKind` wird als neuer Typ in der gemeinsamen Modell-Assembly eingeführt.
- `SourceFile` erhält das Feld `OriginalLineEnding`.
- Die Lade-Schicht (vor `SourceCleaner`) ruft `LineEndingKind.Detect` auf dem Raw-Content auf
  und setzt das Feld.
- Das optionale Header-Feld `line-ending` wird vom Header-Parser erkannt und überschreibt
  ggf. den erkannten Wert.
- `ResolveAbsolutePosition` in `DiagnosticMarkerInserter` wird auf die einfache Formel umgestellt.
- `IsStringLiteralStart` und die Dual-Kandidaten-Logik werden entfernt.
- Bestehende Demo-Dateien benötigen **keine Anpassung**.
- ADR-007 behält den Status „Accepted", wird aber durch dieses ADR in seinem
  Entscheidungsabschnitt abgelöst. Der Kontext- und Konsequenz-Abschnitt von ADR-007 bleibt
  als historische Dokumentation erhalten.
