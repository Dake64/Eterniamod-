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

if ($player -notmatch "TotalCorruption\s*<=\s*0[\s\S]*CursedEnergy\s*<\s*MinimumCastEnergy[\s\S]*regen\s*=\s*1") {
    throw "CursedMagePlayer should regenerate up to MinimumCastEnergy without corruption so a promoted Cursed Mage can start using the tome."
}

if ($player -notmatch "ConsumeEnergy\(int amount\)[\s\S]*if \(!IsActiveCursedMage\(\)\)") {
    throw "CursedMagePlayer.ConsumeEnergy should reject spending outside active Cursed Mage."
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
