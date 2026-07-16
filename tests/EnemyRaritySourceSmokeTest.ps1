$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$globalNpcPath = Join-Path $repoRoot "Content\Globals\EterniaGlobalNPC.cs"
$content = Get-Content -Raw $globalNpcPath

foreach ($required in @(
    "EnemyRarityProfile",
    "NormalProfiles",
    "BossProfiles",
    "ApplyRarityProfile",
    "RollRarityProfile",
    "DrawEnemyBadge")) {
    if ($content -notmatch $required) {
        throw "EterniaGlobalNPC should use structured rarity profile flow: missing $required."
    }
}

if ($content -match "lifeMultiplier\s*=\s*8f" -or
    $content -match "damageMultiplier\s*=\s*4f" -or
    $content -match "defenseMultiplier\s*=\s*4f") {
    throw "Normal enemy rarity scaling should not use old extreme 8x/4x multipliers."
}

if ($content -notmatch "rarity\s*==\s*EnemyRarity\.Common[\s\S]*return;") {
    throw "Common enemies should not draw rarity badges; it creates too much screen clutter."
}

# --- A boss's level must follow world progression ----------------------------
# Found in playtest: bosses roll level 20-100 from rarity alone, so a Legendary King Slime
# (the FIRST boss) came out ~level 60 -> ~+20 defense. Terraria subtracts defense/2 from every
# hit, which floored early gear to 1 damage and made the fight unwinnable.

if ($content -notmatch "BossLevelProgressionScale") {
    throw "A boss's rolled level must be scaled by world progression, or early bosses become immune to early gear."
}

if ($content -notmatch "if \(npc\.boss\)[\s\S]{0,200}BossLevelProgressionScale") {
    throw "The progression scale should apply to the boss's enemyLevel."
}

if ($content -notmatch "Main\.hardMode \? 0\.6f : 0\.2f") {
    throw "Pre-Hardmode bosses should stay low-level; the level ceiling opens up with the world."
}

# --- Boss rarity should be RARE ----------------------------------------------
# It used to be more generous than the normal table (only 32% Common). A boss is a deliberate,
# re-summonable encounter: Common must be the norm.

if ($content -notmatch "0\.005f, EnemyRarity\.Nightmare") {
    throw "Nightmare bosses should be a genuine rarity, not a 2% coin flip."
}

if ($content -notmatch "npc\.lastInteraction" -or
    $content -notmatch "AddExperience\(exp\)") {
    throw "EXP attribution should remain based on the last interacting player and AddExperience gate."
}

Write-Host "Enemy rarity source smoke test passed."
