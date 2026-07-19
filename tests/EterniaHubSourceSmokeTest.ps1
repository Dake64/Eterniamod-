$ErrorActionPreference = "Stop"

# The mod had grown four panels behind four separate keys. The hub turns them into pages of
# one book WITHOUT rewriting any of them: the tab strip is a layer over the existing panels,
# so consolidating navigation cannot break their contents.

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiRoot = Join-Path $repoRoot "Content\UI"

$hub = Get-Content -Raw (Join-Path $uiRoot "EterniaHubUI.cs")
$toolkit = Get-Content -Raw (Join-Path $uiRoot "EterniaUI.cs")
$keybinds = Get-Content -Raw (Join-Path $repoRoot "Content\Systems\EterniaKeybinds.cs")

# --- One key opens everything -------------------------------------------------
if ($keybinds -notmatch "ToggleEterniaMenu") {
    throw "There should be a single keybind that opens the hub."
}

if ($hub -notmatch "ToggleEterniaMenu\.JustPressed") {
    throw "The hub should react to its own keybind."
}

# The per-panel keys stay as direct shortcuts -- removing them would break muscle memory for
# no gain, since the hub is about not NEEDING four keys, not about forbidding them.
foreach ($shortcut in @("ToggleSoulUI", "ToggleStatsUI", "TogglePassiveUI", "ToggleBossLog")) {
    if ($keybinds -notmatch $shortcut) {
        throw "The direct shortcut '$shortcut' should still exist alongside the hub key."
    }
}

# --- Every panel is reachable as a tab ----------------------------------------
foreach ($page in @("Soul", "Stats", "Passive", "Bosses")) {
    if ($hub -notmatch "MajorPanel\.$page") {
        throw "The hub is missing a tab for MajorPanel.$page."
    }
}

# Opening the Soul panel needs its UIState pushed too; setting the flag alone shows an empty
# panel, so the hub must route through the system rather than poking Visible directly.
if ($hub -notmatch "SoulUISystem\.OpenSoulPanel\(\)") {
    throw "The hub should open the Soul panel through OpenSoulPanel, not by setting Visible."
}

if ($hub -match "SoulUISystem\.Visible = true") {
    throw "Setting SoulUISystem.Visible directly opens an empty panel; use OpenSoulPanel."
}

# --- Switching tabs must not stack panels -------------------------------------
if ($hub -notmatch "CloseMajorPanelsExcept") {
    throw "Opening a page should close the others, or panels pile on top of each other."
}

if ($toolkit -notmatch "public static void CloseAllMajorPanels") {
    throw "The toolkit should expose CloseAllMajorPanels so the hub key can close the book."
}

# --- The strip must not move between tabs -------------------------------------
# A navigation bar that jumps as you switch pages is worse than none, so it anchors to the
# tallest page rather than to whichever one happens to be open.
$strip = [regex]::Match(
    $hub,
    'public static Rectangle TabStrip\(\)[\s\S]+?\n\s{8}\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($strip -notmatch "GetCenteredPanel\(1120, 628") {
    throw "The tab strip should anchor to the tallest page so it stays put across tabs."
}

# --- It is a layer, not a rewrite ---------------------------------------------
# If the hub ever starts drawing panel CONTENT, the safety of this approach is gone.
foreach ($panel in @("StatsUI.cs", "PassiveUI.cs", "BossLogUI.cs")) {
    $src = Get-Content -Raw (Join-Path $uiRoot $panel)

    if ($src -match "EterniaHubUI") {
        throw "$panel should not know about the hub; the hub layers over it, not the reverse."
    }
}

Write-Host "Eternia hub source smoke test passed."
