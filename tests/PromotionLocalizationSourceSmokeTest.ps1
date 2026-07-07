$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$rewardPath = Join-Path $repoRoot "Content\Players\PromotionRewardPlayer.cs"
$localizationPath = Join-Path $repoRoot "en-US.hjson"

$reward = Get-Content -Raw $rewardPath
$localization = Get-Content -Raw $localizationPath

$expectedItems = @(
    "BloodletterBlade",
    "TrainingGauntlet",
    "TrainingShield",
    "PracticeYoyo",
    "RageCleaver",
    "ImpactMace",
    "EnergySidearm",
    "Longbow",
    "TrainingPistol",
    "ResonantCrossbow",
    "ElementalApprenticeStaff",
    "CursedApprenticeTome",
    "InfinityTome",
    "ArcaneFocus",
    "BeastWhip",
    "FusionWhip",
    "TechWhip",
    "BeginnerNecromancyBook"
)

foreach ($item in $expectedItems) {
    if ($reward -notmatch [regex]::Escape($item)) {
        throw "PromotionRewardPlayer should still grant '$item'."
    }

    if ($localization -notmatch "(?ms)\b$([regex]::Escape($item)):\s*\{[^}]*DisplayName:") {
        throw "Missing DisplayName localization for promotion item '$item'."
    }
}

Write-Host "Promotion localization source smoke test passed."
