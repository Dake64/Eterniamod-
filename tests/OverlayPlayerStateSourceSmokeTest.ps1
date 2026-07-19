$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiRoot = Join-Path $repoRoot "Content\UI"
$shared = Get-Content -Raw (Join-Path $uiRoot "EterniaUI.cs")

if ($shared -notmatch "ShouldDrawPlayerUI\(") {
    throw "EterniaUI should provide ShouldDrawPlayerUI for consistent overlay active/dead/menu gating."
}

$overlayFiles = @(
    "ArcherFocusUI.cs",
    "BerserkerUI.cs",
    "ClassProgressionUI.cs",
    "CursedMageUI.cs",
    "ElementalistUI.cs",
    "EnergyHeatUI.cs",
    "ExpBarUI.cs",
    "FighterComboUI.cs",
    "GunnerUI.cs",
    "NecromancerUI.cs",
    "StunnerChargeUI.cs",
    "VirtuosoUI.cs"
)

# A readout drawn over the player sits in the middle of the screen, which is exactly where the
# full-size panels open. Those must use the stricter gate or they render ON TOP of the Boss
# Codex -- drawing order alone never accounted for one covering the other.
$worldSpaceOverlays = @(
    "AdvancedSummonerUI.cs",
    "BeastTamerUI.cs",
    "BerserkerUI.cs",
    "CrimsonTrailUI.cs",
    "SubclassResourceUI.cs",
    "TechSummonerUI.cs"
)

foreach ($file in $worldSpaceOverlays) {
    $content = Get-Content -Raw (Join-Path $uiRoot $file)

    if ($content -notmatch "EterniaUI\.ShouldDrawWorldOverlay\(player\)") {
        throw "$file draws over the player, so it must use ShouldDrawWorldOverlay or it covers open panels."
    }
}

# The panels gate themselves on ShouldDrawPlayerUI, so that one must NOT consider whether a
# panel is open -- otherwise every panel hides itself the moment it is opened.
$toolkit = Get-Content -Raw (Join-Path $uiRoot "EterniaUI.cs")

$basicGate = [regex]::Match(
    $toolkit,
    'public static bool ShouldDrawPlayerUI\(Player player\)[\s\S]+?\n\s{8}\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($basicGate -match "AnyMajorPanelOpen") {
    throw "ShouldDrawPlayerUI must not check for open panels; the panels themselves depend on it."
}

foreach ($file in $overlayFiles) {
    $path = Join-Path $uiRoot $file
    $content = Get-Content -Raw $path

    # ShouldDrawWorldOverlay is the stricter gate for readouts drawn over the player in the
    # world: it calls ShouldDrawPlayerUI and additionally hides them while a full-size panel
    # (Boss Codex, passive tree...) is covering the screen. Either satisfies this check.
    if ($content -notmatch "EterniaUI\.ShouldDraw(PlayerUI|WorldOverlay)\(player\)") {
        throw "$file should gate on EterniaUI.ShouldDrawPlayerUI/ShouldDrawWorldOverlay before drawing player-bound overlay state."
    }
}

$runtimeOverlayChecks = @{
    "ArcherFocusUI.cs" = "IsActiveArcher"
    "BerserkerUI.cs" = "IsActiveBerserker"
    "CursedMageUI.cs" = "IsActiveCursedMage"
    "ElementalistUI.cs" = "IsActiveElementalist"
    "EnergyHeatUI.cs" = "IsActiveEnergyGunner"
    "FighterComboUI.cs" = "IsActiveFighter"
    "GunnerUI.cs" = "IsActiveGunner"
    "NecromancerUI.cs" = "IsActiveNecromancer"
    "StunnerChargeUI.cs" = "IsActiveStunner"
    "VirtuosoUI.cs" = "IsActiveVirtuoso"
}

foreach ($entry in $runtimeOverlayChecks.GetEnumerator()) {
    $content = Get-Content -Raw (Join-Path $uiRoot $entry.Key)

    if ($content -notmatch "$($entry.Value)\(\)") {
        throw "$($entry.Key) should use $($entry.Value)() so stale CurrentSubclass values cannot draw overlays without the matching active Soul."
    }
}

# Base classes no longer have a combat resource (Momentum/Charge/Focus/Bond were
# removed), so there is no BaseClassResourceUI overlay left to gate.

Write-Host "Overlay player state source smoke test passed."
