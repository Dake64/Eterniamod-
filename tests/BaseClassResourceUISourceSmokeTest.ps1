$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiPath = Join-Path $repoRoot "Content\UI\BaseClassResourceUI.cs"

if (!(Test-Path $uiPath)) {
    throw "Missing required source file: $uiPath"
}

$ui = Get-Content -Raw $uiPath

if ($ui -notmatch "Eternia: Base Class Resource UI") {
    throw "BaseClassResourceUI should register a named interface layer."
}

if ($ui -notmatch "InterfaceScaleType\.UI") {
    throw "BaseClassResourceUI should draw in UI scale."
}

if ($ui -notmatch "GetModPlayer<SubclassPlayer>\(\)" -or
    $ui -notmatch "GetModPlayer<BaseClassPlayer>\(\)") {
    throw "BaseClassResourceUI should read both SubclassPlayer and BaseClassPlayer."
}

$requiredPairs = @{
    "Warrior" = "WarriorMomentum"
    "Mage" = "MageCharge"
    "Ranger" = "RangerFocus"
    "Summoner" = "SummonerBond"
}

foreach ($pair in $requiredPairs.GetEnumerator()) {
    if ($ui -notmatch [regex]::Escape($pair.Key)) {
        throw "BaseClassResourceUI should handle subclass '$($pair.Key)'."
    }

    if ($ui -notmatch [regex]::Escape($pair.Value)) {
        throw "BaseClassResourceUI should display resource '$($pair.Value)'."
    }
}

foreach ($label in @("MOMENTUM", "CHARGE", "FOCUS", "BOND")) {
    if ($ui -notmatch [regex]::Escape($label)) {
        throw "BaseClassResourceUI should draw label '$label'."
    }
}

Write-Host "Base class resource UI source smoke test passed."
