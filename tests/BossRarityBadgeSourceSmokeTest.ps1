$ErrorActionPreference = "Stop"

# The rarity/level badge is skipped for Common enemies, but bosses roll Common
# most of the time (BossProfiles), so they almost never showed a badge. A boss is
# always significant, so it must ALWAYS display its rarity + level; only non-boss
# Common enemies stay unlabeled.

$repoRoot = Split-Path -Parent $PSScriptRoot
$content = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\EterniaGlobalNPC.cs")

if ($content -notmatch "EnemyRarity\.Common\s*&&\s*!npc\.boss") {
    throw "Bosses must not fall into the minimal Common level tag; they always show the full rarity badge."
}

Write-Host "Boss rarity badge source smoke test passed."
