$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$curseRoot = Join-Path $repoRoot "Content\Items\Accessories\Curses"

$weak = Get-Content -Raw (Join-Path $curseRoot "WeakCurse.cs")
$blood = Get-Content -Raw (Join-Path $curseRoot "BloodCurse.cs")

if ($weak -notmatch "class WeakCurse : ModItem") {
    throw "WeakCurse should remain a real ModItem accessory."
}

if ($blood -notmatch "class BloodCurse : ModItem") {
    throw "BloodCurse should be a real ModItem accessory, not an empty placeholder class."
}

foreach ($pair in @(
    @{Name="WeakCurse"; Content=$weak; Corruption="10"},
    @{Name="BloodCurse"; Content=$blood; Corruption="35"})) {
    if ($pair.Content -notmatch "Item\.accessory\s*=\s*true") {
        throw "$($pair.Name) should be an accessory."
    }

    if ($pair.Content -notmatch "UpdateAccessory") {
        throw "$($pair.Name) should apply its curse in UpdateAccessory."
    }

    if ($pair.Content -notmatch "BaseCorruption\s*\+=\s*$($pair.Corruption)") {
        throw "$($pair.Name) should add $($pair.Corruption) base corruption."
    }

    if ($pair.Content -notmatch "AddRecipes\(") {
        throw "$($pair.Name) should be craftable."
    }
}

if ($blood -notmatch "statLifeMax2\s*-=\s*20") {
    throw "BloodCurse should have a clear life tradeoff for its stronger corruption."
}

if ($blood -notmatch "ItemID\.Vertebrae" -or
    $blood -notmatch "ItemID\.FallenStar") {
    throw "BloodCurse recipe should use crimson/cursed materials and Fallen Stars."
}

Write-Host "Curse accessories source smoke test passed."
