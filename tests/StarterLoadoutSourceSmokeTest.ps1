$ErrorActionPreference = "Stop"

# Design change (2026-07-06): activating a Class Soul no longer hands the player a
# starter weapon. This test now guards that the starter-loadout system stays
# removed (the Soul only defines the class).

$repoRoot = Split-Path -Parent $PSScriptRoot
$player = Get-Content -Raw (Join-Path $repoRoot "Content\Players\EterniaPlayer.cs")

if ($player -match "GiveStarterWeapon") {
    throw "Activating a Class Soul should not auto-give a starter weapon."
}

if ($player -match "StarterGiven") {
    throw "The starter-weapon tracking flags should be removed."
}

Write-Host "Starter loadout (removed) source smoke test passed."
