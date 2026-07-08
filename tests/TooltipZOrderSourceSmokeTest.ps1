$ErrorActionPreference = "Stop"

# Hover tooltips in the Stats and Passive panels were drawn inline while iterating
# rows/nodes, so later-drawn rows/panels covered them (tooltip appeared behind).
# Tooltips must be QUEUED during the pass and drawn last (on top) via EterniaUI's
# deferred tooltip helper.

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiRoot = Join-Path $repoRoot "Content\UI"

$eternia = Get-Content -Raw (Join-Path $uiRoot "EterniaUI.cs")

if ($eternia -notmatch "public static void QueueTooltip\(" -or
    $eternia -notmatch "public static void DrawQueuedTooltip\(") {
    throw "EterniaUI should provide a deferred tooltip (QueueTooltip + DrawQueuedTooltip) so tooltips draw on top."
}

foreach ($ui in @("StatsUI", "PassiveUI")) {
    $content = Get-Content -Raw (Join-Path $uiRoot "$ui.cs")

    if ($content -notmatch "EterniaUI\.QueueTooltip\(") {
        throw "$ui should queue its hover tooltip via EterniaUI.QueueTooltip."
    }

    if ($content -notmatch "EterniaUI\.DrawQueuedTooltip\(") {
        throw "$ui should flush the queued tooltip via EterniaUI.DrawQueuedTooltip so it draws on top."
    }

    if ($content -match "EterniaUI\.DrawTooltip\(") {
        throw "$ui should not draw tooltips inline mid-pass (use the deferred QueueTooltip)."
    }
}

Write-Host "Tooltip z-order source smoke test passed."
