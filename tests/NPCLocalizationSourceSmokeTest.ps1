$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$localizationPaths = @(
    (Join-Path $repoRoot "en-US.hjson")
)

foreach ($path in $localizationPaths) {
    $content = Get-Content -Raw $path

    if ($content -match "Eternal N P C") {
        throw "$path should not display the guide NPC as 'Eternal N P C'."
    }

    if ($content -notmatch "(?ms)EternalNPC:\s*\{[^}]*DisplayName:\s*Eternal") {
        throw "$path should localize EternalNPC as Eternal."
    }
}

Write-Host "NPC localization source smoke test passed."
