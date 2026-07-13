$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$playerPath = Join-Path $repoRoot "Content\Players\EnergyShooterPlayer.cs"
$globalPath = Join-Path $repoRoot "Content\Globals\EnergyGunnerGlobalProjectile.cs"
$registryPath = Join-Path $repoRoot "Content\Passives\PassiveRegistry.cs"

foreach ($p in @($playerPath, $globalPath, $registryPath)) {
    if (-not (Test-Path $p)) {
        throw "Missing expected file: $p"
    }
}

$player = Get-Content -Raw $playerPath
$global = Get-Content -Raw $globalPath
$registry = Get-Content -Raw $registryPath

# --- Temperature backbone: heat, ceiling, percent, zones -----------------------

foreach ($member in @("public float Heat", "public bool Overheated", "float MaxHeat", "float HeatPercent", "int Zone")) {
    if ($player -notmatch [regex]::Escape($member)) {
        throw "EnergyShooterPlayer should expose '$member' for the temperature mechanic."
    }
}

# Zone thresholds must be 40% (hot) and 70% (critical).
if ($player -notmatch "HeatPercent\s*>=\s*70f\s*\?\s*2") {
    throw "Zone should turn critical (2) at 70% heat."
}

if ($player -notmatch "HeatPercent\s*>=\s*40f\s*\?\s*1") {
    throw "Zone should turn hot (1) at 40% heat."
}

# Firing builds heat and overheats at the ceiling.
if ($player -notmatch "Heat\s*\+=") {
    throw "Firing a ranged weapon should build heat."
}

# Heat is per-weapon: the player reads HeatPerShot off the held energy weapon.
if ($player -notmatch "HeatPerShot") {
    throw "EnergyShooterPlayer should read the weapon's HeatPerShot (per-weapon heat)."
}

if ($player -notmatch "Heat\s*>=\s*MaxHeat" -or $player -notmatch "Overheat\(\)") {
    throw "Reaching MaxHeat should trigger Overheat()."
}

# --- Overheat penalty: lockout, true damage, Burning ---------------------------

if ($player -notmatch "Overheated\s*=\s*true") {
    throw "Overheat() should lock the weapon (Overheated = true)."
}

if ($player -notmatch "Player\.Hurt\(") {
    throw "Overheat() should deal self damage."
}

if ($player -notmatch "BuffID\.OnFire") {
    throw "Overheat() should inflict Burning (OnFire)."
}

# CanUseItem must refuse to fire while overheated.
if ($player -notmatch "if\s*\(Overheated\)\s*[\r\n]+\s*\{\s*[\r\n]+\s*return false;") {
    throw "CanUseItem should block firing while Overheated."
}

# Overheated weapon stays locked until it cools to a safe temperature.
if ($player -notmatch "Overheated\s*&&\s*Heat\s*<=\s*MaxHeat\s*\*\s*0\.3f") {
    throw "An overheated weapon should stay locked until it cools to a safe fraction of MaxHeat."
}

# --- Zone bonuses: damage / fire-rate / crit -----------------------------------

if ($player -notmatch "Zone\s*==\s*2" -or $player -notmatch "damage\s*\+=\s*0\.20f") {
    throw "The critical zone should grant +20% damage."
}

if ($player -notmatch "Zone\s*==\s*1" -or $player -notmatch "damage\s*\+=\s*0\.10f") {
    throw "The hot zone should grant +10% damage."
}

if ($player -notmatch "UseSpeedMultiplier" -or $player -notmatch "1\.20f\s*:\s*Zone\s*==\s*1\s*\?\s*1\.10f") {
    throw "The zones should raise fire rate (+20% critical, +10% hot)."
}

if ($player -notmatch "crit\s*\+=\s*10f") {
    throw "The critical zone should grant +10% crit."
}

# --- Passive tree shaping ------------------------------------------------------

# Every Energy node must actually change the mechanic somewhere in the player.
$energyNodes = @(
    "Reactor Core",    # higher ceiling
    "Energy Core",     # less heat per shot
    "Ion Surge",       # faster cooling
    "Fusion Cannon",   # softer overheat self-damage
    "Overload",        # emergency explosion
    "Overcharge",      # heat-scaled damage
    "Particle Beam",   # crit in critical zone
    "Plasma Reactor"   # plasma conductors
)

foreach ($node in $energyNodes) {
    if ($player -notmatch "HasEnergy\(`"$node`"\)") {
        throw "Energy node '$node' should shape the temperature mechanic in EnergyShooterPlayer."
    }
}

# Conductors of Plasma is exposed for the global projectile.
if ($player -notmatch "ConductorsOfPlasma") {
    throw "EnergyShooterPlayer should expose ConductorsOfPlasma for plasma-conductor shots."
}

# --- Marker gating: only real energy weapons build heat --------------------------

$rangerDir = Join-Path $repoRoot "Content\Items\Weapons\Ranger"
$markerPath = Join-Path $rangerDir "IEnergyWeapon.cs"
$basePath = Join-Path $rangerDir "EnergyWeapon.cs"

if (-not (Test-Path $markerPath)) {
    throw "Missing IEnergyWeapon marker interface."
}

if (-not (Test-Path $basePath)) {
    throw "Missing EnergyWeapon base class."
}

# The player only heats up for IEnergyWeapon items.
if ($player -notmatch "IsEnergyWeapon\(item\)") {
    throw "EnergyShooterPlayer should gate the temperature mechanic on IsEnergyWeapon(item)."
}

if ($player -notmatch "item\.ModItem\s+is\s*[\r\n]?\s*Content\.Items\.Weapons\.Ranger\.IEnergyWeapon") {
    throw "IsEnergyWeapon should test for the IEnergyWeapon marker."
}

# The HM energy base opts in; the pre-HM prototypes deliberately do NOT.
$energyBase = Get-Content -Raw $basePath
if ($energyBase -notmatch "EnergyWeapon\s*:\s*ModItem\s*,\s*IEnergyWeapon") {
    throw "EnergyWeapon base should implement IEnergyWeapon."
}

# The interface declares per-weapon heat, and the base exposes it.
$marker = Get-Content -Raw $markerPath
if ($marker -notmatch "float\s+HeatPerShot") {
    throw "IEnergyWeapon should declare HeatPerShot (per-weapon heat)."
}

if ($energyBase -notmatch "HeatPerShot") {
    throw "EnergyWeapon base should expose HeatPerShot."
}

# Every Hardmode energy weapon sets its own heat -> at least the full roster's worth.
$heatOverrides =
    ([regex]::Matches(
        (Get-ChildItem -Path $rangerDir -Filter *.cs | Get-Content -Raw | Out-String),
        "override\s+float\s+HeatPerShot")).Count

if ($heatOverrides -lt 20) {
    throw "Expected the full energy arsenal to each define HeatPerShot (found $heatOverrides)."
}

foreach ($proto in @(
    "CoilPistol", "SparkCarbine", "ElectromagneticRifle", "ExperimentalPlasmaShotgun",
    "TeslaRepeater", "PulseLauncher", "UnstablePlasmaCannon", "ExperimentalPhotonRifle",
    "RailgunPrototype")) {
    $protoPath = Join-Path $rangerDir "$proto.cs"
    if (-not (Test-Path $protoPath)) {
        throw "Missing pre-Hardmode prototype: $proto."
    }
    $protoSrc = Get-Content -Raw $protoPath
    # Check the class declaration, not comments -- prototypes explain they are NOT energy weapons.
    if ($protoSrc -match "class\s+$proto\s*:[^\{]*IEnergyWeapon") {
        throw "Prototype '$proto' must NOT implement IEnergyWeapon (no Temperature pre-Hardmode)."
    }
}

# --- Global projectile: plasma conductors --------------------------------------

if ($global -notmatch "ConductorsOfPlasma") {
    throw "EnergyGunnerGlobalProjectile should read ConductorsOfPlasma."
}

if ($global -notmatch "penetrate\s*\+=" -or $global -notmatch "scale\s*\*=") {
    throw "Plasma conductors should make critical-zone shots pierce more and grow."
}

if ($global -notmatch "BuffID\.OnFire") {
    throw "Plasma-conductor shots should burn what they hit."
}

Write-Host "Energy Gunner temperature source smoke test passed."
