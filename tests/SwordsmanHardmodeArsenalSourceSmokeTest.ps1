$ErrorActionPreference = "Stop"

# The Swordsman sword line continues through hardmode, one craftable bleed sword per
# major tier (Mythril -> Endgame). Each is IBleedWeapon, a melee swing weapon, and
# craftable at a hardmode station. Damage rises monotonically down the tier order
# and every sword stays at or below its vanilla same-tier reference.

$repoRoot = Split-Path -Parent $PSScriptRoot
$w = Join-Path $repoRoot "Content\Items\Weapons\Warrior"

# Ordered by progression tier, with the vanilla same-tier damage cap.
$line = @(
    @{ Name = "QuicksilverFang";     Cap = 45  },   # Mythril / Orichalcum
    @{ Name = "SanguineCleaver";     Cap = 66  },   # Adamantite / Titanium
    @{ Name = "HallowedBloodletter"; Cap = 66  },   # Mechanical bosses (Excalibur = 66)
    @{ Name = "ChlorophyteHemoblade";Cap = 70  },   # Chlorophyte (Claymore = 70)
    @{ Name = "TitansGutcleaver";    Cap = 100 },   # Golem
    @{ Name = "CrimsonRequiem";      Cap = 95  },   # Solar Eclipse (Terra Blade = 95)
    @{ Name = "Exsanguinator";       Cap = 190 }    # Endgame (Terrarian = 190)
)

$previousDamage = 42   # Bloodletter Blade (cobalt-tier promotion reward) precedes the line.

foreach ($entry in $line) {
    $path = Join-Path $w "$($entry.Name).cs"

    if (!(Test-Path $path)) {
        throw "Missing hardmode Swordsman sword '$($entry.Name)'."
    }

    $c = Get-Content -Raw $path

    if ($c -notmatch ":\s*ModItem,\s*IBleedWeapon") {
        throw "$($entry.Name) should be a bleed sword (ModItem, IBleedWeapon)."
    }

    if ($c -notmatch "public int BleedChance") {
        throw "$($entry.Name) should define a signature BleedChance."
    }

    if ($c -notmatch "DamageClass\.Melee" -or
        $c -notmatch "ItemUseStyleID\.Swing") {
        throw "$($entry.Name) should be a melee swing sword."
    }

    if ($c -notmatch "CreateRecipe\(\)") {
        throw "$($entry.Name) should be craftable."
    }

    if ($c -notmatch "TileID\.(MythrilAnvil|LunarCraftingStation)") {
        throw "$($entry.Name) should craft at a hardmode station."
    }

    $damage = [int][regex]::Match($c, "Item\.damage\s*=\s*(\d+);").Groups[1].Value

    if ($damage -le $previousDamage) {
        throw "$($entry.Name) does $damage damage, not more than the previous tier ($previousDamage). The line must climb."
    }

    if ($damage -gt $entry.Cap) {
        throw "$($entry.Name) does $damage damage, above its vanilla same-tier cap ($($entry.Cap)). Must not outclass vanilla."
    }

    $previousDamage = $damage
}

# The recipe groups that let the line craft from either ore variant per tier.
$groups = Get-Content -Raw (Join-Path $repoRoot "Content\Systems\EterniaRecipeGroups.cs")
foreach ($g in @("EterniaSilver", "EterniaGold", "EterniaEvilBar", "EterniaMythril", "EterniaAdamantite")) {
    if ($groups -notmatch [regex]::Escape($g)) {
        throw "EterniaRecipeGroups should register the '$g' recipe group."
    }
}

Write-Host "Swordsman hardmode arsenal source smoke test passed."
