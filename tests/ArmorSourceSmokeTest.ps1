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

    # Every piece needs a real equip slot or the mod will not even load. What changed: the
    # HELMETS now have Eternia's own art, so they take their own equip slot through
    # EquipLoader instead of borrowing a vanilla one. Body and legs still have no art, so
    # they keep borrowing a vanilla look.
    #
    # (ArmorEquipTextureSourceSmokeTest is what guarantees the helms' declared textures
    # actually exist -- a declared path with no file is what would break loading.)
    if ($src -notmatch "Item\.headSlot\s*=\s*(ArmorIDs\.|EquipLoader\.GetEquipSlot)") {
        throw "$setName must set headSlot, either to its own equip slot or a vanilla ArmorIDs one."
    }

    foreach ($slot in @("bodySlot", "legSlot")) {
        if ($src -notmatch "Item\.$slot\s*=\s*(ArmorIDs\.|EquipLoader\.GetEquipSlot)") {
            throw "$setName must set $slot to an equip slot."
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
