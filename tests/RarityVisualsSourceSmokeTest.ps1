$ErrorActionPreference = "Stop"

# Rare enemies should feel threatening: a rarity-colored aura that pulses and
# scales with rarity, plus a dramatic badge. Guard that the visual layers exist.

$repoRoot = Split-Path -Parent $PSScriptRoot
$content = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\EterniaGlobalNPC.cs")

if ($content -notmatch "public override bool PreDraw\(") {
    throw "EterniaGlobalNPC should draw a rarity aura in PreDraw so rare enemies stand out."
}

if ($content -notmatch "GetRarityIntensity") {
    throw "Rarity visuals should scale with a per-rarity intensity (GetRarityIntensity)."
}

if ($content -notmatch "GlobalTimeWrappedHourly") {
    throw "Rarity visuals should pulse over time."
}

Write-Host "Rarity visuals source smoke test passed."
