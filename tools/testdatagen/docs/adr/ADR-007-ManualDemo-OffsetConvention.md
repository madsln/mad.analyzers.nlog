# ADR-007: Offset-Konvention für manuell geschriebene Demo-Dateien

**Status:** Accepted  
**Datum:** 2026-04-25  
**Kontext:** WL-05 – Test Compiler  
**Folgt auf:** ADR-004

---

## Kontext

ADR-004 legt fest, dass der `MetadataHeaderBuilder` den `startOffset` nach der Python-Formel
berechnet:

```
startOffset = (normMsgPos − normClassPos) + crlfCount − 2
```

Diese Formel gilt **ausschließlich für generierte Dateien**, deren Body der `CodeFileBuilder`
mit reinen `\n`-Zeilenenden erzeugt.

Manuell geschriebene Demo-Dateien (z. B. `MAD2254`, `MAD2023`, `MAD2253`, `MAD1727`) entstehen
in einem Texteditor unter Windows und werden mit **`\r\n`-Zeilenenden** gespeichert. Der Autor
misst den Offset als rohe Zeichenposition im CRLF-Dokument:

```
startOffset_manual = rawMsgPos − rawClassPos
                   = (normMsgPos − normClassPos) + crlfCount   // ein extra '+1' pro \n
```

Das `− 2` der Python-Formel fehlt hier vollständig. Im Ergebnis ist `startOffset_manual`
gegenüber `startOffset_generated` um genau **2 größer**.

`SourceCleaner.SplitHeaderAndBody` normalisiert vor der Übergabe an `DiagnosticMarkerInserter`
beide Variantentypen auf `\n`-Zeilenenden. Die inverse Formel in
`ResolveAbsolutePosition` kannte bis dato nur den generierten Fall und überschoss daher bei
manuellen Dateien die tatsächliche Stringposition um 2 Zeichen.

**Symptom:** Der `{|#N:…|}`-Marker begann 2 Zeichen zu spät, z. B. nach dem öffnenden `$"` statt
auf ihm.

---

## Entscheidung

`ResolveAbsolutePosition` berechnet beide Kandidatenpositionen und wählt anhand eines
**Heuristik-Checks** die richtige aus:

```csharp
int absPosGen = classStart + (startOffset - crlfCount + 2);  // generierte Formel (ADR-004)
int absPosMan = absPosGen - 2;                               // manuelle Formel

// Manuelle Position bevorzugen, wenn sie auf einen String-Literal-Opener zeigt
if (absPosMan >= 0 && absPosMan + length <= body.Length && IsStringLiteralStart(body[absPosMan]))
    return absPosMan;

return absPosGen;

static bool IsStringLiteralStart(char c) => c is '"' or '$' or '@';
```

**Begründung der Heuristik:**

- Generierte Nachrichten beginnen immer mit `"Message…` → `body[absPosGen] == '"'`.
  Zwei Zeichen davor (`absPosMan`) steht `(` → kein String-Literal-Opener. Kein Konflikt.
- Manuelle Nachrichten können mit `"`, `$"` oder `@"` beginnen → `body[absPosMan]` trifft den
  Opener. `body[absPosGen]` zeigt dagegen auf das erste *inhaltliche* Zeichen nach dem Opener
  und ist kein String-Literal-Opener (z. B. `D` in `"Doing…`). Kein Konflikt.

Die Heuristik ist **deterministisch** und kommt ohne ein neues Header-Feld aus. Damit bleiben
alle bestehenden manuell erstellten Demo-Dateien ohne Anpassung ihres Headers kompatibel.

---

## Konsequenzen

- `ResolveAbsolutePosition` in `DiagnosticMarkerInserter` implementiert die Dual-Formel
  (siehe oben). Die Methode bleibt `private static` und hat keine Auswirkungen auf andere
  Pipeline-Stufen.
- Manuell geschriebene Demo-Dateien **müssen** ihren `startOffset` als
  `rawMsgPos − rawClassPos` (CRLF-Dokument-Offset) angeben – genauso wie bisher intuitiv
  gemessen.
- Generierte Dateien behalten die Formel aus ADR-004 unverändert.
- Sollte künftig eine dritte Konvention hinzukommen, muss `ResolveAbsolutePosition` erweitert
  oder die Heuristik durch ein explizites Header-Flag ersetzt werden.
