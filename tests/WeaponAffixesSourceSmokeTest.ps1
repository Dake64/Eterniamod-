$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

$table = Read-File "Content\Affixes\AffixTable.cs"
$global = Read-File "Content\Globals\EterniaAffixGlobalItem.cs"

# --- Rarity decides HOW MANY sub-stats, not how big --------------------------
# That is what makes a Legendary categorically better instead of just a lucky Common.

if ($table -notmatch "AffixCount") {
    throw "The tier should decide how many sub-stats a weapon rolls."
}

foreach ($tier in @("Common", "Uncommon", "Rare", "SuperRare", "Legendary", "Mythic", "Ancient", "Nightmare")) {
    if ($table -notmatch $tier) {
        throw "The affix ladder should mirror the enemy rarity scale (missing $tier)."
    }
}

# A Common weapon must stay a plain vanilla weapon -- no sub-stats, no tooltip clutter.
if ($table -notmatch "_ => 0") {
    throw "Common weapons should roll zero sub-stats."
}

# Distinct affixes only -- a weapon must never roll the same sub-stat twice.
if ($table -notmatch "pool\.RemoveAt") {
    throw "RollAffixes should pick distinct sub-stats."
}

# --- Only real weapons roll --------------------------------------------------

if ($global -notmatch "AppliesToEntity") {
    throw "The affix global should only attach to weapons, not every item in the world."
}

foreach ($guard in @("item\.pick == 0", "item\.accessory", "maxStack == 1")) {
    if ($global -notmatch $guard) {
        throw "Tools/accessories/stackables should not roll weapon sub-stats (missing $guard)."
    }
}

# --- It rolls once, and the roll sticks to the item --------------------------

if ($global -notmatch "OnCreated" -or $global -notmatch "OnSpawn") {
    throw "Weapons should roll when crafted/bought (OnCreated) and when they drop (OnSpawn)."
}

if ($global -notmatch "if \(rolled\)") {
    throw "A weapon must only ever roll once."
}

# The affix list is a reference type: without Clone the roll is shared or lost.
if ($global -notmatch "public override GlobalItem Clone") {
    throw "The rolled affixes must be cloned with the item, or copies share/lose their roll."
}

if ($global -notmatch "SaveData" -or $global -notmatch "LoadData") {
    throw "A weapon's roll must persist with the item."
}

if ($global -notmatch "NetSend" -or $global -notmatch "NetReceive") {
    throw "The roll must be networked, or multiplayer clients see a different weapon."
}

# --- The sub-stats actually do something -------------------------------------

foreach ($hook in @(
    "ModifyWeaponDamage",
    "ModifyWeaponCrit",
    "ModifyWeaponKnockback",
    "UseSpeedMultiplier",
    "HoldItem")) {

    if ($global -notmatch $hook) {
        throw "The sub-stats should be applied through $hook."
    }
}

if ($global -notmatch "ModifyTooltips") {
    throw "A weapon should show its rarity and sub-stats on the tooltip."
}

Write-Host "Weapon affixes source smoke test passed."
