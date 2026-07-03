$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$contentRoot = Join-Path $repoRoot "Content"

$sourceFiles = Get-ChildItem -Path $contentRoot -Recurse -File -Filter *.cs
$joinedSource = ($sourceFiles | ForEach-Object { Get-Content -Raw $_.FullName }) -join "`n"

if ($joinedSource -cmatch '"Eternia/') {
    throw "Texture paths should use the canonical mod name prefix ETERNIA/, not Eternia/."
}

$noSoul = Get-Content -Raw (Join-Path $contentRoot "Buffs\NoSoulDebuff.cs")
$violation = Get-Content -Raw (Join-Path $contentRoot "Buffs\SoulViolationDebuff.cs")

foreach ($pair in @(
    @{Name="NoSoulDebuff"; Content=$noSoul},
    @{Name="SoulViolationDebuff"; Content=$violation})) {
    if ($pair.Content -notmatch 'override string Texture =>' -or
        $pair.Content -notmatch '"ETERNIA/Content/Buffs/SoulLessDebuff"') {
        throw "$($pair.Name) should reuse the existing SoulLessDebuff texture through an uppercase ETERNIA/ resource path."
    }
}

$subclassLocked = Get-Content -Raw (Join-Path $contentRoot "Items\Weapons\Promotion\SubclassLockedItem.cs")
if ($subclassLocked -notmatch 'public override string Texture => TexturePath') {
    throw "Promotion weapons should inherit a Texture override from SubclassLockedItem."
}

Get-ChildItem -Path (Join-Path $contentRoot "Items\Weapons\Promotion") -File -Filter *.cs |
    Where-Object { $_.Name -notin @("SubclassLockedItem.cs", "SubclassLockHelper.cs") } |
    ForEach-Object {
        $content = Get-Content -Raw $_.FullName

        if ($content -notmatch 'protected override string TexturePath =>') {
            throw "$($_.Name) should provide TexturePath because promotion weapons currently reuse existing sprites."
        }

        if ($content -notmatch '"ETERNIA/Content/Items/') {
            throw "$($_.Name) should point TexturePath at a canonical ETERNIA/ item texture."
        }
    }

$yoyoProjectile = Get-Content -Raw (Join-Path $contentRoot "Projectiles\Promotion\PracticeYoyoProjectile.cs")
if ($yoyoProjectile -notmatch 'override string Texture =>' -or
    $yoyoProjectile -notmatch '"ETERNIA/Content/Items/Weapons/Fighter/TrainingGauntlet"') {
    throw "PracticeYoyoProjectile should reuse an existing texture through an explicit ETERNIA/ Texture override."
}

$whipBase = Get-Content -Raw (Join-Path $contentRoot "Projectiles\Summoner\BaseEterniaWhipProjectile.cs")
if ($whipBase -notmatch 'override string Texture =>' -or
    $whipBase -notmatch '"ETERNIA/Content/Items/Souls/SummonerSoul"') {
    throw "Summoner whip projectiles should inherit a texture override from BaseEterniaWhipProjectile."
}

[regex]::Matches($joinedSource, '"ETERNIA/Content/[^"]+"') |
    ForEach-Object {
        $resourcePath =
            $_.Value.Trim('"').Substring("ETERNIA/".Length)

        $pngPath =
            Join-Path $repoRoot ($resourcePath + ".png")

        if (-not (Test-Path $pngPath)) {
            throw "Texture override points at missing resource: $($_.Value)."
        }
    }

Write-Host "Resource texture path source smoke test passed."
