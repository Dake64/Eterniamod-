$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiRoot = Join-Path $repoRoot "Content\UI"

$soulSystem = Get-Content -Raw (Join-Path $uiRoot "SoulUISystem.cs")
$statsUi = Get-Content -Raw (Join-Path $uiRoot "StatsUI.cs")
$passiveUi = Get-Content -Raw (Join-Path $uiRoot "PassiveUI.cs")
$eterniaUi = Get-Content -Raw (Join-Path $uiRoot "EterniaUI.cs")

if ($soulSystem -notmatch "public override void Unload\(\)") {
    throw "SoulUISystem should clear static UI state in Unload for reliable tModLoader reloads."
}

foreach ($field in @(
    "Visible",
    "SoulInterface",
    "SoulUI",
    "ExpInterface",
    "ExpBarUI",
    "ElementalistInterface",
    "ElementalistUI",
    "CursedMageInterface",
    "CursedMageUI",
    "NecromancerInterface",
    "NecromancerUI")) {
    if ($field -eq "Visible") {
        if ($soulSystem -notmatch "Visible\s*=\s*false;") {
            throw "SoulUISystem.Unload should reset Visible to false."
        }
    } elseif ($soulSystem -notmatch "$field\s*=\s*null;") {
        throw "SoulUISystem.Unload should clear $field."
    }
}

foreach ($pair in @(
    @{Name="StatsUI"; Content=$statsUi},
    @{Name="PassiveUI"; Content=$passiveUi})) {
    if ($pair.Content -notmatch "public override void Unload\(\)" -or
        $pair.Content -notmatch "Visible\s*=\s*false;") {
        throw "$($pair.Name) should reset its static Visible toggle in Unload."
    }
}

if ($eterniaUi -notmatch "CloseMajorPanelsExcept" -or
    $eterniaUi -notmatch "MajorPanel\.Soul" -or
    $eterniaUi -notmatch "MajorPanel\.Stats" -or
    $eterniaUi -notmatch "MajorPanel\.Passive") {
    throw "EterniaUI should provide CloseMajorPanelsExcept for mutually exclusive major panels."
}

foreach ($pair in @(
    @{Name="StatsUI"; Content=$statsUi; Panel="MajorPanel.Stats"},
    @{Name="PassiveUI"; Content=$passiveUi; Panel="MajorPanel.Passive"},
    @{Name="SoulUISystem"; Content=$soulSystem; Panel="MajorPanel.Soul"})) {
    $expected = "EterniaUI\.CloseMajorPanelsExcept\(\s*EterniaUI\.$($pair.Panel)\s*\)"

    if ($pair.Content -notmatch $expected) {
        throw "$($pair.Name) should call EterniaUI.CloseMajorPanelsExcept(EterniaUI.$($pair.Panel)) when opened."
    }
}

$soulUi = Get-Content -Raw (Join-Path $uiRoot "SoulUI.cs")

if ($soulUi -notmatch "Main\.mouseLeftRelease") {
    throw "SoulUI dragging should require a fresh mouse press, not any held left click."
}

Write-Host "UI lifecycle source smoke test passed."
