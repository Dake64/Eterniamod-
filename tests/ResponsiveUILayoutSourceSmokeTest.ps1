$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiRoot = Join-Path $repoRoot "Content\UI"

$shared = Get-Content -Raw (Join-Path $uiRoot "EterniaUI.cs")
$passive = Get-Content -Raw (Join-Path $uiRoot "PassiveUI.cs")
$stats = Get-Content -Raw (Join-Path $uiRoot "StatsUI.cs")
$soul = Get-Content -Raw (Join-Path $uiRoot "SoulUI.cs")
$exp = Get-Content -Raw (Join-Path $uiRoot "ExpBarUI.cs")
$progression = Get-Content -Raw (Join-Path $uiRoot "ClassProgressionUI.cs")

if ($shared -notmatch "static void DrawWrappedText\(") {
    throw "EterniaUI should provide DrawWrappedText so long UI copy can wrap instead of overflowing."
}

if ($shared -match "Math\.Max\(320,\s*Main\.screenWidth - margin \* 2\)" -or
    $shared -match "Math\.Max\(260,\s*Main\.screenWidth - marginX \* 2\)") {
    throw "EterniaUI panel helpers should not force fixed minimum widths past the available screen width."
}

if ($shared -notmatch "availableWidth" -or
    $shared -notmatch "minWidth") {
    throw "EterniaUI panel helpers should derive responsive availableWidth/minWidth values."
}

if ($shared -notmatch "int width =\s*Math\.Min\(\s*rect\.Width,\s*maxWidth\)" -or
    $shared -notmatch "int height =\s*Math\.Min\(\s*rect\.Height,\s*maxHeight\)") {
    throw "EterniaUI.ClampToScreen should shrink oversized rectangles before clamping their position."
}

if ($shared -notmatch "panelHeight" -or
    $shared -notmatch "screenMaxY") {
    throw "EterniaUI.GetBottomLeftPanel should shrink height and clamp y for very small screens."
}

if ($passive -match "const int SidebarWidth" -or
    $passive -notmatch "GetPassiveAreas\(" -or
    $passive -notmatch "compactLayout") {
    throw "PassiveUI should derive sidebar/tree rectangles responsively and support compact layouts."
}

if ($passive -match "Math\.Max\(140,\s*content\.Bottom - sidebar\.Bottom - 14\)") {
    throw "PassiveUI compact tree area should not force a minimum height that can overflow the panel."
}

if ($passive -notmatch "remainingTreeHeight") {
    throw "PassiveUI compact layout should calculate remainingTreeHeight before creating the tree area."
}

if ($passive -notmatch "treeArea\.Width <= 0" -or
    $passive -notmatch "treeArea\.Height <= 0") {
    throw "PassiveUI BuildLayouts should guard against empty or collapsed tree areas."
}

if ($passive -notmatch "hubRadius") {
    throw "PassiveUI should lay the tree out as a radial web (hubRadius) instead of a grid, so it scales without shrinking nodes."
}

if ($passive -notmatch "TierStep\(") {
    throw "PassiveUI radial web should march nodes outward by tier (TierStep per node kind) along each affinity spoke."
}

if ($passive -notmatch "panX") {
    throw "PassiveUI tree should be a pannable canvas (panX) so the player can drag around a large web."
}

if ($passive -match "System\.Math\.Max\(NodeMinHeight, nodeHeight\)") {
    throw "PassiveUI should not force NodeMinHeight without checking whether the group can fit it."
}

if ($passive -match "rect\.Width - 76") {
    throw "PassiveUI compact nodes should not reserve a fixed button column that can erase passive names."
}

if ($passive -notmatch "compactButtonHeight") {
    throw "PassiveUI compact nodes should derive button height from the node rectangle."
}

if ($passive -notmatch "affinityRows" -or
    $passive -notmatch "affinityTop") {
    throw "PassiveUI compact affinity bars should calculate rows/top from available sidebar space."
}

if ($stats -match "pointBar\.X \+ 150" -or
    $stats -match "pointBar\.X \+ 286" -or
    $stats -notmatch "DrawSummaryPills\(") {
    throw "StatsUI summary pills should be laid out dynamically, not with fixed X offsets that overflow narrow panels."
}

if ($stats -match "int rowHeight = 66" -or
    $stats -match "int y = panel\.Y \+ 132") {
    throw "StatsUI stat rows should derive y/height from available panel space, not fixed row coordinates."
}

if ($stats -notmatch "availableRowsHeight" -or
    $stats -notmatch "StatRow\[\] rows") {
    throw "StatsUI should build stat rows from data and fit them into the current panel height."
}

if ($soul -notmatch "EterniaUI\.DrawWrappedText") {
    throw "SoulUI should wrap explanatory text for Empty Soul and other long copy."
}

if ($progression -notmatch "EterniaUI\.DrawTrimmedText") {
    throw "ClassProgressionUI should trim the class path label to prevent overflow."
}

if ($exp -match "GetBottomLeftPanel\(\s*368,\s*48,") {
    throw "ExpBarUI panel height should be at least the shared panel top band height to avoid overdraw."
}

Write-Host "Responsive UI layout source smoke test passed."
