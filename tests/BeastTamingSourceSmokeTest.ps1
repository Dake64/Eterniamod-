$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

# --- Registry: 6 beasts, each tamed from a creature, granting a staff --------

$registry = Read-File "Content\Taming\BeastTameRegistry.cs"

if ($registry -notmatch "class BeastTameRegistry" -or $registry -notmatch "ByNPC\(") {
    throw "BeastTameRegistry should map wild creatures to beasts via ByNPC."
}

$map = @{
    "Wolf" = "WolfFangTotem"; "Boar" = "BoarhideTotem"; "Raptor" = "RaptorTalon";
    "Bear" = "UrsineTotem"; "Sabertooth" = "SabertoothFang"; "Wyvern" = "Wyrmcaller"
}
foreach ($id in $map.Keys) {
    if ($registry -notmatch "Id\s*=\s*`"$id`"") {
        throw "BeastTameRegistry should list the '$id' beast."
    }
    if ($registry -notmatch "ItemType<$($map[$id])>") {
        throw "Taming '$id' should grant the $($map[$id]) staff."
    }
}

# Each entry names the wild creature(s) that tame it.
if ($registry -notmatch "NPCID\.Wolf" -or $registry -notmatch "NPCID\.WyvernHead") {
    throw "BeastTameRegistry entries should name their source NPCs."
}

# --- Taming mechanic: weaken to low health, then whip -------------------------

$player = Read-File "Content\Players\BeastTamingPlayer.cs"

if ($player -notmatch "class BeastTamingPlayer\s*:\s*ModPlayer") {
    throw "BeastTamingPlayer should track the tamed pack."
}

# Persisted collection.
if ($player -notmatch "TamedBeasts" -or $player -notmatch "SaveData" -or $player -notmatch "LoadData") {
    throw "Tamed beasts should be saved and loaded."
}

# Only a whip strike on a low-health, surviving creature can tame.
if ($player -notmatch "ProjectileID\.Sets\.IsAWhip") {
    throw "Taming should require a whip strike."
}

if ($player -notmatch "TameHealthThreshold" -or $player -notmatch "0\.15f") {
    throw "Taming should require the creature to be at low health (~15%)."
}

if ($player -notmatch "target\.life\s*>\s*target\.lifeMax\s*\*\s*TameHealthThreshold") {
    throw "A too-healthy creature should not be tameable."
}

# Chance scales with how weak the creature is, and is available to Summoners.
if ($player -notmatch "MinTameChance" -or $player -notmatch "MaxTameChance" -or
    $player -notmatch "MathHelper\.Lerp" -or $player -notmatch "SoulId\.Summoner") {
    throw "Tame chance should scale with the creature's weakness, for Summoners."
}

# Wild Bond makes taming easier (Beast tree shapes taming too).
if ($player -notmatch "HasBeast\(`"Wild Bond`"\)") {
    throw "Wild Bond should improve the tame chance."
}

# A too-healthy creature teaches the player to weaken it first.
if ($player -notmatch "weaken it") {
    throw "Whipping a healthy tameable creature should hint to weaken it first."
}

# Success grants the staff and the creature joins you (despawns, no loot).
if ($player -notmatch "QuickSpawnItem" -or $player -notmatch "StaffType\(\)") {
    throw "A successful tame should hand over the beast's staff."
}

if ($player -notmatch "target\.active\s*=\s*false") {
    throw "A tamed creature should join you (vanish) rather than drop loot."
}

# --- Discoverability: a weakened tameable creature sparkles ------------------

$globalNpc = Read-File "Content\Globals\TameableGlobalNPC.cs"

if ($globalNpc -notmatch "class TameableGlobalNPC\s*:\s*GlobalNPC" -or
    $globalNpc -notmatch "BeastTameRegistry\.ByNPC" -or
    $globalNpc -notmatch "Dust\.NewDustDirect") {
    throw "A tameable creature at low health should sparkle so the Summoner notices it."
}

# --- Staves are no longer crafted (obtained by taming) -----------------------

foreach ($staff in $map.Values) {
    $src = Read-File "Content\Items\Weapons\Summoner\$staff.cs"
    if ($src -match "AddRecipes") {
        throw "$staff should be obtained by taming, not crafting (no AddRecipes)."
    }
}

Write-Host "Beast taming source smoke test passed."
