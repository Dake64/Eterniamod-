$ErrorActionPreference = "Stop"

# Second obtention pass for the Peleador fist line: the tier fists are tied to the
# boss / zone / event they belong to (boss drops, underground chest loot, event drop)
# instead of a plain material gate. This pins that wiring to source.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$drops = Get-Content -Raw (Join-Path $c "Globals\FighterDropsGlobalNPC.cs")
$chest = Get-Content -Raw (Join-Path $c "Systems\FighterChestLoot.cs")
$prospector = Get-Content -Raw (Join-Path $c "Items\Weapons\Fighter\ProspectorsGauntlets.cs")
$bloodfeast = Get-Content -Raw (Join-Path $c "Items\Weapons\Fighter\BloodfeastGauntlets.cs")

# --- Boss / event / trickle drops --------------------------------------------
if ($drops -notmatch "class FighterDropsGlobalNPC\s*:\s*GlobalNPC" -or
    $drops -notmatch "ModifyNPCLoot" -or
    $drops -notmatch "ItemDropRule\.Common") {
    throw "FighterDropsGlobalNPC should add drop rules in ModifyNPCLoot."
}

$expectedDrops = @{
    "NPCID.BrainofCthulhu"    = "BloodfeastGauntlets"    # evil boss (drop-only)
    "NPCID.EaterofWorldsHead" = "BloodfeastGauntlets"    # evil boss (drop-only)
    "NPCID.QueenBee"          = "ThornfistGauntlets"     # jungle boss (bonus)
    "NPCID.Mothron"           = "NightfallGauntlets"     # Solar Eclipse (bonus)
    "NPCID.Plantera"          = "PlaguebringerFists"     # boss (bonus)
    "NPCID.UndeadMiner"       = "ProspectorsGauntlets"   # cavern trickle
}

foreach ($pair in $expectedDrops.GetEnumerator()) {
    if ($drops -notmatch [regex]::Escape($pair.Key)) {
        throw "FighterDropsGlobalNPC should drop from $($pair.Key)."
    }
    if ($drops -notmatch [regex]::Escape($pair.Value)) {
        throw "FighterDropsGlobalNPC should drop $($pair.Value)."
    }
}

# --- Underground chest loot --------------------------------------------------
if ($chest -notmatch "PostWorldGen" -or
    $chest -notmatch "worldSurface" -or
    $chest -notmatch "ProspectorsGauntlets") {
    throw "FighterChestLoot should seed ProspectorsGauntlets into underground chests at world gen."
}

# It must stay OUT of the Dungeon (that space is the Swordsman's Bonewarden Sabre).
if ($chest -notmatch "wallDungeon") {
    throw "FighterChestLoot should exclude Dungeon chests (reserved for the Swordsman)."
}

# --- The drop/chest fists must NOT be craftable (soft gate removed) ----------
if ($prospector -match "CreateRecipe\(\)") {
    throw "ProspectorsGauntlets must be chest/Undead-Miner loot only, not craftable."
}

if ($bloodfeast -match "CreateRecipe\(\)") {
    throw "BloodfeastGauntlets must be an evil-boss drop only, not craftable."
}

Write-Host "Fighter obtention source smoke test passed."
