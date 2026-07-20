$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$sharedPath = Join-Path $repoRoot "Content\UI\EterniaUI.cs"
$content = Get-Content -Raw $sharedPath

if ($content -notmatch "public static bool DrawButton\(") {
    throw "Could not locate EterniaUI.DrawButton."
}

if ($content -notmatch "bool clicked\s*=") {
    throw "DrawButton should calculate a clicked variable before returning."
}

if ($content -notmatch "Main\.mouseLeftRelease\s*=\s*false") {
    throw "DrawButton should consume Main.mouseLeftRelease after a click so one hold cannot trigger repeated UI actions."
}

if ($content -notmatch "public static void DrawTooltip\(") {
    throw "Could not locate EterniaUI.DrawTooltip."
}

if ($content -notmatch "Main\.screenHeight") {
    throw "DrawTooltip should cap tooltip height against Main.screenHeight."
}

if ($content -notmatch "panel\.Bottom") {
    throw "DrawTooltip should stop drawing wrapped lines when the clamped panel is full."
}

if ($content -notmatch "public static List<string> WrapText\(") {
    throw "Could not locate EterniaUI.WrapText."
}

if ($content -notmatch "FitText\(word, maxWidth, scale\)") {
    throw "WrapText should use FitText for overlong single words so tooltips cannot overflow horizontally."
}

$uiRoot = Join-Path $repoRoot "Content\UI"
$stats = Get-Content -Raw (Join-Path $uiRoot "StatsUI.cs")
$passive = Get-Content -Raw (Join-Path $uiRoot "PassiveUI.cs")
$soul = Get-Content -Raw (Join-Path $uiRoot "SoulUI.cs")

if ($content -notmatch "public static bool DrawCloseButton\(") {
    throw "EterniaUI should provide a shared DrawCloseButton helper for dismissible panels."
}

foreach ($pair in @(
    @{Name="StatsUI"; Content=$stats},
    @{Name="PassiveUI"; Content=$passive},
    @{Name="SoulUI"; Content=$soul})) {
    if ($pair.Content -notmatch "EterniaUI\.DrawCloseButton") {
        throw "$($pair.Name) should draw a visible close button."
    }
}

if ($stats -notmatch "Visible\s*=\s*false;" -or
    $passive -notmatch "Visible\s*=\s*false;") {
    throw "StatsUI and PassiveUI close buttons should set Visible to false."
}

if ($soul -notmatch "SoulUISystem\.CloseSoulPanel\(\)") {
    throw "SoulUI close button should close through SoulUISystem.CloseSoulPanel so UserInterface state is cleared."
}

foreach ($pair in @(
    @{Name="StatsUI"; Content=$stats},
    @{Name="PassiveUI"; Content=$passive})) {
    if ($pair.Content -notmatch "EterniaUI\.ShouldDrawPlayerUI\(player\)") {
        throw "$($pair.Name) should use EterniaUI.ShouldDrawPlayerUI(player) before drawing player-bound panel state."
    }
}

if ($soul -notmatch "EterniaUI\.ShouldDrawPlayerUI\(player\)") {
    throw "SoulUI should use EterniaUI.ShouldDrawPlayerUI(player) before drawing player-bound state."
}

# The Soul panel is no longer draggable. It was a floating widget from when it was the mod's
# only panel; now it is a page of the hub, and a page that reopens somewhere different every
# time moves the close button and the tab strip out from under the player's hand.
if ($soul -match "dragHandle" -or $soul -match "dragging") {
    throw "SoulUI should no longer be draggable now that it is a centred page of the hub."
}

if ($soul -notmatch "EterniaUI\.GetCenteredPanel") {
    throw "SoulUI should be centred like every other page."
}

if ($soul -notmatch "panel\.Contains\(Main\.MouseScreen\.ToPoint\(\)\)") {
    throw "SoulUI should still claim mouseInterface while hovered, or clicks fall through to the world."
}

Write-Host "UI interaction hardening source smoke test passed."
