$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$path = Join-Path $repoRoot "Content\Globals\EnemyDebuffDisplayGlobalNPC.cs"

if (-not (Test-Path $path)) {
    throw "EnemyDebuffDisplayGlobalNPC.cs is missing."
}

$c = Get-Content -Raw $path

# --- Draws an enemy's debuffs as icons above it ------------------------------

if ($c -notmatch "class EnemyDebuffDisplayGlobalNPC\s*:\s*GlobalNPC") {
    throw "The debuff display should be a GlobalNPC."
}

if ($c -notmatch "public override void PostDraw") {
    throw "Debuff icons should be drawn over the enemy in PostDraw."
}

if ($c -notmatch "npc\.buffType" -or $c -notmatch "npc\.buffTime") {
    throw "It should read the enemy's active buffs."
}

if ($c -notmatch "TextureAssets\.Buff\[") {
    throw "It should draw the real buff icon texture."
}

# --- Only DEBUFFS, so an enemy's own buffs never clutter it ------------------

if ($c -notmatch "Main\.debuff\[") {
    throw "Only harmful debuffs should be shown, not every buff an enemy carries."
}

# --- Doesn't draw on things that shouldn't have an overlay ------------------

foreach ($skip in @("CountsAsACritter", "friendly", "townNPC", "life <= 0")) {
    if ($c -notmatch $skip) {
        throw "The overlay should skip $skip enemies."
    }
}

# --- Bounded so a heavily-debuffed enemy doesn't draw a mile-long row --------

if ($c -notmatch "MaxIcons") {
    throw "The icon row should be capped."
}

Write-Host "Enemy debuff display source smoke test passed."
