$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$description = Get-Content -Raw (Join-Path $repoRoot "description.txt")
$workshop = Get-Content -Raw (Join-Path $repoRoot "description_workshop.txt")
$build = Get-Content -Raw (Join-Path $repoRoot "build.txt")

foreach ($pair in @(
    @{Name="description.txt"; Content=$description},
    @{Name="description_workshop.txt"; Content=$workshop})) {
    if ($pair.Content -match "Modify this file|This description will show up") {
        throw "$($pair.Name) should not contain the default tModLoader template text."
    }

    foreach ($term in @("Soul", "Warrior", "Mage", "Ranger", "Summoner", "promotion")) {
        if ($pair.Content -notmatch $term) {
            throw "$($pair.Name) should describe the core Eternia class/Soul loop and mention '$term'."
        }
    }
}

if ($build -notmatch "displayName = Eternia" -or
    $build -notmatch "version = 0\.1") {
    throw "build.txt should keep the current Eternia display name and version metadata."
}

Write-Host "Mod metadata source smoke test passed."
