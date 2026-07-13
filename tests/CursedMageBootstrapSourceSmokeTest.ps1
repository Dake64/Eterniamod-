$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$playerPath = Join-Path $repoRoot "Content\Players\CursedMagePlayer.cs"
$tomePath = Join-Path $repoRoot "Content\Items\Weapons\Magic\CursedApprenticeTome.cs"
$projectilePath = Join-Path $repoRoot "Content\Projectiles\CursedBoltProjectile.cs"

$player = Get-Content -Raw $playerPath
$tome = Get-Content -Raw $tomePath
$projectile = Get-Content -Raw $projectilePath

if ($player -notmatch "public const int MinimumCastEnergy\s*=\s*3") {
    throw "CursedMagePlayer should define MinimumCastEnergy so the tome and regen use one bootstrap threshold."
}

if ($player -notmatch "public bool IsActiveCursedMage\(") {
    throw "CursedMagePlayer should expose IsActiveCursedMage for delayed projectiles and energy spending."
}

# Energy always bootstraps: a FIXED pre-hardmode regen, and a >=1 hardmode baseline even
# at zero corruption, so casting is never soft-locked.
if ($player -notmatch "PreHardmodeRegen" -or $player -notmatch "c >= 26 \? 2 :") {
    throw "CursedMagePlayer should bootstrap energy (fixed pre-hardmode regen + hardmode baseline)."
}

# Any Mage can spend Cursed Energy so curse weapons work pre-hardmode too.
if ($player -notmatch "ConsumeEnergy\(int amount\)[\s\S]*if \(!IsActiveMage\(\)") {
    throw "CursedMagePlayer.ConsumeEnergy should let any Mage spend energy (pre-hardmode curse weapons)."
}

if ($tome -notmatch "CursedMagePlayer\.MinimumCastEnergy" -or
    $tome -match "CursedEnergy\s*>=\s*3" -or
    $tome -match "ConsumeEnergy\(3\)") {
    throw "CursedApprenticeTome should use CursedMagePlayer.MinimumCastEnergy instead of hard-coded 3."
}

if ($projectile -notmatch "if \(!cursed\.IsActiveCursedMage\(\)\)") {
    throw "CursedBoltProjectile should stop effects when the owner is no longer Cursed Mage."
}

Write-Host "Cursed Mage bootstrap source smoke test passed."
