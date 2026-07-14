$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$armorDir = Join-Path $repoRoot "Content\Items\Armor"

function Read-File($path) {
    if (-not (Test-Path $path)) { throw "Missing expected file: $path" }
    return Get-Content -Raw $path
}

# --- Base --------------------------------------------------------------------

$base = Read-File (Join-Path $armorDir "EterniaArmor.cs")

if ($base -notmatch "abstract class EterniaArmor\s*:\s*ModItem" -or
    $base -notmatch "Item\.defense") {
    throw "EterniaArmor should be the armour base."
}

foreach ($classBase in @("WarriorArmor", "MageArmor", "RangerArmor", "SummonerArmor")) {
    if ($base -notmatch "abstract class $classBase\s*:\s*EterniaArmor") {
        throw "$classBase should extend EterniaArmor."
    }
}

# --- Every set: 3 pieces, real equip slots, a set bonus ----------------------
# Each set lives in ONE file so the bonus sits next to the pieces it belongs to.

$sets = @{
    # set file            = @(head, body, legs)
    "SteelboundSet"     = @("SteelboundHelm", "SteelboundChest", "SteelboundGreaves")
    "EmberweaveSet"     = @("EmberweaveHood", "EmberweaveRobe", "EmberweaveLeggings")
    "HuntersGarbSet"    = @("HuntersGarbHood", "HuntersGarbChest", "HuntersGarbLeggings")
    "PackmasterSet"     = @("PackmasterHood", "PackmasterChest", "PackmasterLeggings")
    "IronchainSet"      = @("IronchainHelm", "IronchainChest", "IronchainGreaves")
    "HemocarnageSet"    = @("HemocarnageHelm", "HemocarnageChest", "HemocarnageGreaves")
    "AegisBulwarkSet"   = @("AegisBulwarkHelm", "AegisBulwarkChest", "AegisBulwarkGreaves")
    "PrismaticSet"      = @("PrismaticHood", "PrismaticRobe", "PrismaticLeggings")
    "BlightweaveSet"    = @("BlightweaveHood", "BlightweaveRobe", "BlightweaveLeggings")
    "LichRegaliaSet"    = @("LichRegaliaHood", "LichRegaliaRobe", "LichRegaliaLeggings")
    "ReactorSuitSet"    = @("ReactorSuitHelm", "ReactorSuitChest", "ReactorSuitGreaves")
    "HawkeyeGarbSet"    = @("HawkeyeGarbHood", "HawkeyeGarbChest", "HawkeyeGarbLeggings")
    "GunslingerRigSet"  = @("GunslingerRigHelm", "GunslingerRigChest", "GunslingerRigGreaves")
    "AlphahideSet"      = @("AlphahideHood", "AlphahideChest", "AlphahideGreaves")
    "ExoframeSet"       = @("ExoframeHelm", "ExoframeChest", "ExoframeGreaves")
    "LegionRegaliaSet"  = @("LegionRegaliaHood", "LegionRegaliaChest", "LegionRegaliaLeggings")
}

foreach ($setName in $sets.Keys) {
    $src = Read-File (Join-Path $armorDir "$setName.cs")
    $pieces = $sets[$setName]

    foreach ($piece in $pieces) {
        if ($src -notmatch "class $piece\s*:\s*(Warrior|Mage|Ranger|Summoner)Armor") {
            throw "$setName should contain the piece '$piece'."
        }
    }

    # Real equip slots. Eternia has no armour art, so every piece BORROWS a vanilla
    # armour's look -- without an equip slot the mod would not even load.
    foreach ($slot in @("headSlot", "bodySlot", "legSlot")) {
        if ($src -notmatch "Item\.$slot\s*=\s*ArmorIDs\.") {
            throw "$setName must set $slot to a vanilla ArmorIDs slot (Eternia has no armour art)."
        }
    }

    # The head piece carries the set bonus.
    if ($src -notmatch "IsArmorSet" -or $src -notmatch "UpdateArmorSet" -or
        $src -notmatch "player\.setBonus\s*=") {
        throw "$setName needs a set bonus (IsArmorSet + UpdateArmorSet + setBonus text)."
    }

    if ($src -notmatch "AddRecipes") {
        throw "$setName should be craftable."
    }
}

# --- The point of the SET BONUS: it bends the subclass's signature mechanic ---
# (Armour reuses the very same Acc* hooks the accessories drive, so the two compose.)

$setMechanics = @{
    "SteelboundSet"    = "AccBonusMaxCombo"        # Warrior  -- Combo
    "HuntersGarbSet"   = "AccFocusRegenMult"       # Ranger   -- Concentration (works pre-promotion)
    "IronchainSet"     = "AccBonusComboDuration"   # Fighter  -- Combo
    "HemocarnageSet"   = "AccTrailGainMult"        # Swordsman -- Crimson Trail
    "AegisBulwarkSet"  = "AccAuraDamage"           # Escudero -- the aura
    "PrismaticSet"     = "AccSwitchCooldownCut"    # Elementalist -- affinity
    "BlightweaveSet"   = "BaseCorruption"          # Cursed Mage -- Corruption
    "LichRegaliaSet"   = "AccReserveMult"          # Necromancer -- Reserved Life
    "ReactorSuitSet"   = "AccHeatPerShotMult"      # Energy Gunner -- Temperature
    "HawkeyeGarbSet"   = "AccPerfectDamage"        # Archer   -- Concentration
    "GunslingerRigSet" = "AccMomentumGainMult"     # Gunner   -- Momentum
    "AlphahideSet"     = "AccFerocityGainMult"     # Beast Tamer -- Ferocity
    "ExoframeSet"      = "AccCoreRateMult"         # Tech Summoner -- Power Core
    "LegionRegaliaSet" = "AccLegionScaleBonus"     # Advanced Summoner -- Legion
}

foreach ($setName in $setMechanics.Keys) {
    $src = Read-File (Join-Path $armorDir "$setName.cs")

    if ($src -notmatch [regex]::Escape($setMechanics[$setName])) {
        throw "$setName's set bonus should bend its mechanic via '$($setMechanics[$setName])'."
    }
}

Write-Host "Armor source smoke test passed."
