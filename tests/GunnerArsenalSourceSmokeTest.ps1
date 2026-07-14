$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$rangerDir = Join-Path $repoRoot "Content\Items\Weapons\Ranger"
$globalsDir = Join-Path $repoRoot "Content\Globals"

function Read-File($path) {
    if (-not (Test-Path $path)) { throw "Missing expected file: $path" }
    return Get-Content -Raw $path
}

# --- Base + dispatch ---------------------------------------------------------

$base = Read-File (Join-Path $rangerDir "GunnerGun.cs")

if ($base -notmatch "abstract class GunnerGun\s*:\s*ModItem") {
    throw "GunnerGun should be the base ModItem for every Gunner firearm."
}

# Bullet weapons (so Momentum applies), with per-gun spread / bullets-per-shot.
if ($base -notmatch "useAmmo\s*=\s*AmmoID\.Bullet") {
    throw "Gunner guns should fire bullets (so Momentum applies)."
}

foreach ($v in @("BulletsPerShot", "SpreadDegrees", "OnBulletHit")) {
    if ($base -notmatch $v) {
        throw "GunnerGun should expose '$v'."
    }
}

if ($base -notmatch "GetGlobalProjectile<GunnerGunGlobalProjectile>\(\)\.gunType\s*=\s*Type") {
    throw "GunnerGun.Shoot should tag each bullet with its gun type."
}

$global = Read-File (Join-Path $globalsDir "GunnerGunGlobalProjectile.cs")
if ($global -notmatch "InstancePerEntity" -or $global -notmatch "gun\.OnBulletHit\(") {
    throw "GunnerGunGlobalProjectile should dispatch on-hit signatures to the firing gun."
}

# --- Every gun exists and extends GunnerGun ----------------------------------

$guns = @(
    "ScrapPistol", "RattlerSMG", "TwinDerringer", "Chatterbox", "HellfireRepeater",
    "MythrilRipper", "Autoloader", "TwinVortexSMG", "Suppressor", "FrostSweeper",
    "ChlorophyteAutogun", "SpectralShredder", "VortexRipper", "StormMinigun",
    "SingularityBarrage"
)

$gunSrc = @{}
foreach ($gun in $guns) {
    $src = Read-File (Join-Path $rangerDir "$gun.cs")
    if ($src -notmatch "class $gun\s*:\s*GunnerGun") {
        throw "$gun should extend GunnerGun."
    }
    $gunSrc[$gun] = $src
}

# --- Signature effects wired -------------------------------------------------

if ($gunSrc["HellfireRepeater"] -notmatch "BuffID\.OnFire") {
    throw "HellfireRepeater bullets should burn."
}

if ($gunSrc["Suppressor"] -notmatch "AddMomentum") {
    throw "Suppressor should feed extra Momentum on hit."
}

if ($gunSrc["FrostSweeper"] -notmatch "BuffID\.Frostburn") {
    throw "FrostSweeper should chill and frost-burn."
}

# Multi-bullet guns fire more than one bullet per shot.
foreach ($twin in @("TwinDerringer", "TwinVortexSMG")) {
    if ($gunSrc[$twin] -notmatch "BulletsPerShot\s*=>\s*2") {
        throw "$twin should fire two bullets per shot."
    }
}

# The endgame minigun is the fastest-firing gun.
if ($gunSrc["SingularityBarrage"] -notmatch "SetGunDefaults\(78,\s*3,") {
    throw "SingularityBarrage should be the endgame minigun (fastest fire rate)."
}

# --- Obtention ---------------------------------------------------------------

$drops = Read-File (Join-Path $globalsDir "GunnerObtentionGlobalNPC.cs")
if ($drops -notmatch "MartianSaucerCore" -or $drops -notmatch "StormMinigun") {
    throw "Storm Minigun should drop from the Martian Saucer."
}

Write-Host "Gunner arsenal source smoke test passed."
