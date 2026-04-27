# ADR-002: Umstellen auf Central Package Management (CPM)

**Status:** Accepted  
**Datum:** 2026-04-27

## Kontext

Aktuell werden Paketversionen in jedem `.csproj`-Projekt einzeln als `Version`-Attribut im
`<PackageReference>`-Element gepflegt. Bei 8 Projekten führt das zu:

- **Redundanz:** Gleiche Pakete (z. B. `Microsoft.CodeAnalysis.CSharp`) werden in mehreren
  Dateien mit derselben Version eingetragen.
- **Fehleranfälligkeit:** Ein Versionsupgrade muss manuell in allen betroffenen `.csproj`-Dateien
  nachgezogen werden – Inkonsistenzen schleichen sich leicht ein.
- **SonarQube-Plugin-Bau:** Der `RoslynSonarQubePluginGenerator` setzt eine gepinnte, konsistente
  Roslyn-Version voraus (→ ADR-001). Zentral verwaltete Versionen stellen sicher, dass kein Projekt
  versehentlich eine abweichende Version zieht.

Das .NET SDK unterstützt ab Version **8.0** nativ das **Central Package Management (CPM)**
über die Datei `Directory.Packages.props` im Repository-Root.

## Entscheidung

Alle NuGet-Paketversionen werden aus den einzelnen `.csproj`-Dateien in eine zentrale Datei
`Directory.Packages.props` im Repository-Root ausgelagert. `<PackageReference>`-Elemente in den
Projekten enthalten danach **nur noch das `Include`-Attribut**, keine `Version` mehr.

---

## Umsetzungsschritte

### 1. `Directory.Packages.props` anlegen (Repository-Root)

Neue Datei `Directory.Packages.props` im Root des Repos erstellen:

```xml
<Project>
  <PropertyGroup>
    <!-- Central Package Management aktivieren -->
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup Label="Roslyn / CodeAnalysis">
    <PackageVersion Include="Microsoft.CodeAnalysis.Common"                         Version="4.9.2" />
    <PackageVersion Include="Microsoft.CodeAnalysis.CSharp"                         Version="4.9.2" />
    <PackageVersion Include="Microsoft.CodeAnalysis"                                Version="4.9.2" />
    <PackageVersion Include="Microsoft.CodeAnalysis.Analyzers"                      Version="3.3.4" />
    <PackageVersion Include="Microsoft.CodeAnalysis.CSharp.Workspaces"              Version="4.9.2" />
  </ItemGroup>

  <ItemGroup Label="Roslyn Testing">
    <PackageVersion Include="Microsoft.CodeAnalysis.CSharp.Analyzer.Testing"        Version="1.1.3" />
    <PackageVersion Include="Microsoft.CodeAnalysis.CSharp.CodeFix.Testing"         Version="1.1.3" />
    <PackageVersion Include="Microsoft.CodeAnalysis.CSharp.CodeRefactoring.Testing" Version="1.1.3" />
    <PackageVersion Include="Microsoft.CodeAnalysis.VisualBasic.Analyzer.Testing"   Version="1.1.3" />
    <PackageVersion Include="Microsoft.CodeAnalysis.VisualBasic.CodeFix.Testing"    Version="1.1.3" />
    <PackageVersion Include="Microsoft.CodeAnalysis.VisualBasic.CodeRefactoring.Testing" Version="1.1.3" />
  </ItemGroup>

  <ItemGroup Label="Test infrastructure">
    <PackageVersion Include="Microsoft.NET.Test.Sdk"  Version="18.4.0" />
    <PackageVersion Include="MSTest.TestAdapter"       Version="4.2.1"  />
    <PackageVersion Include="MSTest.TestFramework"     Version="4.2.1"  />
  </ItemGroup>

  <ItemGroup Label="NLog">
    <PackageVersion Include="NLog.Extensions.Logging" Version="6.1.2" />
  </ItemGroup>

  <ItemGroup Label="JSON">
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>

  <ItemGroup Label="Internal packages">
    <PackageVersion Include="mad.analyzers.nlog" Version="1.0.0" />
  </ItemGroup>
</Project>
```

> **Hinweis:** `ManagePackageVersionsCentrally` kann alternativ auch in `Directory.Build.props`
> gesetzt werden – aus Übersichtlichkeitsgründen gehört es aber in `Directory.Packages.props`.

---

### 2. `Version`-Attribute aus allen `.csproj`-Dateien entfernen

In **jeder** der folgenden Dateien muss das `Version="..."`-Attribut aus allen
`<PackageReference>`-Elementen entfernt werden:

| Projekt                                                                   | Datei                                                                  |
|---------------------------------------------------------------------------|------------------------------------------------------------------------|
| `mad.analyzers.common`                                                    | `src/lib/mad.analyzers.common/mad.analyzers.common.csproj`             |
| `mad.analyzers.nlog.analyzer`                                             | `src/analyzer/mad.analyzers.nlog.analyzer/mad.analyzers.nlog.analyzer.csproj` |
| `mad.analyzers.nlog.CodeFixes`                                            | `src/codefixes/mad.analyzers.nlog.CodeFixes/mad.analyzers.nlog.CodeFixes.csproj` |
| `mad.analyzers.nlog.Test`                                                 | `src/test/mad.analyzers.nlog.Test/mad.analyzers.nlog.Test.csproj`      |
| `mad.analyzers.nlog.demo`                                                 | `src/demo/mad.analyzers.nlog.demo/mad.analyzers.nlog.demo.csproj`      |
| `mad.analyzers.nlog.Package`                                              | `src/package/mad.analyzers.nlog.Package/mad.analyzers.nlog.Package.csproj` |
| `mad.analyzers.nlog.SonarPlugin`                                          | `src/installer/mad.analyzers.nlog.SonarPlugin/mad.analyzers.nlog.SonarPlugin.csproj` |
| `TestDataGen`                                                             | `tools/testdatagen/TestDataGen/TestDataGen.csproj`                     |

**Vorher (Beispiel):**
```xml
<PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.9.2" />
```

**Nachher:**
```xml
<PackageReference Include="Microsoft.CodeAnalysis.CSharp" />
```

---

### 3. `packages.lock.json`-Dateien neu generieren

Da CPM die interne Restore-Logik ändert, müssen alle Lock-Dateien neu erzeugt werden:

```powershell
dotnet restore --force-evaluate
```

Betroffen sind alle Projekte, die `RestorePackagesWithLockFile=true` erben
(gesetzt in `Directory.Build.props`).

---

### 4. Build-Validierung

```powershell
dotnet build .\mad.analyzers.sln --configuration Release
dotnet test  .\mad.analyzers.sln --configuration Release
```

Beide Befehle müssen fehlerfrei durchlaufen.

---

## Sonderfall: `VersionOverride`

Sollte ein einzelnes Projekt ausnahmsweise eine abweichende Version eines Pakets benötigen,
kann CPM lokal übersteuert werden:

```xml
<!-- Nur in dem einen Projekt, das eine andere Version braucht -->
<PackageReference Include="Newtonsoft.Json" VersionOverride="13.0.1" />
```

Dieses Vorgehen soll die **Ausnahme** bleiben und muss im zugehörigen `.csproj` kommentiert
werden.

---

## Risiken und Konsequenzen

| Risiko                                                                          | Bewertung    | Maßnahme                                                                   |
|---------------------------------------------------------------------------------|--------------|----------------------------------------------------------------------------|
| Paket wird in `Directory.Packages.props` vergessen → Build-Fehler              | Gering       | Compiler/Restore bricht sofort ab, leicht zu beheben                       |
| `packages.lock.json` veraltet → Restore schlägt fehl in CI                     | Mittel       | `dotnet restore --force-evaluate` nach Umstellung ausführen                |
| `VersionOverride` wird zu häufig genutzt → Zweck von CPM wird ausgehebelt      | Gering       | Code-Review-Richtlinie: `VersionOverride` nur mit Begründung              |
| Tools/IDEs, die CPM nicht kennen, zeigen fälschlich fehlende Versionen an      | Gering       | VS 2022 17.4+ und Rider 2023.2+ unterstützen CPM vollständig              |

## Alternativen (verworfen)

- **`Directory.Build.props` mit `<PackageReference Update>`:** Funktioniert, ist aber kein
  offizieller CPM-Mechanismus – schlechter tooling-Support, keine Validierung durch den SDK.
- **Manuelle Disziplin ohne zentrales Management:** Nicht skalierbar, fehleranfällig (aktuelle
  Situation).
- **Global.json + NuGet.config nur mit Feeds:** Löst das Versionsproblem nicht.

## Referenzen

- [Microsoft Docs – Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
- [NuGet CPM GitHub Spec](https://github.com/NuGet/Home/wiki/Centrally-managing-NuGet-package-versions)
- ADR-001: Downgrade von Roslyn 5.3.0 auf 4.9.0 für SonarQube-Plugin-Kompatibilität
