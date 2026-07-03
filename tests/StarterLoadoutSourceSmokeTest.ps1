$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$playerPath = Join-Path $repoRoot "Content\Players\EterniaPlayer.cs"

$player = Get-Content -Raw $playerPath

if ($player -notmatch "ModContent\.ItemType<TrainingBlade>\(\)") {
    throw "Warrior starter loadout should include TrainingBlade."
}

if ($player -notmatch "ModContent\.ItemType<ApprenticeWand>\(\)") {
    throw "Mage starter loadout should include ApprenticeWand."
}

if ($player -notmatch "ModContent\.ItemType<TrainingBow>\(\)") {
    throw "Ranger starter loadout should include TrainingBow."
}

if ($player -notmatch "GiveStarterStack\(ItemID\.WoodenArrow,\s*250\)") {
    throw "Ranger starter loadout should include WoodenArrow ammo."
}

if ($player -notmatch "ModContent\.ItemType<TrainingWhip>\(\)") {
    throw "Summoner starter loadout should include TrainingWhip."
}

if ($player -notmatch "private void GiveStarterStack\(int itemType,\s*int stack\)") {
    throw "EterniaPlayer should expose a helper for starter stacks."
}

Write-Host "Starter loadout source smoke test passed."
