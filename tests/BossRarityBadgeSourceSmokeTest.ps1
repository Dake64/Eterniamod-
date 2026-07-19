$ErrorActionPreference = "Stop"

# --- The rarity banner may only fire for a boss that ACTUALLY spawned ----------
# Playtest 2026-07-16: "RARE" appeared on screen with no boss anywhere. The announcement
# was made from SetDefaults, which tModLoader also runs on the SAMPLE instance it builds
# for every NPC type (ContentSamples/bestiary) -- those never enter the world.
$globalNpc = Get-Content -Raw (Join-Path (Split-Path -Parent $PSScriptRoot) "Content\Globals\EterniaGlobalNPC.cs")

$setDefaults = [regex]::Match(
    $globalNpc,
    'public override void SetDefaults\(NPC npc\)[\s\S]+?\n\s{8}\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($setDefaults -match "AnnounceNotableBoss") {
    throw "Never announce from SetDefaults: it also runs on sample NPCs that never spawn."
}

if ($globalNpc -notmatch "public override void OnSpawn\(NPC npc, IEntitySource source\)") {
    throw "The rarity announcement should hang off OnSpawn, which only runs for real spawns."
}

$announce = [regex]::Match(
    $globalNpc,
    'private void AnnounceNotableBoss\(NPC npc\)[\s\S]+?\n\s{8}\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

# A real world NPC occupies its own slot; a sample instance does not.
if ($announce -notmatch "Main\.npc\[npc\.whoAmI\] != npc") {
    throw "The announcement should verify the NPC really occupies a world slot."
}

if ($announce -notmatch "npc\.boss" -or $announce -notmatch "announced") {
    throw "The announcement should be boss-only and fire once per spawn."
}


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
