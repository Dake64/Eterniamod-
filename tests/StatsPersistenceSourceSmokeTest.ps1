$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$statsPath = Join-Path $repoRoot "Content\Players\EterniaStatsPlayer.cs"
$content = Get-Content -Raw $statsPath

$fields = @(
    "Vitality",
    "Power",
    "Precision",
    "Agility",
    "Focus",
    "BleedAffinity",
    "ComboAffinity",
    "DefenseAffinity",
    "PrecisionAffinity",
    "RageAffinity",
    "ControlAffinity",
    "EnergyAffinity",
    "BowAffinity",
    "GunAffinity",
    "MusicAffinity",
    "ElementalAffinity",
    "CurseAffinity",
    "InfinityAffinity",
    "ArcaneAffinity",
    "BeastAffinity",
    "FusionAffinity",
    "TechAffinity",
    "ShadowAffinity",
    "StatPoints"
)

foreach ($field in $fields) {
    $saveNeedle = "tag[`"$field`"] = $field"
    $loadNeedle = "$field = tag.GetInt(`"$field`")"

    if (-not $content.Contains($saveNeedle)) {
        throw "EterniaStatsPlayer.SaveData should persist $field."
    }

    if (-not $content.Contains($loadNeedle)) {
        throw "EterniaStatsPlayer.LoadData should restore $field."
    }
}

if (-not $content.Contains('tag["UnlockedPassives"] = UnlockedPassives')) {
    throw "EterniaStatsPlayer.SaveData should persist UnlockedPassives."
}

if (-not $content.Contains('tag.Get<List<string>>("UnlockedPassives")')) {
    throw "EterniaStatsPlayer.LoadData should restore UnlockedPassives."
}

Write-Host "Stats persistence source smoke test passed."
