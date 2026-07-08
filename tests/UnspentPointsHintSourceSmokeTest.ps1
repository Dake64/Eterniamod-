$ErrorActionPreference = "Stop"

# It is easy to forget you have stat/passive points to spend. The EXP bar should
# surface a hint when there are unspent points.

$repoRoot = Split-Path -Parent $PSScriptRoot
$exp = Get-Content -Raw (Join-Path $repoRoot "Content\UI\ExpBarUI.cs")

if ($exp -notmatch "StatPoints") {
    throw "The EXP bar should read unspent stat points (EterniaStatsPlayer.StatPoints)."
}

if ($exp -notmatch "passivePoints") {
    throw "The EXP bar should read unspent passive points."
}

if ($exp -notmatch "to spend") {
    throw "The EXP bar should hint that points are available to spend."
}

Write-Host "Unspent points hint source smoke test passed."
