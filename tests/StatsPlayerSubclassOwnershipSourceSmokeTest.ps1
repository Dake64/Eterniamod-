$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$statsPath = Join-Path $repoRoot "Content\Players\EterniaStatsPlayer.cs"
$promotionRulesPath = Join-Path $repoRoot "Content\Progression\ClassPromotionRules.cs"

$stats = Get-Content -Raw $statsPath
$promotionRules = Get-Content -Raw $promotionRulesPath

if ($stats -match "GetWarriorSubclass" -or
    $stats -match "GetRangerSubclass" -or
    $stats -match "GetMageSubclass" -or
    $stats -match "GetSummonerSubclass") {
    throw "EterniaStatsPlayer should not own subclass resolution; ClassPromotionRules is the authoritative promotion service."
}

if ($stats -match '"Gunslinger"') {
    throw "Stale subclass name 'Gunslinger' should not remain in stats-owned promotion logic."
}

if ($promotionRules -notmatch "ResolveSubclass") {
    throw "ClassPromotionRules should remain the central subclass resolver."
}

Write-Host "StatsPlayer subclass ownership source smoke test passed."
