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

Write-Host "Eternia ores source smoke test passed."
