$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$pathsToScan = @(
    (Join-Path $repoRoot "Content"),
    (Join-Path $repoRoot "Localization"),
    (Join-Path $repoRoot "en-US.hjson")
)

$removedTerms = @(
    "Cartomancer",
    "CartomancerSoul",
    "CardAffinity",
    "NecroAffinity",
    "ApprenticeDeck",
    "BasicCardProjectile",
    "StrikeCardProjectile"
)

foreach ($path in $pathsToScan) {
    if (!(Test-Path $path)) {
        continue
    }

    $files = @()

    if ((Get-Item $path).PSIsContainer) {
        $files = Get-ChildItem -LiteralPath $path -Recurse -File |
            Where-Object {
                $_.Extension -in @(".cs", ".hjson", ".json", ".txt")
            }
    }
    else {
        $files = @(Get-Item $path)
    }

    foreach ($file in $files) {
        $content = Get-Content -Raw $file.FullName

        foreach ($term in $removedTerms) {
            if ($content -match [regex]::Escape($term)) {
                throw "Removed Cartomancer residue '$term' found in $($file.FullName)."
            }
        }
    }
}

$removedAssetNames = @(
    "ApprenticeDeck.png",
    "BasicCardProjectile.png",
    "StrikeCardProjectile.png"
)

foreach ($assetName in $removedAssetNames) {
    $matches = Get-ChildItem -LiteralPath (Join-Path $repoRoot "Content") -Recurse -File |
        Where-Object { $_.Name -eq $assetName }

    if ($matches) {
        throw "Removed Cartomancer asset still exists: $($matches[0].FullName)."
    }
}

Write-Host "Removed Cartomancer residue smoke test passed."
