$ErrorActionPreference = "Stop"

# Second obtention pass: the tier-gating Swordsman swords are tied to the boss/zone
# they belong to (boss drops, Dungeon chest loot, event drop) instead of a soft
# material gate. This pins that wiring to source.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$drops = Get-Content -Raw (Join-Path $c "Globals\SwordsmanDropsGlobalNPC.cs")
$chest = Get-Content -Raw (Join-Path $c "Systems\SwordsmanChestLoot.cs")
$dreadReaver = Get-Content -Raw (Join-Path $c "Items\Weapons\Warrior\DreadReaver.cs")
$bonewarden = Get-Content -Raw (Join-Path $c "Items\Weapons\Warrior\BonewardenSabre.cs")

# --- Boss / event drops ------------------------------------------------------
if ($drops -notmatch "class SwordsmanDropsGlobalNPC\s*:\s*GlobalNPC" -or
    $drops -notmatch "ModifyNPCLoot" -or
    $drops -notmatch "ItemDropRule\.Common") {
    throw "SwordsmanDropsGlobalNPC should add drop rules in ModifyNPCLoot."
}

$expectedDrops = @{
    "NPCID.EyeofCthulhu" = "DreadReaver"     # boss drop (Eye of Cthulhu tier)
    "NPCID.QueenBee"     = "Thornrender"      # boss drop (Queen Bee tier)
    "NPCID.Mothron"      = "CrimsonRequiem"   # event drop (Solar Eclipse)
    "NPCID.CursedSkull"  = "BonewardenSabre"  # Dungeon undead trickle
}

foreach ($pair in $expectedDrops.GetEnumerator()) {
    if ($drops -notmatch [regex]::Escape($pair.Key)) {
        throw "SwordsmanDropsGlobalNPC should drop from $($pair.Key)."
    }
    if ($drops -notmatch [regex]::Escape($pair.Value)) {
        throw "SwordsmanDropsGlobalNPC should drop $($pair.Value)."
    }
}

# --- Dungeon chest loot ------------------------------------------------------
if ($chest -notmatch "PostWorldGen" -or
    $chest -notmatch "wallDungeon" -or
    $chest -notmatch "BonewardenSabre") {
    throw "SwordsmanChestLoot should seed BonewardenSabre into Dungeon chests at world gen."
}

# --- The drop/chest swords must NOT be craftable (soft gate removed) ---------
if ($dreadReaver -match "CreateRecipe\(\)") {
    throw "DreadReaver must be an Eye of Cthulhu drop only, not craftable."
}

if ($bonewarden -match "CreateRecipe\(\)") {
    throw "BonewardenSabre must be Dungeon loot only, not craftable."
}

Write-Host "Swordsman obtention source smoke test passed."
