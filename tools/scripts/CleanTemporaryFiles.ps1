[CmdletBinding()]
param (
    [Parameter()]
    [switch]
    $DontRemoveVsData,

    [Parameter()]
    [string]
    $Root = (Resolve-Path -Path "$PSScriptRoot\..\..")
)

Write-Host "Cleaning repository from temporary files"
Write-Host "################################################################################"

Write-Host "Removing '/build' directory"
$buildDirectory = Join-Path $Root "build"
Remove-Item $buildDirectory -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "################################################################################"

Write-Host "Removing 'bin' and 'obj' directories"
Get-ChildItem -Directory -Recurse -Include bin, obj | ForEach-Object {
    $directory = $_.FullName
    Write-Host "- removing '$directory'"
    Remove-Item -Recurse $directory -Force
}
Write-Host "################################################################################"

Write-Host "Removing 'TestResults' directories"
Get-ChildItem -Directory -Recurse -Include TestResults | ForEach-Object {
    $directory = $_.FullName
    Write-Host "- removing '$directory'"
    Remove-Item -Recurse $directory -Force
}
Write-Host "################################################################################"

Write-Host "Removing 'packages.lock.json' files"
Get-ChildItem -File -Recurse -Include "packages.lock.json" | ForEach-Object {
    $file = $_.FullName
    Write-Host "- removing '$file'"
    Remove-Item $file -Force
}
Write-Host "################################################################################"

if (-not $DontRemoveVsData)
{
    Write-Host "Removing '.vs' directories"
    Get-ChildItem -Directory -Hidden -Recurse -Include ".vs" | ForEach-Object {
        $directory = $_.FullName
        Write-Host "- removing '$directory'"
        Remove-Item -Recurse $directory -Force
    }
}
else {
    Write-Host "Leaving '.vs' directories untouched as requested by the user"
}
Write-Host "################################################################################"

Write-Host "Finished"