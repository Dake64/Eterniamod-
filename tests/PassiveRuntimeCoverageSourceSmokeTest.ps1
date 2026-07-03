$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$registryPath = Join-Path $repoRoot "Content\Passives\PassiveRegistry.cs"
$contentRoot = Join-Path $repoRoot "Content"

$registry = Get-Content -Raw $registryPath
$runtimeSource =
    (Get-ChildItem -Path $contentRoot -Recurse -File -Filter *.cs |
        Where-Object { $_.FullName -notlike "*\Content\Passives\PassiveRegistry.cs" } |
        ForEach-Object { Get-Content -Raw $_.FullName }) -join "`n"

$passiveNames =
    [regex]::Matches($registry, 'new PassiveNode\(\s*"([^"]+)"') |
    ForEach-Object { $_.Groups[1].Value } |
    Sort-Object -Unique

if ($passiveNames.Count -lt 1) {
    throw "PassiveRegistry should define at least one passive."
}

foreach ($passive in $passiveNames) {
    $escaped =
        [regex]::Escape($passive)

    if ($runtimeSource -notmatch "HasActivePassive\([^)]*`"$escaped`"") {
        throw "Passive '$passive' is registered but has no runtime HasActivePassive effect."
    }
}

Write-Host "Passive runtime coverage source smoke test passed."
