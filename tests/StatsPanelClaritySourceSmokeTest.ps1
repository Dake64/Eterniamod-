$ErrorActionPreference = "Stop"

# The stats panel used to show only the PER-POINT rate ("+0.3% all damage"), never
# what your invested points were actually giving you. It now shows:
#  - the CURRENT total effect per stat,
#  - the per-point rate as secondary text,
#  - a "Next point:" preview in the tooltip,
#  - EXP progress toward the next level,
#  - a pulse on unspent Stats/Passives points.

$repoRoot = Split-Path -Parent $PSScriptRoot
$stats = Get-Content -Raw (Join-Path $repoRoot "Content\UI\StatsUI.cs")

if ($stats -notmatch "private static string CurrentEffect\(") {
    throw "StatsUI should compute the CURRENT total effect of each stat."
}

foreach ($token in @("Now: ", "Per point: ", "Next point: ")) {
    if ($stats -notmatch [regex]::Escape($token)) {
        throw "StatsUI tooltip should show '$token' so the player sees value and the next-point preview."
    }
}

if ($stats -notmatch "Not invested yet") {
    throw "StatsUI should read clearly when a stat has no points invested."
}

# EXP progress lives in the panel where points are spent.
if ($stats -notmatch "expToNextLevel" -or
    $stats -notmatch "currentExp" -or
    $stats -notmatch "EterniaUI\.DrawProgressBar") {
    throw "StatsUI should show EXP progress toward the next level."
}

# Unspent points are visually surfaced.
if ($stats -notmatch "unspent") {
    throw "StatsUI should highlight unspent Stats/Passives points."
}

# The panel's math must mirror EterniaStatsPlayer (guard against silent drift).
$player = Get-Content -Raw (Join-Path $repoRoot "Content\Players\EterniaStatsPlayer.cs")

foreach ($pair in @(
    @{ Stat = "Vitality"; Rate = "Vitality \* 3" },
    @{ Stat = "Power"; Rate = "Power \* 0\.003f" },
    @{ Stat = "Precision"; Rate = "Precision \* 0\.15f" },
    @{ Stat = "Agility"; Rate = "Agility \* 0\.005f" },
    @{ Stat = "Focus"; Rate = "Focus \* 3" })) {
    if ($player -notmatch $pair.Rate) {
        throw "EterniaStatsPlayer changed its $($pair.Stat) math; update StatsUI.CurrentEffect to match."
    }
}

Write-Host "Stats panel clarity source smoke test passed."
