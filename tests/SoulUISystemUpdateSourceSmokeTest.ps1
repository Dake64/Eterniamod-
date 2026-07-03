$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$systemPath = Join-Path $repoRoot "Content\UI\SoulUISystem.cs"
$content = Get-Content -Raw $systemPath

if ($content -notmatch "ExpInterface\?\.Update\(gameTime\);") {
    throw "SoulUISystem should update the EXP interface every UI tick."
}

if ($content -match '(?s)if\s*\(\s*Visible\s*\)\s*\{[^}]*ExpInterface\?\.Update\(gameTime\);') {
    throw "EXP interface updates should not depend on the Soul panel being visible."
}

if ($content -notmatch '(?s)if\s*\(\s*Visible\s*\)\s*\{\s*SoulInterface\?\.Update\(gameTime\);') {
    throw "Soul panel updates should stay gated behind the Soul panel visibility toggle."
}

if ($content -notmatch "int insertIndex = mouseTextIndex;") {
    throw "SoulUISystem should use an incrementing insertIndex so layer order matches source order."
}

if ($content -notmatch "layers\.Insert\(insertIndex\+\+") {
    throw "SoulUISystem layer inserts should increment insertIndex after each insertion."
}

if ($content -notmatch "static void CloseSoulPanel\(") {
    throw "SoulUISystem should expose CloseSoulPanel for UI-driven closing."
}

if ($content -notmatch "SoulInterface\?\.SetState\(null\);") {
    throw "CloseSoulPanel should clear the SoulInterface state when closing."
}

Write-Host "Soul UI system update source smoke test passed."
