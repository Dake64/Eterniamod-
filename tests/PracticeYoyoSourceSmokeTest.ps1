$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$itemPath = Join-Path $repoRoot "Content\Items\Weapons\Promotion\PracticeYoyo.cs"
$projectilePath = Join-Path $repoRoot "Content\Projectiles\Promotion\PracticeYoyoProjectile.cs"
$yoyoPlayerPath = Join-Path $repoRoot "Content\Players\YoyoMasterPlayer.cs"

$item = Get-Content -Raw $itemPath
$projectile = Get-Content -Raw $projectilePath
$yoyoPlayer = Get-Content -Raw $yoyoPlayerPath

if ($yoyoPlayer -notmatch "ProjAIStyleID\.Yoyo") {
    throw "YoyoMasterPlayer should key its mechanic off real yoyo projectiles."
}

foreach ($required in @(
    "PracticeYoyoProjectile",
    "ItemUseStyleID.Shoot",
    "Item.noMelee = true",
    "Item.noUseGraphic = true",
    "Item.channel = true",
    "Item.shootSpeed = 12f"
)) {
    if ($item -notmatch [regex]::Escape($required)) {
        throw "PracticeYoyo should configure real yoyo item behavior: missing '$required'."
    }
}

foreach ($required in @(
    "ProjAIStyleID.Yoyo",
    "ProjectileID.Sets.YoyosLifeTimeMultiplier",
    "ProjectileID.Sets.YoyosMaximumRange",
    "ProjectileID.Sets.YoyosTopSpeed",
    "DamageClass.MeleeNoSpeed"
)) {
    if ($projectile -notmatch [regex]::Escape($required)) {
        throw "PracticeYoyoProjectile should configure real yoyo projectile behavior: missing '$required'."
    }
}

Write-Host "Practice Yoyo source smoke test passed."
