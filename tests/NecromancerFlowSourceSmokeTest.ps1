$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$necroPlayerPath = Join-Path $repoRoot "Content\Players\NecromancerPlayer.cs"
$baseMinionPath = Join-Path $repoRoot "Content\Projectiles\Necromancer\BaseNecroMinion.cs"

$necroPlayer = Get-Content -Raw $necroPlayerPath
$baseMinion = Get-Content -Raw $baseMinionPath

if ($necroPlayer -notmatch "MaxNecroSlots\s*=\s*1\s*\+\s*stats\.ShadowAffinity\s*/\s*5") {
    throw "Necromancer slots should scale from ShadowAffinity."
}

if ($necroPlayer -notmatch 'HasActivePassive\(\s*soul\.ActiveSoul,\s*"Bone Conduit"\s*\)') {
    throw "Bone Conduit should affect Necromancer slot scaling only through the active Soul tree."
}

if ($baseMinion -match "ActiveNecroSummons\+\+" -or
    $baseMinion -match "ManaDrainPerSecond\\s*\\+=") {
    throw "BaseNecroMinion should not duplicate NecromancerPlayer summon or mana accounting."
}

Write-Host "Necromancer flow source smoke test passed."
