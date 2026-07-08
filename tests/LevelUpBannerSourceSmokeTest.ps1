$ErrorActionPreference = "Stop"

# Leveling up used to spam three chat messages that cluttered the screen. It should
# instead show a single on-screen banner and leave the chat clean.

$repoRoot = Split-Path -Parent $PSScriptRoot
$bannerPath = Join-Path $repoRoot "Content\UI\LevelUpBannerUI.cs"
$levelPath = Join-Path $repoRoot "Content\Players\EterniaLevelPlayer.cs"

if (!(Test-Path $bannerPath)) {
    throw "Level-up feedback should use an on-screen banner (LevelUpBannerUI)."
}

$level = Get-Content -Raw $levelPath

if ($level -notmatch "LevelUpBannerUI\.Show") {
    throw "LevelUp should trigger the on-screen banner."
}

if ($level -match "Main\.NewText") {
    throw "LevelUp should not spam the chat with level-up messages; use the banner."
}

Write-Host "Level-up banner source smoke test passed."
