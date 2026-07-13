$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$necroPlayerPath = Join-Path $repoRoot "Content\Players\NecromancerPlayer.cs"
$baseMinionPath = Join-Path $repoRoot "Content\Projectiles\Necromancer\BaseNecroMinion.cs"

$necroPlayer = Get-Content -Raw $necroPlayerPath
$baseMinion = Get-Content -Raw $baseMinionPath

# No minion slots: undead RESERVE max life and DRAIN mana. Shadow affinity eases the
# reserved-life toll, and Bone Conduit eases it further.
if ($necroPlayer -notmatch "ReservedLifeFraction" -or
    $necroPlayer -notmatch "ReservePercent") {
    throw "Necromancer should use reserved life (ReservedLifeFraction / ReservePercent), not slots."
}

if ($necroPlayer -notmatch "stats\.ShadowAffinity") {
    throw "Shadow affinity should ease the Necromancer's reserved-life toll."
}

if ($necroPlayer -notmatch 'HasActivePassive\(\s*soul\.ActiveSoul,\s*"Bone Conduit"\s*\)') {
    throw "Bone Conduit should affect the Necromancer mechanic only through the active Soul tree."
}

# Mana shortage crumbles the weakest undead first.
if ($necroPlayer -notmatch "DespawnWeakest" -or $necroPlayer -notmatch "statMana") {
    throw "Necromancer should drain mana and crumble the weakest undead when it runs dry."
}

if ($baseMinion -match "ActiveNecroSummons\+\+" -or
    $baseMinion -match "ManaDrainPerSecond\\s*\\+=") {
    throw "BaseNecroMinion should not duplicate NecromancerPlayer summon or mana accounting."
}

Write-Host "Necromancer flow source smoke test passed."
