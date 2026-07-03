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

if ($content -notmatch "npc\.lastInteraction" -or
    $content -notmatch "AddExperience\(exp\)") {
    throw "EXP attribution should remain based on the last interacting player and AddExperience gate."
}

Write-Host "Enemy rarity source smoke test passed."
