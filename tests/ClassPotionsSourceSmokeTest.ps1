$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

# --- The shared plumbing -----------------------------------------------------

$baseItem = Read-File "Content\Items\Consumables\EterniaConsumable.cs"

if ($baseItem -notmatch "Item\.buffType\s*=\s*BuffType" -or
    $baseItem -notmatch "Item\.buffTime\s*=\s*BuffDuration") {
    throw "EterniaConsumable base should apply its buff via Item.buffType / Item.buffTime."
}

if ($baseItem -match "Item\.potion\s*=\s*true") {
    throw "Class potions are combat buffs, not healing -- they must not trigger Potion Sickness."
}

$baseBuff = Read-File "Content\Buffs\EterniaPotionBuff.cs"

if ($baseBuff -notmatch "abstract void Apply\(Player player\)") {
    throw "EterniaPotionBuff should define the Apply(player) hook every potion buff fills in."
}

# --- Every potion has an item, a buff, and a recipe --------------------------

$potions = @{
    "WarriorsBrew"     = "DamageClass\.Melee"
    "ArcanistsBrew"    = "DamageClass\.Magic"
    "HuntersBrew"      = "DamageClass\.Ranged"
    "PackleadersBrew"  = "DamageClass\.Summon"
    "BattleTonic"      = "DamageClass\.Melee"
    "GrandBattleTonic" = "GetCritChance"
    "EternalFeast"     = "lifeRegen"
    "WardingTonic"     = "statDefense"
}

foreach ($name in $potions.Keys) {
    $item = Read-File "Content\Items\Consumables\$name.cs"

    if ($item -notmatch "BuffType<${name}Buff>") {
        throw "$name should grant its ${name}Buff."
    }

    if ($item -notmatch "CreateRecipe") {
        throw "$name should be craftable."
    }

    $buff = Read-File "Content\Buffs\${name}Buff.cs"

    if ($buff -notmatch "EterniaPotionBuff") {
        throw "${name}Buff should extend EterniaPotionBuff."
    }

    if ($buff -notmatch $potions[$name]) {
        throw "${name}Buff should actually apply its effect ($($potions[$name]))."
    }
}

# The four base-class brews must cover all four souls, no overlap.
$coverage = @{
    "WarriorsBrew"    = "DamageClass\.Melee"
    "ArcanistsBrew"   = "DamageClass\.Magic"
    "HuntersBrew"     = "DamageClass\.Ranged"
    "PackleadersBrew" = "DamageClass\.Summon"
}

foreach ($name in $coverage.Keys) {
    $buff = Read-File "Content\Buffs\${name}Buff.cs"

    if ($buff -notmatch "GetDamage\($($coverage[$name])\)") {
        throw "${name}Buff should boost its class's damage ($($coverage[$name]))."
    }
}

# --- Localization ------------------------------------------------------------

$loc = Read-File "en-US.hjson"

foreach ($name in $potions.Keys) {
    if ($loc -notmatch "(?ms)\b${name}:\s*\{[^}]*DisplayName:") {
        throw "en-US.hjson should localize $name DisplayName."
    }

    if ($loc -notmatch "(?ms)\b${name}:\s*\{[^}]*Tooltip:") {
        throw "en-US.hjson should give $name a Tooltip."
    }
}

Write-Host "Class potions source smoke test passed."
