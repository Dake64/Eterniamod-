$ErrorActionPreference = "Stop"

# The Elemental Mage arsenal: 7 pre-hardmode + 8 hardmode staves, each an ElementalStaff
# that fires the shared ElementalArsenalBolt tagged with its element. Damage rises tier
# to tier; the affinity/cycle staves use Element = -1. Some staves are drop/chest
# rewards (obtention 2nd pass) instead of craftable.

$repoRoot = Split-Path -Parent $PSScriptRoot
$m = Join-Path $repoRoot "Content\Items\Weapons\Magic"

# In progression order. Element: 0 Fire, 1 Ice, 2 Lightning, 3 Wind, 4 Earth, -1 cycle.
$line = @(
    @{ Name = "EmberSpark";             Dmg = 18;  Element = 0;  DropOnly = $false },
    @{ Name = "BorealStaff";            Dmg = 25;  Element = 1;  DropOnly = $true  },  # ice chests
    @{ Name = "SparkRod";               Dmg = 29;  Element = 2;  DropOnly = $true  },  # King Slime
    @{ Name = "GaleScepter";            Dmg = 34;  Element = 3;  DropOnly = $false },
    @{ Name = "GraniteCore";            Dmg = 40;  Element = 4;  DropOnly = $true  },  # granite
    @{ Name = "MagmaTome";              Dmg = 47;  Element = 0;  DropOnly = $false },
    @{ Name = "ElementalSageScepter";   Dmg = 56;  Element = -1; DropOnly = $true  },  # Skeletron
    @{ Name = "PhoenixStaff";           Dmg = 72;  Element = 0;  DropOnly = $false },
    @{ Name = "GlacialScepter";         Dmg = 80;  Element = 1;  DropOnly = $false },
    @{ Name = "StormOrb";               Dmg = 88;  Element = 2;  DropOnly = $false },
    @{ Name = "HurricaneStaff";         Dmg = 96;  Element = 3;  DropOnly = $true  },  # Pirates
    @{ Name = "SeismicScepter";         Dmg = 104; Element = 4;  DropOnly = $true  },  # Golem
    @{ Name = "GrimoireOfFiveElements"; Dmg = 118; Element = -1; DropOnly = $false },
    @{ Name = "CataclysmStaff";         Dmg = 135; Element = -1; DropOnly = $true  },  # Nebula Pillar
    @{ Name = "HeartOfGaia";            Dmg = 155; Element = -1; DropOnly = $false }
)

$previous = 0

foreach ($entry in $line) {
    $path = Join-Path $m "$($entry.Name).cs"
    if (!(Test-Path $path)) {
        throw "Missing elemental staff '$($entry.Name)'."
    }

    $c = Get-Content -Raw $path

    if ($c -notmatch ":\s*ElementalStaff") {
        throw "$($entry.Name) should extend ElementalStaff."
    }
    if ($c -notmatch ("public override int Element => " + [regex]::Escape("$($entry.Element)") + ";")) {
        throw "$($entry.Name) should be element $($entry.Element)."
    }

    # Craftable, unless it is a drop/chest reward.
    if ($entry.DropOnly) {
        if ($c -match "CreateRecipe\(\)") {
            throw "$($entry.Name) is a drop/chest reward and must NOT be craftable."
        }
    }
    elseif ($c -notmatch "CreateRecipe\(\)") {
        throw "$($entry.Name) should be craftable."
    }

    $dmg = [int][regex]::Match($c, "SetElementalDefaults\((\d+)").Groups[1].Value
    if ($dmg -ne $entry.Dmg) {
        throw "$($entry.Name) should do $($entry.Dmg) damage but does $dmg."
    }
    if ($dmg -le $previous) {
        throw "$($entry.Name) ($dmg) should out-damage the previous tier ($previous)."
    }
    $previous = $dmg
}

# --- Shared mechanic: one bolt parameterized by element ----------------------
$staff = Get-Content -Raw (Join-Path $m "ElementalStaff.cs")
if ($staff -notmatch "ElementalArsenalBolt" -or $staff -notmatch "ResolveElement") {
    throw "ElementalStaff should fire the ElementalArsenalBolt with the resolved element."
}
if ($staff -notmatch "IsActiveElementalist\(\)" -or $staff -notmatch "CurrentElement") {
    throw "Affinity/cycle staves should fire the active affinity when promoted."
}

$bolt = Get-Content -Raw (Join-Path $repoRoot "Content\Projectiles\ElementalArsenalBolt.cs")
if ($bolt -notmatch "DamageClass\.Magic" -or $bolt -notmatch "ai\[0\]") {
    throw "ElementalArsenalBolt should be a Magic projectile keyed by element (ai[0])."
}
# Base element effect applies to any caster (works pre-hardmode).
if ($bolt -notmatch "BuffID\.OnFire" -or $bolt -notmatch "BuffID\.Frostburn" -or
    $bolt -notmatch "BuffID\.Electrified") {
    throw "ElementalArsenalBolt should apply the base element effect regardless of subclass."
}

# --- Obtention: drops + ice chest --------------------------------------------
$drops = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\ElementalDropsGlobalNPC.cs")
$chest = Get-Content -Raw (Join-Path $repoRoot "Content\Systems\ElementalChestLoot.cs")

$expectedDrops = @{
    "NPCID.KingSlime"        = "SparkRod"
    "NPCID.SkeletronHead"    = "ElementalSageScepter"
    "NPCID.PirateCaptain"    = "HurricaneStaff"
    "NPCID.Golem"            = "SeismicScepter"
    "NPCID.LunarTowerNebula" = "CataclysmStaff"
    "NPCID.GraniteGolem"     = "GraniteCore"
}
foreach ($pair in $expectedDrops.GetEnumerator()) {
    if ($drops -notmatch [regex]::Escape($pair.Key) -or $drops -notmatch [regex]::Escape($pair.Value)) {
        throw "ElementalDropsGlobalNPC should drop $($pair.Value) from $($pair.Key)."
    }
}

if ($chest -notmatch "PostWorldGen" -or $chest -notmatch "BorealStaff" -or
    $chest -notmatch "IsIceBiomeChest") {
    throw "ElementalChestLoot should seed BorealStaff into Ice-biome chests."
}

Write-Host "Elemental arsenal source smoke test passed."
