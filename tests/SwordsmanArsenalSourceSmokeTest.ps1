$ErrorActionPreference = "Stop"

# The Warrior/Swordsman path needs a pre-hardmode sword line so there is variety
# to choose from before promotion. Each is a craftable signature bleed sword
# (IBleedWeapon), a real melee swing weapon, and NOT subclass-locked (usable
# before hardmode by any Warrior building toward the Swordsman).

$repoRoot = Split-Path -Parent $PSScriptRoot
$w = Join-Path $repoRoot "Content\Items\Weapons\Warrior"

$swords = @(
    "SerratedIronBlade",
    "SilverlightRapier",
    "HuntersWarblade",
    "DreadReaver",
    "CorruptorsRipper",
    "Thornrender",
    "BonewardenSabre",
    "MoltenGutripper"
)

# These two are obtained by DROP / chest loot instead of crafting (they are tied to
# a boss / the Dungeon in the obtention pass -- see SwordsmanObtention test).
$dropOnly = @("DreadReaver", "BonewardenSabre")

foreach ($s in $swords) {
    $path = Join-Path $w "$s.cs"

    if (!(Test-Path $path)) {
        throw "Missing pre-hardmode Warrior sword '$s'."
    }

    $c = Get-Content -Raw $path

    if ($c -notmatch ":\s*ModItem,\s*IBleedWeapon") {
        throw "$s should be a bleed sword (ModItem, IBleedWeapon)."
    }

    if ($c -notmatch "public int BleedChance") {
        throw "$s should define a signature BleedChance."
    }

    if ($c -notmatch "DamageClass\.Melee" -or
        $c -notmatch "ItemUseStyleID\.Swing") {
        throw "$s should be a melee swing sword."
    }

    if ($dropOnly -contains $s) {
        if ($c -match "CreateRecipe\(\)") {
            throw "$s is a drop/chest reward and must NOT be craftable (that was the soft gate we removed)."
        }
    }
    elseif ($c -notmatch "CreateRecipe\(\)") {
        throw "$s should be craftable in pre-hardmode (CreateRecipe)."
    }

    if ($c -match "SubclassLockedItem") {
        throw "$s should NOT be subclass-locked; it is a pre-hardmode Warrior sword."
    }
}

Write-Host "Swordsman arsenal source smoke test passed."
