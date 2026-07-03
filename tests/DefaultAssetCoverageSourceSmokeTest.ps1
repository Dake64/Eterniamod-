$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$contentRoot = Join-Path $repoRoot "Content"

$assetBearingBases = @(
    "ModItem",
    "ModProjectile",
    "ModBuff",
    "ModNPC",
    "ClassSoulItem",
    "BaseNecroMinion"
)

$textureOverrideBases = @(
    "SubclassLockedItem",
    "BaseEterniaWhipProjectile"
)

Get-ChildItem -Path $contentRoot -Recurse -File -Filter *.cs |
    ForEach-Object {
        $content = Get-Content -Raw $_.FullName

        $matches =
            [regex]::Matches(
                $content,
                '(?m)public\s+(abstract\s+)?class\s+(\w+)\s*:\s*(\w+)')

        foreach ($match in $matches) {
            $isAbstract =
                -not [string]::IsNullOrEmpty($match.Groups[1].Value)

            if ($isAbstract) {
                continue
            }

            $className =
                $match.Groups[2].Value

            $baseName =
                $match.Groups[3].Value

            if ($textureOverrideBases -contains $baseName) {
                continue
            }

            if ($assetBearingBases -notcontains $baseName) {
                continue
            }

            if ($content -match 'override\s+string\s+Texture\s*=>') {
                continue
            }

            $expectedPng =
                [System.IO.Path]::ChangeExtension($_.FullName, ".png")

            if (-not (Test-Path $expectedPng)) {
                $relative =
                    Resolve-Path -Path $_.FullName -Relative

                throw "$className in $relative depends on a default texture but '$expectedPng' is missing."
            }
        }
    }

Write-Host "Default asset coverage source smoke test passed."
