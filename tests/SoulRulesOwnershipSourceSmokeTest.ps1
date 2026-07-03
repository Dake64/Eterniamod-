$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$soulRulesPath = Join-Path $repoRoot "Content\Souls\SoulRules.cs"
$promotionRulesPath = Join-Path $repoRoot "Content\Progression\ClassPromotionRules.cs"

$soulRules = Get-Content -Raw $soulRulesPath
$promotionRules = Get-Content -Raw $promotionRulesPath

if ($soulRules -match "GetSubclassName") {
    throw "SoulRules should not duplicate base subclass naming; use ClassPromotionRules.GetBaseClassName/ResolveSubclass."
}

if ($promotionRules -notmatch "GetBaseClassName\(SoulId activeSoul\)") {
    throw "ClassPromotionRules should own base class naming."
}

Write-Host "SoulRules ownership source smoke test passed."
