$ErrorActionPreference = "Stop"

# The six subclasses whose signature mechanic used to have no HUD (Infinity Mage,
# Arcane Bard, Beast Tamer, Advanced Summoner, Tech Summoner, Yoyo Master) now show
# a resource bar, like the other subclasses already do. One consolidated ModSystem
# surfaces all six. This pins that bar to its source.

$repoRoot = Split-Path -Parent $PSScriptRoot
$ui = Join-Path $repoRoot "Content\UI\SubclassResourceUI.cs"

if (-not (Test-Path $ui)) {
    throw "Missing Content\UI\SubclassResourceUI.cs (HUD bars for the 6 mechanics)."
}

$src = Get-Content -Raw $ui

# It draws a bar as an interface layer (same pattern as the other resource UIs).
if ($src -notmatch "ModifyInterfaceLayers" -or
    $src -notmatch "DrawFloatingResourceBar" -or
    $src -notmatch "ShouldDrawPlayerUI") {
    throw "SubclassResourceUI should draw a resource bar via ModifyInterfaceLayers/DrawFloatingResourceBar."
}

# It must surface each of the six mechanics (active check + resource field).
foreach ($token in @(
    "IsActiveInfinityMage", "Overflow",
    "IsActiveArcaneBard", "Crescendo",
    "IsActiveBeastTamer", "Ferocity",
    "IsActiveAdvancedSummoner", "Command",
    "IsActiveTechSummoner", "PowerCore",
    "IsActiveYoyoMaster", "precisionStacks")) {
    if ($src -notmatch [regex]::Escape($token)) {
        throw "SubclassResourceUI should surface '$token'."
    }
}

Write-Host "Subclass resource UI source smoke test passed."
