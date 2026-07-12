$ErrorActionPreference = "Stop"

# Second obtention pass for the shield line: the tier shields are tied to the boss /
# zone / event they belong to (boss drops, underground chest loot) instead of a plain
# material gate. This pins that wiring to source.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$drops = Get-Content -Raw (Join-Path $c "Globals\ShieldDropsGlobalNPC.cs")
$chest = Get-Content -Raw (Join-Path $c "Systems\ShieldChestLoot.cs")
$corrupt = Get-Content -Raw (Join-Path $c "Items\Weapons\Guardian\CorruptShield.cs")

# --- Boss / event drops ------------------------------------------------------
if ($drops -notmatch "class ShieldDropsGlobalNPC\s*:\s*GlobalNPC" -or
    $drops -notmatch "ModifyNPCLoot" -or
    $drops -notmatch "ItemDropRule\.Common") {
    throw "ShieldDropsGlobalNPC should add drop rules in ModifyNPCLoot."
}

$expectedDrops = @{
    "NPCID.BrainofCthulhu"    = "CorruptShield"       # evil boss (drop-only)
    "NPCID.EaterofWorldsHead" = "CorruptShield"       # evil boss (drop-only, OnKill)
    "NPCID.SkeletronPrime"    = "HallowedBulwark"     # mech boss (bonus)
    "NPCID.Mothron"           = "NightfallBarrier"    # Solar Eclipse (bonus)
    "NPCID.Plantera"          = "PlaguebringerWall"   # boss (bonus)
}

foreach ($pair in $expectedDrops.GetEnumerator()) {
    if ($drops -notmatch [regex]::Escape($pair.Key)) {
        throw "ShieldDropsGlobalNPC should drop from $($pair.Key)."
    }
    if ($drops -notmatch [regex]::Escape($pair.Value)) {
        throw "ShieldDropsGlobalNPC should drop $($pair.Value)."
    }
}

# --- Underground chest loot --------------------------------------------------
if ($chest -notmatch "PostWorldGen" -or
    $chest -notmatch "worldSurface" -or
    $chest -notmatch "GlacialShield") {
    throw "ShieldChestLoot should seed GlacialShield into underground chests at world gen."
}

# It must stay OUT of the Dungeon (reserved for the Swordsman).
if ($chest -notmatch "wallDungeon") {
    throw "ShieldChestLoot should exclude Dungeon chests."
}

# --- The drop shield must NOT be craftable (soft gate removed) ---------------
if ($corrupt -match "CreateRecipe\(\)") {
    throw "CorruptShield must be an evil-boss drop only, not craftable."
}

Write-Host "Shield obtention source smoke test passed."
