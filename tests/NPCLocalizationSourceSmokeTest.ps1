$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$localizationPaths = @(
    (Join-Path $repoRoot "Localization\en-US_Mods.ETERNIA.hjson"),
    (Join-Path $repoRoot "en-US.hjson")
)

foreach ($path in $localizationPaths) {
    $content = Get-Content -Raw $path

    if ($content -match "Eternal N P C") {
        throw "$path should not display the guide NPC as 'Eternal N P C'."
    }

    if ($content -notmatch "NPCs\.EternalNPC\.DisplayName:\s*Eternal") {
        throw "$path should localize EternalNPC as Eternal."
    }
}

Write-Host "NPC localization source smoke test passed."
