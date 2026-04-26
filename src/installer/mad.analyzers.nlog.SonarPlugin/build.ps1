<#
.SYNOPSIS
    Builds the SonarQube plugin (.jar) for mad.analyzers.nlog locally.

.DESCRIPTION
    Uses the SonarSource Roslyn SDK to generate a SonarQube-compatible plugin JAR
    from the published NuGet package.

    Prerequisites:
      - Java JDK 11+ on PATH (for Maven-based plugin build)
      - Maven 3.x on PATH  -OR-  Docker (to run the build in a container)
      - The NuGet package mad.analyzers.nlog must be available on NuGet.org
        or reachable via the configured NuGet source.

.PARAMETER Version
    The package version to embed in the plugin (defaults to the version in
    PluginManifest.xml – 0.6.0).

.PARAMETER SdkDir
    Directory where the SonarSource Roslyn SDK repository is (or will be) cloned.
    Defaults to <repo-root>/../sonarqube-roslyn-sdk.

.PARAMETER OutDir
    Output directory for the generated .jar file.
    Defaults to <repo-root>/build/sonar-plugin/.

.EXAMPLE
    # Build using the defaults
    .\build.ps1

.EXAMPLE
    # Build a specific version
    .\build.ps1 -Version 0.7.0
#>
[CmdletBinding()]
param(
    [string]$Version        = '1.0.0',
    [string]$SdkDir         = 'C:\Tools\SonarQube.Roslyn.SDK',
    [string]$OutDir         = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ---------------------------------------------------------------------------
# Resolve paths
# ---------------------------------------------------------------------------
$repoRoot     = Resolve-Path "$PSScriptRoot\..\..\..\..\"
$manifestFile = Join-Path $PSScriptRoot 'PluginManifest.xml'
$rulesDir     = Join-Path $PSScriptRoot 'rules'

if (-not $SdkDir) {
    $SdkDir = Join-Path (Split-Path $repoRoot -Parent) 'sonarqube-roslyn-sdk'
}
if (-not $OutDir) {
    $OutDir = Join-Path $repoRoot 'build\sonar-plugin'
}

$OutDir = [System.IO.Path]::GetFullPath($OutDir)
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

Write-Host "=== SonarQube Plugin Build ===" -ForegroundColor Cyan
Write-Host "  Version     : $Version"
Write-Host "  SDK dir     : $SdkDir"
Write-Host "  Output dir  : $OutDir"
Write-Host ""

# ---------------------------------------------------------------------------
# Step 1 – Ensure the Roslyn SDK is available
# ---------------------------------------------------------------------------
if (-not (Test-Path $SdkDir)) {
    throw "SonarQube Roslyn SDK not found at '$SdkDir', please clone or download from 'https://github.com/SonarSource/sonarqube-roslyn-sdk'"
}

# ---------------------------------------------------------------------------
# Step 2 – Build the SDK (once; skip if generator exe already exists)
# ---------------------------------------------------------------------------
$generatorExe = Join-Path $SdkDir 'RoslynSonarQubePluginGenerator.exe'
if (-not (Test-Path $generatorExe)) {
    throw "RoslynSonarQubePluginGenerator.exe not found in SDK directory. Please provide the sdk executables at path '$SdkDir'"
}

# ---------------------------------------------------------------------------
# Step 3 – Patch manifest version
# ---------------------------------------------------------------------------
$manifestXml = [xml](Get-Content $manifestFile)
$manifestXml.AnalyzerPlugin.Version        = $Version
$manifestXml.AnalyzerPlugin.PackageVersion = $Version
$patchedManifest = Join-Path $OutDir 'PluginManifest.xml'
$manifestXml.Save($patchedManifest)
Write-Host "Patched manifest written to $patchedManifest"

# ---------------------------------------------------------------------------
# Step 4 – Run the plugin generator
# ---------------------------------------------------------------------------
Write-Host "Running RoslynSonarQubePluginGenerator..." -ForegroundColor Yellow
& $generatorExe `
    /analyzer:"mad.analyzers.nlog:$Version" `
    /ouputdir:"$OutDir" `
    /language:cs `
    /customnugetrepo:'C:\Users\maxim\.feeds\nuget' `
    /recurse

if ($LASTEXITCODE -ne 0) {
    throw "RoslynSonarQubePluginGenerator exited with code $LASTEXITCODE"
}

# ---------------------------------------------------------------------------
# Done
# ---------------------------------------------------------------------------
$jar = Get-ChildItem -Path $OutDir -Filter '*.jar' | Select-Object -First 1
if ($jar) {
    Write-Host ""
    Write-Host "Plugin built successfully: $($jar.FullName)" -ForegroundColor Green
} else {
    Write-Warning "Build completed but no .jar was found in $OutDir"
}
