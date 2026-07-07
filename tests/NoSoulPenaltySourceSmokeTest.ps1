$ErrorActionPreference = "Stop"

# Design (2026-07-06): a body always needs a Soul. The no-Soul penalty must fire
# whenever NO Soul is equipped (ActiveSoul == None / !HasAnySoul), regardless of
# what is sitting in the inventory. Equipping any Soul (Empty or Class) removes it.
# The penalty debuff shown is SoulLessDebuff ("Alma Perdida"); the old redundant
# NoSoulDebuff is removed.

$repoRoot = Split-Path -Parent $PSScriptRoot
$playerPath = Join-Path $repoRoot "Content\Players\EterniaPlayer.cs"
$player = Get-Content -Raw $playerPath

# 1. The penalty must be gated on "no Soul equipped", not on owning a class Soul item.
if ($player -notmatch "(?s)if\s*\(\s*!HasAnySoul\s*\)\s*\{\s*ApplyNoSoulPenalty\(\)") {
    throw "EterniaPlayer must apply the no-Soul penalty whenever no Soul is equipped (!HasAnySoul)."
}

# 2. It must NOT gate the penalty behind possessing a class Soul in the inventory.
if ($player -match "HasClassSoulAvailable") {
    throw "The no-Soul penalty must no longer depend on HasClassSoulAvailable/inventory scans."
}

# 3. The applied debuff must be SoulLessDebuff (Alma Perdida), not the old NoSoulDebuff.
if ($player -notmatch "BuffType<\s*SoulLessDebuff\s*>") {
    throw "ApplyNoSoulPenalty must add SoulLessDebuff (the 'Alma Perdida' debuff)."
}

if ($player -match "BuffType<\s*NoSoulDebuff\s*>") {
    throw "EterniaPlayer must not apply the removed NoSoulDebuff."
}

# 4. The redundant orphan buff class must be gone; SoulLessDebuff must remain.
$noSoulPath = Join-Path $repoRoot "Content\Buffs\NoSoulDebuff.cs"
$soulLessPath = Join-Path $repoRoot "Content\Buffs\SoulLessDebuff.cs"

if (Test-Path $noSoulPath) {
    throw "Redundant NoSoulDebuff.cs should be removed; SoulLessDebuff is now the applied penalty buff."
}

if (!(Test-Path $soulLessPath)) {
    throw "SoulLessDebuff.cs must exist as the applied no-Soul penalty buff."
}

Write-Host "No-Soul penalty source smoke test passed."
