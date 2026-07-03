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

if ($skeleton -notmatch "base\.AI\(\);[\s\S]*if \(!Projectile\.active\)[\s\S]*return;") {
    throw "SkeletonMinion.AI should stop after base.AI() kills the projectile."
}

if ($skeleton -notmatch "Projectile\.minion\s*=\s*true") {
    throw "SkeletonMinion should remain registered as a minion projectile."
}

Write-Host "Necromancer minion AI source smoke test passed."
