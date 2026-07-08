$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiRoot = Join-Path $repoRoot "Content\UI"

$sharedUiPath = Join-Path $uiRoot "EterniaUI.cs"
$passiveUiPath = Join-Path $uiRoot "PassiveUI.cs"
$statsUiPath = Join-Path $uiRoot "StatsUI.cs"
$soulUiPath = Join-Path $uiRoot "SoulUI.cs"
$expUiPath = Join-Path $uiRoot "ExpBarUI.cs"
$progressionUiPath = Join-Path $uiRoot "ClassProgressionUI.cs"

if (-not (Test-Path $sharedUiPath)) {
    throw "UI rework should centralize shared drawing primitives in Content\UI\EterniaUI.cs."
}

$shared = Get-Content -Raw $sharedUiPath
$passive = Get-Content -Raw $passiveUiPath
$stats = Get-Content -Raw $statsUiPath
$soul = Get-Content -Raw $soulUiPath
$exp = Get-Content -Raw $expUiPath
$progression = Get-Content -Raw $progressionUiPath

foreach ($method in @(
    "GetCenteredPanel",
    "DrawPanel",
    "DrawButton",
    "DrawTooltip",
    "DrawProgressBar",
    "DrawConnector",
    "DrawPill")) {
    if ($shared -notmatch "static .* $method\(") {
        throw "EterniaUI should expose $method as a shared primitive."
    }
}

if ($passive -match "new Rectangle\(\s*700\s*,\s*120" -or
    $passive -match "panel\.X\s*\+\s*passive\.X" -or
    $passive -match "panel\.Y\s*\+\s*passive\.Y") {
    throw "PassiveUI should use responsive calculated layout, not fixed panel coordinates or PassiveNode X/Y offsets."
}

if ($passive -notmatch "GroupPassivesByAffinity" -or
    $passive -notmatch "DrawPassiveNode" -or
    $passive -notmatch "GetPassiveState" -or
    $passive -notmatch "EterniaUI\.QueueTooltip") {
    throw "PassiveUI should group passives by affinity and draw stateful nodes with deferred wrapped tooltips."
}

foreach ($pair in @(
    @{Name="StatsUI"; Content=$stats},
    @{Name="SoulUI"; Content=$soul},
    @{Name="ExpBarUI"; Content=$exp},
    @{Name="ClassProgressionUI"; Content=$progression})) {
    if ($pair.Content -notmatch "EterniaUI\.DrawPanel") {
        throw "$($pair.Name) should use the shared EterniaUI panel language."
    }
}

if ($stats -notmatch "EterniaUI\.DrawButton" -or
    $exp -notmatch "EterniaUI\.DrawProgressBar" -or
    $progression -notmatch "EterniaUI\.DrawPill") {
    throw "Stats, EXP, and progression UI should use shared buttons, progress bars, and pills."
}

$allowedWithoutToolkit = @(
    "EterniaUI.cs",
    "SoulUISystem.cs"
)

Get-ChildItem -File $uiRoot -Filter "*.cs" | ForEach-Object {
    if ($allowedWithoutToolkit -contains $_.Name) {
        return
    }

    $fileContent = Get-Content -Raw $_.FullName

    if ($fileContent -notmatch "EterniaUI\.") {
        throw "$($_.Name) should use the shared EterniaUI toolkit."
    }

    if ($fileContent -match "Color\.Black \* 0\.7f") {
        throw "$($_.Name) should not use the old prototype black overlay color directly."
    }

    if ($fileContent -match "Color\.Black \*") {
        throw "$($_.Name) should use the shared EterniaUI palette instead of raw Color.Black tinting."
    }
}

Write-Host "UI rework source smoke test passed."
