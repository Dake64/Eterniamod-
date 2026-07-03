$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiPath = Join-Path $repoRoot "Content\UI\ClassProgressionUI.cs"
$rulesPath = Join-Path $repoRoot "Content\Progression\ClassPromotionRules.cs"

if (!(Test-Path $uiPath)) {
    throw "Missing required source file: $uiPath"
}

$ui = Get-Content -Raw $uiPath
$rules = Get-Content -Raw $rulesPath

if ($rules -notmatch "GetDominantAffinityName\(") {
    throw "ClassPromotionRules should expose GetDominantAffinityName(...)."
}

if ($rules -notmatch "GetDominantAffinityValue\(") {
    throw "ClassPromotionRules should expose GetDominantAffinityValue(...)."
}

if ($ui -notmatch "Eternia: Class Progression UI") {
    throw "ClassProgressionUI should register a named interface layer."
}

foreach ($requiredCall in @(
    "ClassPromotionRules.GetBaseClassName",
    "ClassPromotionRules.GetDominantAffinityName",
    "ClassPromotionRules.GetDominantAffinityValue",
    "ClassPromotionRules.IsPromotionForSoul",
    "GetModPlayer<SubclassPlayer>()",
    "GetLockedPromotion",
    "GetModPlayer<EterniaStatsPlayer>()"
)) {
    if ($ui -notmatch [regex]::Escape($requiredCall)) {
        throw "ClassProgressionUI should call '$requiredCall'."
    }
}

foreach ($label in @("BASE CLASS", "PROMOTION WAITING", "PROMOTED", "PATH LOCKED", "Locked:")) {
    if ($ui -notmatch [regex]::Escape($label)) {
        throw "ClassProgressionUI should draw status '$label'."
    }
}

if ($ui -notmatch "GetPromotionDetail") {
    throw "ClassProgressionUI should centralize locked-vs-dominant detail text."
}

Write-Host "Class progression UI source smoke test passed."
