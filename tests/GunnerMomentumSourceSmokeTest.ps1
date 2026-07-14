$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$playerPath = Join-Path $repoRoot "Content\Players\GunnerPlayer.cs"
$globalPath = Join-Path $repoRoot "Content\Globals\GunnerGlobalProjectile.cs"

foreach ($p in @($playerPath, $globalPath)) {
    if (-not (Test-Path $p)) {
        throw "Missing expected file: $p"
    }
}

$player = Get-Content -Raw $playerPath
$global = Get-Content -Raw $globalPath

# --- Momentum backbone -------------------------------------------------------

foreach ($member in @("public float Momentum", "const float MaxMomentum", "float MomentumPercent", "public bool DeadEye", "int Tier")) {
    if ($player -notmatch [regex]::Escape($member)) {
        throw "GunnerPlayer should expose '$member' for the Momentum mechanic."
    }
}

# Bullet weapons only.
if ($player -notmatch "useAmmo\s*!=\s*AmmoID\.Bullet") {
    throw "The Momentum mechanic should apply to bullet weapons only."
}

# --- Builds on hit, decays when idle -----------------------------------------

if ($player -notmatch "OnHitNPCWithProj" -or $player -notmatch "Momentum\s*\+=") {
    throw "Landing a bullet should build Momentum."
}

if ($player -notmatch "ticksSinceHit\s*>\s*30" -or $player -notmatch "Momentum\s*-=") {
    throw "Momentum should decay once you stop landing shots."
}

# --- Tiers (40 / 70) ---------------------------------------------------------

if ($player -notmatch "Momentum\s*>=\s*70f" -or $player -notmatch "Momentum\s*>=\s*40f") {
    throw "Momentum should grant tiered bonuses at 40 and 70."
}

# --- Dead Eye at 100 ---------------------------------------------------------

if ($player -notmatch "Momentum\s*>=\s*MaxMomentum" -or $player -notmatch "ActivateDeadEye\(\)") {
    throw "Reaching max Momentum should ignite Dead Eye."
}

if ($player -notmatch "DeadEyeTimer" -or $player -notmatch "DeadEye\s*=\s*false") {
    throw "Dead Eye should run on a timer and end."
}

# Dead Eye resets Momentum to zero when it ends.
if ($player -notmatch "DeadEyeTimer\s*<=\s*0" -or $player -notmatch "Momentum\s*=\s*0f") {
    throw "Dead Eye should drop Momentum back to zero when it ends."
}

# Dead Eye bonuses.
if ($player -notmatch "0\.30f" -or $player -notmatch "28f\s*:\s*20f" -or $player -notmatch "0\.25f") {
    throw "Dead Eye should grant strong damage / crit / fire-rate bonuses."
}

# --- Passive tree shaping ----------------------------------------------------

$gunNodes = @("Quick Trigger", "Rapid Chamber", "Deadshot", "Hair Trigger", "Bullet Storm", "Full Auto", "Executioner")

foreach ($node in $gunNodes) {
    if ($player -notmatch "HasGun\(`"$node`"\)") {
        throw "Gun node '$node' should shape the Momentum mechanic in GunnerPlayer."
    }
}

# Armor Piercing shapes Dead Eye armor pen (exposed for the global).
if ($player -notmatch "Armor Piercing" -or $player -notmatch "DeadEyeArmorPen") {
    throw "Armor Piercing should boost Dead Eye armor penetration."
}

# Trigger Discipline keystone: kills during Dead Eye extend it.
if ($player -notmatch "HasKeystone\(`"Trigger Discipline`"\)" -or $player -notmatch "target\.life\s*<=\s*0") {
    throw "Trigger Discipline should extend Dead Eye on kills."
}

# --- Global projectile: Dead Eye pierce + armor pen --------------------------

if ($global -notmatch "DeadEyePierce" -or $global -notmatch "DeadEyeArmorPen") {
    throw "GunnerGlobalProjectile should apply Dead Eye pierce and armor penetration."
}

if ($global -notmatch "useAmmo\s*!=\s*AmmoID\.Bullet" -or $global -notmatch "gunner\.DeadEye") {
    throw "The global should only empower a Gunner's bullets during Dead Eye."
}

Write-Host "Gunner Momentum source smoke test passed."
