$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$localizationPaths = @(
    (Join-Path $repoRoot "Localization\en-US_Mods.ETERNIA.hjson"),
    (Join-Path $repoRoot "en-US.hjson")
)

foreach ($path in $localizationPaths) {
    $content = Get-Content -Raw $path

    foreach ($item in @("WeakCurse", "BloodCurse")) {
        if ($content -notmatch "(?ms)\b$([regex]::Escape($item)):\s*\{[^}]*DisplayName:") {
            throw "$path should localize $item DisplayName."
        }

        if ($content -notmatch "(?ms)\b$([regex]::Escape($item)):\s*\{[^}]*Tooltip:\s*[^`"`r`n][^`r`n]*") {
            throw "$path should give $item a non-empty Tooltip explaining its curse tradeoff."
        }
    }

    $starterExpectations = @{
        "TrainingBlade" = "Momentum"
        "ApprenticeWand" = "Charge"
        "TrainingBow" = "Focus"
        "TrainingWhip" = "Bond"
    }

    foreach ($entry in $starterExpectations.GetEnumerator()) {
        if ($content -notmatch "(?ms)\b$([regex]::Escape($entry.Key)):\s*\{[^}]*Tooltip:\s*[^`r`n]*$([regex]::Escape($entry.Value))") {
            throw "$path should explain that $($entry.Key) builds $($entry.Value)."
        }
    }
}

Write-Host "Localization content source smoke test passed."
