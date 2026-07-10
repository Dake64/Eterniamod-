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

foreach ($file in $overlayFiles) {
    $path = Join-Path $uiRoot $file
    $content = Get-Content -Raw $path

    if ($content -notmatch "EterniaUI\.ShouldDrawPlayerUI\(player\)") {
        throw "$file should call EterniaUI.ShouldDrawPlayerUI(player) before drawing player-bound overlay state."
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
