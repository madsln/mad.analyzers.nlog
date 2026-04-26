# ADR-001: Downgrade von Roslyn 5.3.0 auf 4.9.0 für SonarQube-Plugin-Kompatibilität

**Status:** Proposed  
**Datum:** 2026-04-25

## Kontext

Der `RoslynSonarQubePluginGenerator` (v4.0.0) unterstützt **maximal Roslyn 4.9.0**. Die aktuellen
Projekte referenzieren `Microsoft.CodeAnalysis` v**5.3.0**, weshalb der Generator keine
Analyzer-Typen laden kann und das SonarQube-Plugin nicht erzeugt wird.

Fehlerbild beim Ausführen von `build.ps1`:

```
Maximum supported Roslyn version: 4.9.0.0
[DEBUG] Failed to resolve assembly 'Microsoft.CodeAnalysis, Version=5.3.0.0 ...'
Could not instantiate analyzers from 'mad.analyzers.nlog.common, Version=1.0.0.0 ...'
[WARNING] No analyzers were found in package: mad.analyzers.nlog.common
Plugin was not generated: No Roslyn analyzers were found.
```

## Entscheidung

Downgrade aller `Microsoft.CodeAnalysis.*`-Paketversionen auf **4.9.0** (bzw. kompatible Versionen).

---

## Betroffene Dateien und Änderungen

### 1. `src/lib/mad.analyzers.common.core/mad.analyzers.common.core.csproj`

| Paket                              | Von    | Auf       |
|------------------------------------|--------|-----------|
| `Microsoft.CodeAnalysis.Common`    | 5.3.0  | **4.9.0** |
| `Microsoft.CodeAnalysis.CSharp`    | 5.3.0  | **4.9.0** |
| `Microsoft.CodeAnalysis.Analyzers` | 5.3.0  | **3.3.4** |

---

### 2. `src/analyzer/mad.analyzers.nlog.common/mad.analyzers.nlog.common.csproj`

| Paket                              | Von    | Auf       |
|------------------------------------|--------|-----------|
| `Microsoft.CodeAnalysis.CSharp`    | 5.3.0  | **4.9.0** |
| `Microsoft.CodeAnalysis.Analyzers` | 5.3.0  | **3.3.4** |

---

### 3. `src/codefixes/mad.analyzers.nlog.common.CodeFixes/mad.analyzers.nlog.common.CodeFixes.csproj`

| Paket                                      | Von    | Auf       |
|--------------------------------------------|--------|-----------|
| `Microsoft.CodeAnalysis.CSharp.Workspaces` | 5.3.0  | **4.9.0** |

---

### 4. `src/test/mad.analyzers.nlog.common.Test/mad.analyzers.nlog.common.Test.csproj`

| Paket                        | Von    | Auf       |
|------------------------------|--------|-----------|
| `Microsoft.CodeAnalysis`     | 5.3.0  | **4.9.0** |

> **Hinweis:** Die `Microsoft.CodeAnalysis.CSharp.*.Testing`-Pakete (v1.1.3) sind auf Roslyn 4.x
> ausgelegt und benötigen keine Änderung.

---

### 5. `src/demo/mad.analyzers.nlog.common.demo/mad.analyzers.nlog.common.demo.csproj`

| Paket                              | Von    | Auf       |
|------------------------------------|--------|-----------|
| `Microsoft.CodeAnalysis.Analyzers` | 5.3.0  | **3.3.4** |

---

### 6. `packages.lock.json`-Dateien (alle betroffenen Projekte)

Nach dem Versions-Downgrade müssen alle Lock-Dateien neu generiert werden:

```powershell
dotnet restore --force-evaluate
```

Betroffen:
- `src/analyzer/mad.analyzers.nlog.common/packages.lock.json`
- `src/codefixes/mad.analyzers.nlog.common.CodeFixes/packages.lock.json`

---

## Risiken und Konsequenzen

| Risiko                                                          | Bewertung | Maßnahme                                                                         |
|-----------------------------------------------------------------|-----------|----------------------------------------------------------------------------------|
| API-Inkompatibilitäten in Analyzer-Code                        | Mittel    | Compiler-Fehler nach Downgrade prüfen; Roslyn 4.9 API ist weitgehend rückwärtskompatibel |
| `EnforceExtendedAnalyzerRules` erfordert Roslyn ≥ 4.x          | Kein Risiko | Bereits kompatibel mit 4.9                                                    |
| Test-TFM `net10.0` vs. Roslyn 4.9                              | Gering    | Roslyn 4.9 läuft problemlos auf .NET 10                                          |
| Zukünftige Roslyn-Features (5.x APIs) stehen nicht zur Verfügung | Mittel  | Solange SonarQube-Kompatibilität benötigt wird, bleibt man auf 4.9 gedeckelt    |

## Alternativen (verworfen)

- **Update des SonarQube Roslyn SDK:** Aktuell gibt es keine offizielle Version mit Roslyn-5-Support.
- **Eigener Fork des Generators:** Zu hoher Aufwand.
- **Separater Build-Branch mit gepinnter Roslyn-Version nur für SonarPlugin:** Möglich, erhöht aber
  den Wartungsaufwand erheblich.
