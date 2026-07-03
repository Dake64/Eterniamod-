$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

$whips = @(
    @{
        Item = "Content\Items\Weapons\Summoner\TrainingWhip.cs"
        Projectile = "TrainingWhipProjectile"
    },
    @{
        Item = "Content\Items\Weapons\Promotion\BeastWhip.cs"
        Projectile = "BeastWhipProjectile"
    },
    @{
        Item = "Content\Items\Weapons\Promotion\FusionWhip.cs"
        Projectile = "FusionWhipProjectile"
    },
    @{
        Item = "Content\Items\Weapons\Promotion\TechWhip.cs"
        Projectile = "TechWhipProjectile"
    }
)

foreach ($whip in $whips) {
    $itemPath = Join-Path $repoRoot $whip.Item
    $content = Get-Content -Raw $itemPath

    if ($content -notmatch "DamageClass\.SummonMeleeSpeed") {
        throw "$($whip.Item) should remain a Summoner whip damage item."
    }

    if ($content -notmatch "Item\.noMelee\s*=\s*true" -or
        $content -notmatch "Item\.noUseGraphic\s*=\s*true" -or
        $content -notmatch "Item\.shoot\s*=\s*ModContent\.ProjectileType<\s*$($whip.Projectile)\s*>\(\)" -or
        $content -notmatch "Item\.shootSpeed\s*=\s*4f") {
        throw "$($whip.Item) should use a dedicated whip projectile instead of direct item melee."
    }
}

$projectileRoot = Join-Path $repoRoot "Content\Projectiles\Summoner"

foreach ($projectileName in @(
    "BaseEterniaWhipProjectile",
    "TrainingWhipProjectile",
    "BeastWhipProjectile",
    "FusionWhipProjectile",
    "TechWhipProjectile")) {
    $matches = Get-ChildItem -File $projectileRoot -Filter "$projectileName.cs" -ErrorAction SilentlyContinue

    if (-not $matches) {
        throw "Missing Summoner whip projectile: $projectileName.cs"
    }
}

$baseProjectile = Get-Content -Raw (Join-Path $projectileRoot "BaseEterniaWhipProjectile.cs")

foreach ($required in @(
    "ProjectileID\.Sets\.IsAWhip\[Type\]\s*=\s*true",
    "Projectile\.DefaultToWhip\(\)",
    "Projectile\.WhipSettings\.Segments",
    "Projectile\.WhipSettings\.RangeMultiplier",
    "Projectile\.FillWhipControlPoints",
    "MinionAttackTargetNPC")) {
    if ($baseProjectile -notmatch $required) {
        throw "BaseEterniaWhipProjectile should contain pattern: $required"
    }
}

foreach ($required in @(
    "OwnerCanUseProjectile",
    "public override bool\? CanHitNPC\(NPC target\)",
    "Projectile\.Kill\(\)",
    "ActiveSoul\s*!=\s*SoulId\.Summoner",
    "SubclassLockHelper\.PlayerHasSubclass",
    "protected virtual string RequiredSubclass")) {
    if ($baseProjectile -notmatch $required) {
        throw "BaseEterniaWhipProjectile should gate hit behavior with active Summoner Soul and required promotion pattern: $required"
    }
}

foreach ($projectileName in @(
    "BeastWhipProjectile",
    "FusionWhipProjectile",
    "TechWhipProjectile")) {
    $content = Get-Content -Raw (Join-Path $projectileRoot "$projectileName.cs")

    if ($content -notmatch "protected override string RequiredSubclass") {
        throw "$projectileName should declare its required promotion so stale projectiles stop hitting after class/Soul changes."
    }
}

$localizationFiles = @(
    (Join-Path $repoRoot "Localization\en-US_Mods.ETERNIA.hjson"),
    (Join-Path $repoRoot "en-US.hjson")
)

foreach ($localizationFile in $localizationFiles) {
    $localization = Get-Content -Raw $localizationFile

    foreach ($projectileName in @(
        "TrainingWhipProjectile",
        "BeastWhipProjectile",
        "FusionWhipProjectile",
        "TechWhipProjectile")) {
        if ($localization -notmatch "$projectileName\.DisplayName") {
            throw "Missing localization for $projectileName in $localizationFile."
        }
    }
}

Write-Host "Summoner whip source smoke test passed."
