$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$basePath = Join-Path $repoRoot "Content\Projectiles\Necromancer\BaseNecroMinion.cs"
$skeletonPath = Join-Path $repoRoot "Content\Projectiles\Necromancer\SkeletonMinion.cs"

$base = Get-Content -Raw $basePath
$skeleton = Get-Content -Raw $skeletonPath

if ($base -notmatch "GetModPlayer<NecromancerPlayer>" -or
    $base -notmatch "IsActiveNecromancer\(\)") {
    throw "BaseNecroMinion should use NecromancerPlayer.IsActiveNecromancer() so minions require the matching active Summoner Soul."
}

if ($base -match "GetModPlayer<SubclassPlayer>") {
    throw "BaseNecroMinion should not read SubclassPlayer.CurrentSubclass directly; that bypasses active Soul gating."
}

# The shared AI (owner validity + follow/chase) now lives in BaseNecroMinion; the base
# kills the minion when the owner is gone or no longer a Necromancer.
if ($base -notmatch "!player\.active" -or $base -notmatch "Projectile\.Kill\(\)") {
    throw "BaseNecroMinion.AI should kill the minion when the owner is gone / not a Necromancer."
}

if ($base -notmatch "Projectile\.minion\s*=\s*true") {
    throw "BaseNecroMinion should register undead as minion projectiles."
}

# SkeletonMinion is now a thin stat definition over the shared base.
if ($skeleton -notmatch ":\s*BaseNecroMinion" -or $skeleton -notmatch "ReservePercent") {
    throw "SkeletonMinion should extend BaseNecroMinion and set its ReservePercent."
}

Write-Host "Necromancer minion AI source smoke test passed."
