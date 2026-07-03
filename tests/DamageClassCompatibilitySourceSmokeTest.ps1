$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$energyPath = Join-Path $repoRoot "Content\Players\EnergyShooterPlayer.cs"
$soulRulesPath = Join-Path $repoRoot "Content\Souls\SoulRules.cs"

$energy = Get-Content -Raw $energyPath
$soulRules = Get-Content -Raw $soulRulesPath

if ($energy -match "item\.DamageType\s*[\r\n]+\s*!=\s*DamageClass\.Ranged") {
    throw "EnergyShooterPlayer should use item.DamageType.CountsAsClass(DamageClass.Ranged), not exact ranged inequality."
}

if ($energy -match "proj\.DamageType\s*[\r\n]+\s*!=\s*DamageClass\.Ranged") {
    throw "EnergyShooterPlayer should use proj.DamageType.CountsAsClass(DamageClass.Ranged), not exact ranged inequality."
}

$itemCounts =
    ([regex]::Matches($energy, "item\.DamageType\s*[\r\n]+\s*\.CountsAsClass\(DamageClass\.Ranged\)")).Count

$projCounts =
    ([regex]::Matches($energy, "proj\.DamageType\s*[\r\n]+\s*\.CountsAsClass\(DamageClass\.Ranged\)")).Count

if ($itemCounts -lt 3) {
    throw "EnergyShooterPlayer should use CountsAsClass for every ranged item check."
}

if ($projCounts -lt 1) {
    throw "EnergyShooterPlayer should use CountsAsClass for ranged projectile checks."
}

if ($soulRules -notmatch "actualClass\.CountsAsClass\(expectedClass\)") {
    throw "SoulRules should remain compatible with inherited or composite tModLoader damage classes."
}

Write-Host "Damage class compatibility source smoke test passed."
