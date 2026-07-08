$ErrorActionPreference = "Stop"

# The rarity/level badge is skipped for Common enemies, but bosses roll Common
# most of the time (BossProfiles), so they almost never showed a badge. A boss is
# always significant, so it must ALWAYS display its rarity + level; only non-boss
# Common enemies stay unlabeled.

$repoRoot = Split-Path -Parent $PSScriptRoot
$content = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\EterniaGlobalNPC.cs")

if ($content -notmatch "(?ms)public override void PostDraw\([^)]*\)\s*\{[^}]*npc\.boss") {
    throw "EterniaGlobalNPC.PostDraw should keep drawing the badge for bosses (exempt npc.boss from the Common skip)."
}

Write-Host "Boss rarity badge source smoke test passed."
