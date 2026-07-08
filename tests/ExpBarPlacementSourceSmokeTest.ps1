$ErrorActionPreference = "Stop"

# The EXP bar used to sit at the bottom-left, where the chat (and its level-up
# messages) cover it. It should live at the top-center, clear of the chat
# (bottom-left), minimap (top-right) and boss health bar (bottom-center).

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiRoot = Join-Path $repoRoot "Content\UI"
$shared = Get-Content -Raw (Join-Path $uiRoot "EterniaUI.cs")

if ($shared -notmatch "public static Rectangle GetTopCenterPanel\(") {
    throw "EterniaUI should expose a GetTopCenterPanel helper."
}

# None of the HUD panels should live at the bottom-left where the chat covers them.
$hudPanels = @(
    "ExpBarUI",
    "ClassProgressionUI",
    "CursedMageUI",
    "ElementalistUI",
    "NecromancerUI")

foreach ($ui in $hudPanels) {
    $content = Get-Content -Raw (Join-Path $uiRoot "$ui.cs")

    if ($content -match "GetBottomLeftPanel") {
        throw "$ui should not sit at the bottom-left where the chat covers it."
    }
}

# The always-on progression HUD (EXP + class) sits at the top (row or center).
foreach ($ui in @("ExpBarUI", "ClassProgressionUI")) {
    $content = Get-Content -Raw (Join-Path $uiRoot "$ui.cs")

    if ($content -notmatch "GetTop(Center|Row)Panel") {
        throw "$ui should use a top placement so it stays visible."
    }
}

Write-Host "EXP bar placement source smoke test passed."
