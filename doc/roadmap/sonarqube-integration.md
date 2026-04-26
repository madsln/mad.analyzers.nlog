# Roadmap: SonarQube Integration

**Status:** Planned  
**Voraussetzung:** Stabiles NuGet-Paket (`mad.analyzers.nlog.common`) ist released

---

## Überblick

SonarQube unterstützt Roslyn-basierte Analyzer über das sogenannte **SonarQube Roslyn SDK**.
Dabei wird ein eigenes NuGet-Paket erzeugt, das SonarQube als Plugin-Grundlage dient und
die Regeln (Rules) in das SonarQube-Regelwerk einbringt.

Die Integration erfolgt in zwei Artefakten:
1. Ein **SonarQube Plugin (`.jar`)**, das die Regeln in SonarQube registriert.
2. Ein **SonarScanner-kompatibles NuGet-Paket**, das beim Scan die Analyzer-DLLs mitliefert.

---

## Vorarbeiten / Abhängigkeiten

- [ ] Das NuGet-Paket `mad.analyzers.nlog.common` muss auf NuGet.org verfügbar sein (oder
      lokal/intern erreichbar für den Build-Schritt).
- [ ] Alle Diagnose-IDs (`MAD1727`, `MAD2017`, `MAD2023`, `MAD2253`, `MAD2254`, `MAD2255`, `MAD2256`)
      müssen final und stabil sein – spätere Umbenennungen erfordern Plugin-Updates.
- [ ] Java (JDK 11+) muss im Build-Umfeld verfügbar sein (für den Plugin-Build via Maven).
- [ ] Maven muss verfügbar sein (oder ein Docker-Image genutzt werden).

---

## Schritt 1 – SonarQube Roslyn SDK einrichten

Das offizielle Werkzeug ist das [SonarSource Roslyn SDK](https://github.com/SonarSource/sonarqube-roslyn-sdk).

```powershell
# SDK-Repository klonen (einmalig)
git clone https://github.com/SonarSource/sonarqube-roslyn-sdk.git
cd sonarqube-roslyn-sdk
mvn install -DskipTests
```

Das SDK enthält das Tool `RoslynSonarPlugin` (als ausführbare `.exe` / dotnet-Tool),
das aus einem NuGet-Paket automatisch ein SonarQube-Plugin generiert.

---

## Schritt 2 – Plugin-Konfigurationsdatei erstellen (`PluginManifest.xml`)

Im neuen Projektordner `src/installer/mad.analyzers.nlog.common.SonarPlugin/` eine
Konfigurationsdatei anlegen:

```xml
<!-- PluginManifest.xml -->
<AnalyzerPlugin>
  <Key>mad.analyzers.nlog.common</Key>
  <Version>0.6.0</Version>               <!-- an CommonVersion koppeln -->
  <StaticResourceName>static/mad.analyzers.nlog.common.zip</StaticResourceName>
  <PackageId>mad.analyzers.nlog.common</PackageId>
  <PackageVersion>0.6.0</PackageVersion>
  <NuGetPackageSource>https://api.nuget.org/v3/index.json</NuGetPackageSource>
  <PluginName>MAD NLog Analyzers</PluginName>
  <PluginDescription>Roslyn-based analyzers for common NLog logging mistakes.</PluginDescription>
</AnalyzerPlugin>
```

> **Hinweis:** `<Key>` muss in SonarQube einmalig sein und darf nach dem ersten Deployment
> nicht mehr geändert werden.

---

## Schritt 3 – Regel-Metadaten hinterlegen

SonarQube benötigt für jede Regel eine Beschreibung in HTML-Form.
Diese Dateien werden im Plugin-Manifest-Ordner unter `rules/<RuleId>/` abgelegt:

```
src/installer/mad.analyzers.nlog.common.SonarPlugin/
  rules/
    MAD1727/
      description.html
      squid.json        ← Metadaten: Severity, Tags, Type
    MAD2017/
      ...
    MAD2023/
    MAD2253/
    MAD2254/
    MAD2255/
    MAD2256/
```

Beispiel `squid.json`:
```json
{
  "title": "Log message template should not use Pascal case names",
  "type": "CODE_SMELL",
  "status": "ready",
  "remediation": {
    "func": "Constant/Issue",
    "constantCost": "2min"
  },
  "tags": ["nlog", "logging"],
  "defaultSeverity": "Minor"
}
```

Die `description.html`-Dateien können aus den bereits vorhandenen Analyzer-Docs unter
`src/analyzer/mad.analyzers.nlog.common/doc/` generiert oder manuell erstellt werden.

---

## Schritt 4 – Plugin bauen

```powershell
# Aus dem Roslyn SDK heraus
RoslynSonarAnalyzer.exe `
    --package-id mad.analyzers.nlog.common `
    --package-version 0.6.0 `
    --sqale rules/ `
    --out ./out/
```

Das Ergebnis ist eine `.jar`-Datei, z. B. `mad-analyzers-nlog-common-plugin-0.6.0.jar`.

---

## Schritt 5 – Neues Projekt in der Solution anlegen

```
src/
  installer/
    mad.analyzers.nlog.common.SonarPlugin/   ← neu
      PluginManifest.xml
      rules/
      build.ps1                              ← lokales Build-Skript
      mad.analyzers.nlog.common.SonarPlugin.csproj  (optional, nur als Platzhalter/Dokumentation)
```

Das `.csproj` kann als reines "None"-Projekt angelegt werden, das lediglich
`PluginManifest.xml` und die `rules/`-Dateien referenziert, damit sie in der Solution
sichtbar sind.

---

## Schritt 6 – CI/CD-Pipeline erweitern (`.github/workflows/ci.yml`)

Nach dem bestehenden `publish`-Job einen neuen Job `publish-sonar-plugin` hinzufügen:

```yaml
publish-sonar-plugin:
  runs-on: windows-latest
  if: startsWith(github.ref, 'refs/tags/versions/sonar/')

  steps:
    - name: Checkout code
      uses: actions/checkout@v6

    - name: Set up Java
      uses: actions/setup-java@v5
      with:
        distribution: temurin
        java-version: '25'

    - name: Set up .NET
      uses: actions/setup-dotnet@v5
      with:
        dotnet-version: '10.0.x'

    - name: Cache Maven packages
      uses: actions/cache@v5
      with:
        path: ~/.m2
        key: ${{ runner.os }}-m2-${{ hashFiles('**/pom.xml') }}
        restore-keys: ${{ runner.os }}-m2

    - name: Clone SonarSource Roslyn SDK
      run: git clone --branch 4.0 --depth 1 https://github.com/SonarSource/sonarqube-roslyn-sdk.git ../sonarqube-roslyn-sdk

    - name: Build Roslyn SDK
      working-directory: ../sonarqube-roslyn-sdk
      run: mvn install -DskipTests --batch-mode --no-transfer-progress

    - name: Build SonarQube plugin
      run: |
        ../sonarqube-roslyn-sdk/RoslynPluginGenerator/bin/Release/net48/RoslynSonarQubePluginGenerator.exe \
            /analyzer:"mad.analyzers.nlog:1.0.0" \
            /ouputdir:"./sonar-plugin" \
            /language:cs \
            /recurse

    - name: Upload plugin artifact
      uses: actions/upload-artifact@v7
      with:
        name: sonar-plugin
        path: ./sonar-plugin/*.jar

    - name: Publish plugin to GitHub Release
      uses: softprops/action-gh-release@v3
      with:
        files: ./sonar-plugin/*.jar
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

---

## Schritt 7 – Deployment in SonarQube

1. `.jar`-Datei in das `extensions/plugins/`-Verzeichnis der SonarQube-Instanz kopieren.
2. SonarQube-Server neu starten.
3. Unter **Administration → Rules** prüfen, ob die `MAD`-Regeln erscheinen.
4. Ein Quality Profile erstellen oder erweitern, das die neuen Regeln aktiviert.

---

## Offene Punkte / Entscheidungen

| # | Frage | Relevanz |
|---|-------|----------|
| 1 | Soll das Plugin auf dem SonarQube Marketplace veröffentlicht werden? | Langfristig; erfordert SonarSource Community-Zertifizierung |
| 2 | Welche SonarQube-Mindestversion soll unterstützt werden? | Beeinflusst Plugin-API-Version im SDK | SonarQube 9.9 LTS (am weitesten verbreitet bei OnPrem-Installationen; aktuelle LTS-Linie) |
| 3 | Soll ein SonarCloud-Support (hosted) ebenfalls angeboten werden? | Andere Deployment-Route als Self-Hosted |
| 4 | Severity-Mapping: Wie sollen die Roslyn-Severity-Stufen auf SonarQube-Typen abgebildet werden? | `Warning` → `CODE_SMELL / Minor`, `Error` → `BUG / Major`? |
| 5 | Soll der Plugin-Build auch lokal reproduzierbar sein (ohne CI)? | Empfehlung: `build.ps1` im Plugin-Projektordner |

**Entscheidungen**
1. nein
2. SonarQube **9.9 LTS** – die am weitesten verbreitete Version bei OnPrem-Installationen; `<SqMinVersion>9.9</SqMinVersion>` ist bereits im `PluginManifest.xml` hinterlegt.
3. nein
4. alles `CODE_SMELL`
5. ja

---

## Referenzen

- [SonarSource Roslyn SDK (GitHub)](https://github.com/SonarSource/sonarqube-roslyn-sdk)
- [SonarQube Plugin Development – Custom Rules](https://docs.sonarsource.com/sonarqube/latest/extension-guide/adding-coding-rules/)
- [SonarQube Rule Metadata Format](https://docs.sonarsource.com/sonarqube/latest/extension-guide/adding-coding-rules/#implementing-a-rule)
- [Existing nupkg CI job](.github/workflows/ci.yml)
