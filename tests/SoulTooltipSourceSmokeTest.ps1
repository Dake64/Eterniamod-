$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$classSoulPath = Join-Path $repoRoot "Content\Items\Souls\ClassSoulItem.cs"
$emptySoulPath = Join-Path $repoRoot "Content\Items\Souls\EmptySoul.cs"

$classSoul = Get-Content -Raw $classSoulPath
$emptySoul = Get-Content -Raw $emptySoulPath

if ($classSoul -notmatch "ModifyTooltips" -or
    $classSoul -notmatch "Equip in an accessory slot" -or
    $classSoul -notmatch "kills the player" -or
    $classSoul -notmatch "severe no-Soul penalty") {
    throw "ClassSoulItem should explain activation, wrong-weapon death, and unequipped Soul penalty."
}

if ($classSoul -notmatch "SoulRules\.GetDisplayName\(Soul\)") {
    throw "ClassSoulItem tooltip should use SoulRules.GetDisplayName(Soul)."
}

if ($emptySoul -notmatch "ModifyTooltips" -or
    $emptySoul -notmatch "Craft in your inventory" -or
    $emptySoul -notmatch "Warrior, Mage, Ranger or Summoner" -or
    $emptySoul -notmatch "does not activate class EXP") {
    throw "EmptySoul should explain the class choice crafting flow and inactive EXP state."
}

Write-Host "Soul tooltip source smoke test passed."
