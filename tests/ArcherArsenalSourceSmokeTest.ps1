$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$rangerDir = Join-Path $repoRoot "Content\Items\Weapons\Ranger"
$projDir = Join-Path $repoRoot "Content\Projectiles\Ranger"
$globalsDir = Join-Path $repoRoot "Content\Globals"

function Read-File($path) {
    if (-not (Test-Path $path)) { throw "Missing expected file: $path" }
    return Get-Content -Raw $path
}

# --- Base + dispatch ---------------------------------------------------------

$base = Read-File (Join-Path $rangerDir "ArcherBow.cs")

if ($base -notmatch "abstract class ArcherBow\s*:\s*ModItem") {
    throw "ArcherBow should be the base ModItem for every Archer bow."
}

foreach ($v in @("OnArrowSpawn", "ModifyArrowHit", "OnArrowHit", "UpdateArrow")) {
    if ($base -notmatch "virtual void $v\(") {
        throw "ArcherBow should expose the '$v' effect hook."
    }
}

# Bows fire real arrows and tag them with the firing bow + Perfect-Shot state.
if ($base -notmatch "useAmmo\s*=\s*AmmoID\.Arrow") {
    throw "Archer bows should fire arrows (so Concentration applies)."
}

if ($base -notmatch "g\.bowType\s*=\s*Type" -or $base -notmatch "g\.perfectArrow\s*=\s*perfect") {
    throw "ArcherBow.Shoot should tag the arrow with its bow type and Perfect-Shot state."
}

$global = Read-File (Join-Path $globalsDir "ArcherBowGlobalProjectile.cs")

if ($global -notmatch "InstancePerEntity" -or $global -notmatch "int bowType" -or $global -notmatch "int pierceHits") {
    throw "ArcherBowGlobalProjectile should carry per-arrow bow type and pierce count."
}

foreach ($dispatch in @("UpdateArrow", "ModifyArrowHit", "OnArrowHit")) {
    if ($global -notmatch "bow\.$dispatch\(") {
        throw "ArcherBowGlobalProjectile should dispatch '$dispatch' to the firing bow."
    }
}

# --- Every bow exists and extends ArcherBow ----------------------------------

$bows = @(
    "HuntersBranch", "WoodlandLongbow", "FalconBow", "WindWhisper", "JungleStinger",
    "CrimsonHunter", "CorruptHunter", "MoltenLongbow", "MythrilPrecisionBow", "Frostpiercer",
    "ChlorophyteSniperBow", "TempleJudgement", "EclipseRecurve", "DragonboneBow",
    "CelestialHorizon", "EternalHorizon"
)

$bowSrc = @{}
foreach ($bow in $bows) {
    $src = Read-File (Join-Path $rangerDir "$bow.cs")
    if ($src -notmatch "class $bow\s*:\s*ArcherBow") {
        throw "$bow should extend ArcherBow."
    }
    $bowSrc[$bow] = $src
}

# --- Signature effects wired -------------------------------------------------

if ($bowSrc["FalconBow"] -notmatch "shotCounter\s*%\s*3") {
    throw "FalconBow should empower every third shot."
}

if ($bowSrc["JungleStinger"] -notmatch "BuffID\.Poisoned") {
    throw "JungleStinger should poison and reward poisoned targets."
}

if ($bowSrc["CrimsonHunter"] -notmatch "hit\.Crit" -or $bowSrc["CrimsonHunter"] -notmatch "HealEffect") {
    throw "CrimsonHunter should heal on critical hits."
}

if ($bowSrc["CorruptHunter"] -notmatch "BuffID\.Ichor") {
    throw "CorruptHunter should sap enemy defense."
}

if ($bowSrc["MoltenLongbow"] -notmatch "perfect" -or $bowSrc["MoltenLongbow"] -notmatch "BuffID\.OnFire") {
    throw "MoltenLongbow Perfect Shots should explode and burn."
}

if ($bowSrc["MythrilPrecisionBow"] -notmatch "perfect" -or $bowSrc["MythrilPrecisionBow"] -notmatch "ArmorPenetration") {
    throw "MythrilPrecisionBow Perfect Shots should ignore defense."
}

if ($bowSrc["Frostpiercer"] -notmatch "BuffID\.Frozen") {
    throw "Frostpiercer Perfect Shots should freeze normal enemies."
}

if ($bowSrc["ChlorophyteSniperBow"] -notmatch "UpdateArrow") {
    throw "ChlorophyteSniperBow arrows should gently home."
}

if ($bowSrc["TempleJudgement"] -notmatch "penetrate\s*=\s*-1") {
    throw "TempleJudgement Perfect Shots should pierce everything."
}

if ($bowSrc["EclipseRecurve"] -notmatch "pierceHits") {
    throw "EclipseRecurve should ramp damage per pierced enemy."
}

if ($bowSrc["DragonboneBow"] -notmatch "SpectralDragon") {
    throw "DragonboneBow should loose a spectral dragon."
}

if ($bowSrc["CelestialHorizon"] -notmatch "StarShard") {
    throw "CelestialHorizon Perfect Shots should rain stars."
}

if ($bowSrc["EternalHorizon"] -notmatch "ConstellationArrow") {
    throw "EternalHorizon Perfect Shots should loose a Constellation Arrow."
}

# --- Custom projectiles ------------------------------------------------------

foreach ($proj in @("SpectralDragon", "StarShard", "ConstellationArrow")) {
    $src = Read-File (Join-Path $projDir "$proj.cs")
    if ($src -notmatch "class $proj\s*:\s*ModProjectile") {
        throw "$proj should be a ModProjectile."
    }
}

$constellation = Read-File (Join-Path $projDir "ConstellationArrow.cs")
if ($constellation -notmatch "DefenseEffectiveness\s*\*=\s*0\.5f") {
    throw "ConstellationArrow should ignore 50% of defense."
}
if ($constellation -notmatch "penetrate\s*=\s*-1" -or $constellation -notmatch "DistanceScale") {
    throw "ConstellationArrow should pierce forever and scale with distance travelled."
}

# --- Obtention ---------------------------------------------------------------

$drops = Read-File (Join-Path $globalsDir "ArcherObtentionGlobalNPC.cs")

foreach ($pair in @(
    @("IceMimic", "Frostpiercer"),
    @("Golem", "TempleJudgement"),
    @("Mothron", "EclipseRecurve"),
    @("DukeFishron", "DragonboneBow"))) {
    if ($drops -notmatch "NPCID\.$($pair[0])" -or $drops -notmatch $pair[1]) {
        throw "Expected $($pair[1]) to drop from $($pair[0])."
    }
}

if ($drops -notmatch "ModifyActiveShop" -or $drops -notmatch "Merchant" -or $drops -notmatch "WoodlandLongbow") {
    throw "The Merchant should sell the Woodland Longbow after the Eye of Cthulhu."
}

$chest = Read-File (Join-Path $repoRoot "Content\Systems\ArcherChestLoot.cs")
if ($chest -notmatch "WindWhisper" -or $chest -notmatch "TileID\.Cloud") {
    throw "Wind Whisper should be seeded into Sky Island chests."
}

Write-Host "Archer arsenal source smoke test passed."
