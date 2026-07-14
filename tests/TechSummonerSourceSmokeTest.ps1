$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

# --- POWER CORE / OVERDRIVE + Tech passive shaping ---------------------------

$player = Read-File "Content\Players\TechSummonerPlayer.cs"

foreach ($member in @("public float PowerCore", "const float MaxPowerCore", "int OverdriveTimer", "IsActiveTechSummoner")) {
    if ($player -notmatch [regex]::Escape($member)) {
        throw "TechSummonerPlayer should expose '$member'."
    }
}

# The core charges on its own, faster with drones deployed.
if ($player -notmatch "PowerCore\s*\+=" -or $player -notmatch "slotsMinions") {
    throw "The Power Core should charge faster with drones deployed."
}

# Overdrive: damage spike + shield.
if ($player -notmatch "OVERDRIVE" -or $player -notmatch "statDefense") {
    throw "Overdrive should grant a damage spike and a shield."
}

# The Tech branch shapes every part of it.
foreach ($node in @("Tech Protocol", "War Machine", "Overclocked Core", "Nanoswarm")) {
    if ($player -notmatch "HasTech\(`"$node`"\)") {
        throw "Tech node '$node' should shape the Power Core / Overdrive mechanic."
    }
}

if ($player -notmatch "HasKeystone\(`"Combat Protocol`"\)") {
    throw "The Combat Protocol keystone should harden the Overdrive shield."
}

# --- Drones are RANGED minions (they shoot; they do not ram) -----------------

$droneBase = Read-File "Content\Projectiles\Summoner\DroneMinion.cs"

if ($droneBase -notmatch "abstract class DroneMinion\s*:\s*ModProjectile") {
    throw "DroneMinion should be the base drone."
}

if ($droneBase -notmatch "MinionContactDamage\(\)\s*=>\s*false") {
    throw "Drones should NOT deal contact damage -- they shoot."
}

if ($droneBase -notmatch "ShotCooldown" -or $droneBase -notmatch "ShotType" -or
    $droneBase -notmatch "ConfigureShot") {
    throw "DroneMinion should fire a configurable bolt on a cooldown."
}

# Drone passives read at runtime.
if ($droneBase -notmatch "Combat AI" -or $droneBase -notmatch "Targeting Array") {
    throw "Combat AI (fire rate) and Targeting Array (range) should shape the drones."
}

$laser = Read-File "Content\Projectiles\Summoner\DroneLaser.cs"
if ($laser -notmatch "DamageClass\.Summon" -or $laser -notmatch "DebuffType") {
    throw "The drone bolt should be summon damage and carry its drone's debuff."
}

# --- Two-stage assembly: forge parts, then BUILD the drone -------------------

$components = @("DroneChassis", "ServoCore", "CommandChip")

foreach ($c in $components) {
    $src = Read-File "Content\Items\Materials\$c.cs"
    if ($src -notmatch "class $c\s*:\s*DroneComponent") {
        throw "$c should be a DroneComponent."
    }
    # Stage 1: parts are FORGED from the tech scrap that drops in the world.
    if ($src -notmatch "AddRecipes") {
        throw "$c should be crafted from tech materials."
    }
}

$kits = @{
    "ScoutDroneKit" = "ScoutDrone"; "RepeaterDroneKit" = "RepeaterDrone";
    "TeslaDroneKit" = "TeslaDrone"; "PlasmaDroneKit" = "PlasmaDrone";
    "RailDroneKit" = "RailDrone"; "OmegaDroneKit" = "OmegaDrone"
}

foreach ($kit in $kits.Keys) {
    $drone = $kits[$kit]

    $dsrc = Read-File "Content\Projectiles\Summoner\$drone.cs"
    if ($dsrc -notmatch "class $drone\s*:\s*DroneMinion") {
        throw "$drone should extend DroneMinion."
    }

    $ksrc = Read-File "Content\Items\Weapons\Summoner\$kit.cs"
    if ($ksrc -notmatch "class $kit\s*:\s*DroneKit" -or $ksrc -notmatch $drone) {
        throw "$kit should be a DroneKit deploying a $drone."
    }

    # Stage 2: every drone is ASSEMBLED from the forged parts.
    if ($ksrc -notmatch "AddIngredient<DroneChassis>" -or
        $ksrc -notmatch "AddIngredient<ServoCore>" -or
        $ksrc -notmatch "AddIngredient<CommandChip>") {
        throw "$kit should be assembled from a Chassis, a Servo Core and a Command Chip."
    }
}

$kitBase = Read-File "Content\Items\Weapons\Summoner\DroneKit.cs"
if ($kitBase -notmatch "AddBuff\(ModContent\.BuffType<DroneMinionBuff>" -or
    $kitBase -notmatch "NewProjectileDirect") {
    throw "DroneKit should deploy its drone and apply the shared fleet buff."
}

# --- Hardmode Tech whips -----------------------------------------------------

$whips = @{ "CircuitLash" = "TechWhipProjectile"; "OmegaLash" = "OmegaWhipProjectile" }

foreach ($whip in $whips.Keys) {
    $item = Read-File "Content\Items\Weapons\Summoner\$whip.cs"
    if ($item -notmatch "class $whip\s*:\s*SummonerWhip" -or $item -notmatch $whips[$whip]) {
        throw "$whip should be a SummonerWhip shooting $($whips[$whip])."
    }

    $proj = Read-File "Content\Projectiles\Summoner\$($whips[$whip]).cs"
    if ($proj -notmatch "RequiredSubclass" -or $proj -notmatch "Tech Summoner") {
        throw "$($whips[$whip]) should be Tech Summoner gear."
    }
}

Write-Host "Tech Summoner source smoke test passed."
