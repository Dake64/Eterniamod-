$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$stunnerPath = Join-Path $repoRoot "Content\Players\StunnerPlayer.cs"
$content = Get-Content -Raw $stunnerPath

if ($content -notmatch "Player\.HeldItem\.DamageType\s*\.\s*CountsAsClass\(\s*DamageClass\.Melee\s*\)") {
    throw "Stunner charge should use CountsAsClass(DamageClass.Melee), not exact DamageClass.Melee equality."
}

if ($content -match "Player\.HeldItem\.DamageType\s*==\s*DamageClass\.Melee") {
    throw "Stunner charge should not use exact DamageClass.Melee equality."
}

Write-Host "Stunner melee source smoke test passed."
