$ErrorActionPreference = "Stop"

# The EXP bar used to sit at the bottom-left, where the chat (and its level-up
# messages) cover it. It should live at the top-center, clear of the chat
# (bottom-left), minimap (top-right) and boss health bar (bottom-center).

$repoRoot = Split-Path -Parent $PSScriptRoot
$exp = Get-Content -Raw (Join-Path $repoRoot "Content\UI\ExpBarUI.cs")
$shared = Get-Content -Raw (Join-Path $repoRoot "Content\UI\EterniaUI.cs")

if ($shared -notmatch "public static Rectangle GetTopCenterPanel\(") {
    throw "EterniaUI should expose a GetTopCenterPanel helper."
}

if ($exp -match "GetBottomLeftPanel") {
    throw "The EXP bar should not sit at the bottom-left where the chat covers it."
}

if ($exp -notmatch "GetTopCenterPanel") {
    throw "The EXP bar should use the top-center placement so it stays visible."
}

Write-Host "EXP bar placement source smoke test passed."
