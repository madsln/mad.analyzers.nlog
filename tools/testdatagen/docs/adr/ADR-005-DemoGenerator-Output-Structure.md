# ADR-005: DemoGenerator-Ausgabestruktur für TestCompiler-Kompatibilität

**Status:** Accepted  
**Datum:** 2026-04-25  
**Kontext:** WL-05 – Test Compiler Pipeline

---

## Kontext

Der `TestCompiler` (WL-05) erwartet als `--source`-Verzeichnis eine Struktur mit drei
Unterverzeichnissen:

```
{source}/
  usings/       ← Preamble-Dateien (Dateiname = Symbolname)
  diagnostics/  ← Demo-Dateien mit erwartetem Diagnostic
  nodiag/       ← Demo-Dateien ohne erwartetes Diagnostic
```

Der `DemoGenerator` (WL-04) schrieb Dateien ursprünglich in:

```
{outDir}/{loggertype}/{ClassName}.cs
```

Diese Struktur ist inkompatibel mit der Erwartung des TestCompilers. Der `AllCommand` (WL-06)
soll `demo` und `test` intern verketten, d. h. das Ausgabeverzeichnis von `demo` muss direkt
als `--source` für `test` verwendbar sein.

---

## Entscheidung

Der `DemoGenerator` schreibt Ausgabedateien ab sofort nach:

```
{outDir}/diagnostics/MAD2017/{loggertype}/{ClassName}.cs
```

Damit erzeugt `demo --out ./tmp/demo` direkt eine Struktur, bei der
`test --source ./tmp/demo` ohne manuelle Zwischenschritte funktioniert.

Die `usings/`- und `nodiag/`-Unterverzeichnisse werden vom `TestCompiler` gracefully behandelt:
fehlende Verzeichnisse werden als leer gewertet.

Die erzeugten Test-Klassennamen entsprechen dem Schema:

```
diagnostics_MAD2017_{loggertype}_Tests
```

was mit den bestehenden generierten Testdateien in
`src/test/mad.analyzers.nlog.Test/generated/diagnostics/` übereinstimmt.

---

## Begründung

- Minimale Kopplung zwischen WL-04 und WL-05: der `AllCommand` übergibt einfach das demo-Ausgabeverzeichnis als test-Source, keine zusätzliche Kopier- oder Umstrukturierungslogik.
- Die Namespace-Konvention in generierten Dateien (`...diagnostics.MAD2017.{loggertype}`) ist bereits korrekt – nur der **Ausgabepfad** ändert sich.
- Die Änderung ist abwärtskompatibel: WL-07-Verifikation vergleicht mit Python-Referenzausgabe; die Dateiinhalte bleiben identisch.

---

## Konsequenzen

- `DemoGenerator.Generate()` schreibt nach `{outDir}/diagnostics/MAD2017/{loggertype.ToLower()}/` statt `{outDir}/{loggertype.ToLower()}/`.
- `WL-07`-Demo-Vergleich: Verzeichnispfade in der Referenzausgabe anpassen oder Python-Skript mit entsprechendem `--out` aufrufen.
- Abnahmekriterium WL-04: `dotnet run -- demo --out ./tmp/demo` erzeugt nun `./tmp/demo/diagnostics/MAD2017/logger/`, `./ilogger/`, `./iloggerextensions/`.
