$ErrorActionPreference = "Stop"

# Player-anchored overlays (class resource bars, combo/heat/rage meters) draw at
# player.Top - Main.screenPosition + a fixed offset. Near a screen edge that puts
# the bar/pills off-screen. Every such overlay must route its anchor through
# EterniaUI.ClampWorldAnchored so the whole overlay stays visible.

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiRoot = Join-Path $repoRoot "Content\UI"

$eternia = Get-Content -Raw (Join-Path $uiRoot "EterniaUI.cs")

if ($eternia -notmatch "public static Vector2 ClampWorldAnchored\(") {
    throw "EterniaUI should expose ClampWorldAnchored for player-anchored overlays."
}

$overlays = @(
    "ArcherFocusUI",
    "BerserkerUI",
    "EnergyHeatUI",
    "GunnerUI",
    "StunnerChargeUI",
    "VirtuosoUI",
    "FighterComboUI"
)

foreach ($overlay in $overlays) {
    $content = Get-Content -Raw (Join-Path $uiRoot "$overlay.cs")

    # Either clamp directly, or route through the shared floating resource bar
    # helper (EterniaUI.DrawFloatingResourceBar), which clamps internally.
    if ($content -notmatch "EterniaUI\.ClampWorldAnchored\(" -and
        $content -notmatch "EterniaUI\.DrawFloatingResourceBar\(") {
        throw "$overlay should clamp its player-anchored draw position via EterniaUI.ClampWorldAnchored (directly or through DrawFloatingResourceBar)."
    }
}

Write-Host "Overlay clamp source smoke test passed."
