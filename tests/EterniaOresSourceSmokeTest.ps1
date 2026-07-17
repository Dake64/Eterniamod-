$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

$base = Read-File "Content\Tiles\Ores\EterniaOreTile.cs"
$gen = Read-File "Content\Systems\EterniaOreGeneration.cs"
$loc = Read-File "en-US.hjson"

# --- They behave like real ores ----------------------------------------------

foreach ($flag in @("TileID\.Sets\.Ore", "tileSpelunker", "tileOreFinderPriority", "MinPick", "RegisterItemDrop")) {
    if ($base -notmatch $flag) {
        throw "EterniaOreTile should behave like a vanilla ore (missing $flag)."
    }
}

# --- They actually exist in the world ----------------------------------------
# An ore with no worldgen pass is unobtainable -- the whole point is that you MINE them.

if ($gen -notmatch "ModifyWorldGenTasks" -or $gen -notmatch "WorldGen\.TileRunner") {
    throw "The ores must be generated into the world (TileRunner pass)."
}

if ($gen -notmatch '"Shinies"') {
    throw "The ore pass should be slotted next to vanilla's own ore pass."
}

# --- Three tiers, gated by pickaxe power, at three depths --------------------
# This is the point: they fill the early gap between the start of the game and Hellstone.

$tiers = @{
    "SoulstoneOreTile" = 40
    "AnimiteOreTile"   = 55
    "ReveniteOreTile"  = 65
}

foreach ($tile in $tiers.Keys) {
    $t = Read-File "Content\Tiles\Ores\$tile.cs"

    if ($t -notmatch "EterniaOreTile") {
        throw "$tile should extend the shared EterniaOreTile."
    }

    if ($t -notmatch "MinPickPower => $($tiers[$tile])") {
        throw "$tile should require pickaxe power $($tiers[$tile]) -- the tiers are what make it a ladder."
    }

    if ($gen -notmatch "$tile") {
        throw "$tile should be seeded by the worldgen pass."
    }
}

# --- Ore -> bar, or the ore is a dead end -----------------------------------

foreach ($pair in @(
    @("SoulstoneOre", "SoulstoneBar"),
    @("AnimiteOre", "AnimiteBar"),
    @("ReveniteOre", "ReveniteBar"))) {

    $ore = Read-File "Content\Items\Placeable\$($pair[0]).cs"
    if ($ore -notmatch "EterniaOreItem") {
        throw "$($pair[0]) should extend EterniaOreItem."
    }

    $bar = Read-File "Content\Items\Materials\$($pair[1]).cs"
    if ($bar -notmatch "AddIngredient<$($pair[0])>" -or $bar -notmatch "TileID\.Furnaces") {
        throw "$($pair[1]) should be smelted from $($pair[0]) at a Furnace."
    }

    foreach ($item in $pair) {
        if ($loc -notmatch "(?ms)\b${item}:\s*\{[^}]*DisplayName:" -and $loc -notmatch "${item}\.DisplayName:") {
            throw "en-US.hjson should localize $item."
        }
    }
}

# Map entries, or they show up as "Soulstone Ore Tile" on the map.
foreach ($tile in $tiers.Keys) {
    if ($loc -notmatch "$tile\.MapEntry") {
        throw "en-US.hjson should name $tile on the map."
    }
}

# --- Each bar must have gear to make, or the ore is a dead end ---------------
# The whole reason these ores exist is to fill the early gap where Eternia had no armour of
# its own. A bar with nothing to craft is the Soul Alloy trap all over again.

foreach ($set in @(
    @("Soulstone", "SoulstoneBar"),
    @("Animite", "AnimiteBar"),
    @("Revenite", "ReveniteBar"))) {

    $armor = Read-File "Content\Items\Armor\$($set[0])Set.cs"

    if ($armor -notmatch "AddIngredient<$($set[1])>") {
        throw "$($set[0]) armour should be crafted from $($set[1]) -- otherwise the ore has no purpose."
    }

    # Class-agnostic by design: the metal answers to whatever Soul you carry, so three early
    # sets cover all four classes instead of needing twelve.
    if ($armor -notmatch "SoulMetalArmor") {
        throw "$($set[0]) armour should use the class-agnostic SoulMetalArmor base."
    }

    if ($armor -notmatch "SoulAscensionPlayer\.ClassOf" -or $armor -notmatch "HasClassSoul") {
        throw "$($set[0])'s set bonus should empower whichever class the player's Soul actually is."
    }

    foreach ($piece in @("Helm", "Chest", "Greaves")) {
        $name = "$($set[0])$piece"
        if ($loc -notmatch "(?ms)\b${name}:\s*\{[^}]*DisplayName:" -and $loc -notmatch "${name}\.DisplayName:") {
            throw "en-US.hjson should localize $name."
        }
    }
}

# --- The top ore is a full rung: armour AND a weapon for every class ---------
# Revenite is the deepest dig before Hellstone, so it has to be worth reaching for whatever
# class you are. These are ungated on purpose: they are pre-Wall-of-Flesh gear, so they must
# work before a subclass even exists.

$reveniteWeapons = @{
    "Content\Items\Weapons\Warrior\ReveniteCleaver.cs"   = "DamageClass\.Melee"
    "Content\Items\Weapons\Ranger\ReveniteLongbow.cs"    = "DamageClass\.Ranged"
    "Content\Items\Weapons\Magic\ReveniteScepter.cs"     = "DamageClass\.Magic"
    "Content\Items\Weapons\Summoner\ReveniteLash.cs"     = "DamageClass\.SummonMeleeSpeed"
}

foreach ($path in $reveniteWeapons.Keys) {
    $w = Read-File $path
    $name = [System.IO.Path]::GetFileNameWithoutExtension($path)

    if ($w -notmatch $reveniteWeapons[$path]) {
        throw "$name should be a $($reveniteWeapons[$path]) weapon -- one per class, or a class is left out."
    }

    if ($w -notmatch "AddIngredient<ReveniteBar>") {
        throw "$name should be forged from Revenite -- that is what makes the top ore worth mining."
    }

    if ($loc -notmatch "(?ms)\b${name}:\s*\{[^}]*DisplayName:" -and $loc -notmatch "${name}\.DisplayName:") {
        throw "en-US.hjson should localize $name."
    }
}

# The Warrior one must carry the mod's melee identity: bleed.
$cleaver = Read-File "Content\Items\Weapons\Warrior\ReveniteCleaver.cs"

if ($cleaver -notmatch "IBleedWeapon") {
    throw "The Revenite Cleaver should be a bleed sword -- that is the Warrior line's identity."
}

# =============================================================================
# HARDMODE ORES
# =============================================================================

# --- They appear only when the world turns, not at worldgen ------------------
# A Hardmode ore seeded at worldgen would be reachable pre-Hardmode -- the whole point is that
# it waits for the Wall of Flesh, exactly like Cobalt/Mythril/Adamantite.

$trigger = Read-File "Content\Globals\HardmodeOreTriggerNPC.cs"

if ($trigger -notmatch "NPCID\.WallofFlesh" -or $trigger -notmatch "SeedHardmodeOres") {
    throw "Hardmode ores should be seeded when the Wall of Flesh falls."
}

if ($trigger -notmatch "NetmodeID\.MultiplayerClient") {
    throw "Only the server / singleplayer may edit the world -- clients must not seed ores."
}

if ($gen -notmatch "SeedHardmodeOres" -or $gen -notmatch "HardmodeOresSeeded") {
    throw "The ore generator should seed Hardmode ores once, guarded by a persisted world flag."
}

if ($gen -notmatch "SaveWorldData" -or $gen -notmatch "LoadWorldData") {
    throw "The 'Hardmode ores seeded' flag must persist, or re-killing the Wall re-seeds them."
}

# --- Three Hardmode tiers, gated by pickaxe power ---------------------------

$hmTiers = @{
    "WraithiteOreTile" = 100
    "AetheriumOreTile" = 150
    "NullsteelOreTile" = 200
}

foreach ($tile in $hmTiers.Keys) {
    $t = Read-File "Content\Tiles\Ores\$tile.cs"

    if ($t -notmatch "EterniaOreTile") {
        throw "$tile should extend the shared EterniaOreTile."
    }

    if ($t -notmatch "MinPickPower => $($hmTiers[$tile])") {
        throw "$tile should require pickaxe power $($hmTiers[$tile])."
    }

    if ($gen -notmatch "$tile") {
        throw "$tile should be seeded by SeedHardmodeOres."
    }
}

# --- Each HM bar has armour AND a weapon (no dead-end materials) -------------

foreach ($set in @("Wraithite", "Aetherium", "Nullsteel")) {
    $bar = Read-File "Content\Items\Materials\${set}Bar.cs"
    if ($bar -notmatch "TileID\.AdamantiteForge") {
        throw "$set Bar should smelt at a Hardmode forge."
    }

    $armor = Read-File "Content\Items\Armor\${set}Set.cs"
    if ($armor -notmatch "SoulMetalArmor" -or $armor -notmatch "AddIngredient<${set}Bar>") {
        throw "$set armour should be class-agnostic and crafted from its bar."
    }

    if ($armor -notmatch "SoulAscensionPlayer\.ClassOf") {
        throw "$set's set bonus should empower whichever class the player's Soul is."
    }
}

# The top HM ore is a full rung: a weapon per class.
$nullsteelWeapons = @{
    "Content\Items\Weapons\Warrior\NullsteelReaver.cs"  = "DamageClass\.Melee"
    "Content\Items\Weapons\Ranger\NullsteelRepeater.cs" = "DamageClass\.Ranged"
    "Content\Items\Weapons\Magic\NullsteelScepter.cs"   = "DamageClass\.Magic"
    "Content\Items\Weapons\Summoner\NullsteelLash.cs"   = "DamageClass\.SummonMeleeSpeed"
}

foreach ($path in $nullsteelWeapons.Keys) {
    $w = Read-File $path
    if ($w -notmatch $nullsteelWeapons[$path] -or $w -notmatch "AddIngredient<NullsteelBar>") {
        throw "$([System.IO.Path]::GetFileNameWithoutExtension($path)) should be a Nullsteel weapon of its class."
    }
}

Write-Host "Eternia ores source smoke test passed."
