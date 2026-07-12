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

    # A passive counts as "wired" if it is read directly via HasActivePassive OR via a
    # named wrapper that resolves to it (e.g. ElementalistPlayer.HasElementNode for the
    # Elemental Mastery nodes, whose effect is the switching/surge mechanic).
    if ($runtimeSource -notmatch "HasActivePassive\([^)]*`"$escaped`"" -and
        $runtimeSource -notmatch "HasElementNode\(`"$escaped`"") {
        throw "Passive '$passive' is registered but has no runtime effect (HasActivePassive / HasElementNode)."
    }
}

Write-Host "Passive runtime coverage source smoke test passed."
