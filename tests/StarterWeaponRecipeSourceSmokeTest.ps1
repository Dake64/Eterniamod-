$ErrorActionPreference = "Stop"

# Class starter weapons are no longer auto-given, so each must be craftable from
# the very start (Wood at a Work Bench). Otherwise a fresh Mage/Ranger/Summoner has
# no class weapon and the vanilla Copper Shortsword (melee) would kill them.

$repoRoot = Split-Path -Parent $PSScriptRoot

$weapons = @{
    "Content\Items\Weapons\Warrior\TrainingBlade.cs"   = "TrainingBlade"
    "Content\Items\Weapons\Magic\ApprenticeWand.cs"    = "ApprenticeWand"
    "Content\Items\Weapons\Ranger\TrainingBow.cs"      = "TrainingBow"
    "Content\Items\Weapons\Summoner\TrainingWhip.cs"   = "TrainingWhip"
}

foreach ($entry in $weapons.GetEnumerator()) {
    $content = Get-Content -Raw (Join-Path $repoRoot $entry.Key)

    if ($content -notmatch "public override void AddRecipes\(") {
        throw "$($entry.Value) should be craftable (AddRecipes) now that it is not auto-given."
    }

    if ($content -notmatch "ItemID\.Wood") {
        throw "$($entry.Value) recipe should use Wood so it is craftable from the start."
    }

    if ($content -notmatch "TileID\.WorkBenches") {
        throw "$($entry.Value) recipe should use the early-game Work Bench."
    }
}

Write-Host "Starter weapon recipe source smoke test passed."
