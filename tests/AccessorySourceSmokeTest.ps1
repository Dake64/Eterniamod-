$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$accDir = Join-Path $repoRoot "Content\Items\Accessories"
$playerDir = Join-Path $repoRoot "Content\Players"

function Read-File($path) {
    if (-not (Test-Path $path)) { throw "Missing expected file: $path" }
    return Get-Content -Raw $path
}

# --- Base --------------------------------------------------------------------

$base = Read-File (Join-Path $accDir "EterniaAccessory.cs")

if ($base -notmatch "abstract class EterniaAccessory\s*:\s*ModItem" -or
    $base -notmatch "Item\.accessory\s*=\s*true") {
    throw "EterniaAccessory should be the accessory base."
}

foreach ($classBase in @("WarriorAccessory", "MageAccessory", "RangerAccessory", "SummonerAccessory")) {
    if ($base -notmatch "abstract class $classBase\s*:\s*EterniaAccessory") {
        throw "$classBase should extend EterniaAccessory."
    }
}

# --- Every accessory is a real accessory and is CRAFTED ----------------------

$accessories = Get-ChildItem -Path $accDir -Filter *.cs |
    Where-Object { $_.Name -ne "EterniaAccessory.cs" }

if ($accessories.Count -lt 40) {
    throw "Expected a broad accessory roster (40+); found $($accessories.Count)."
}

foreach ($acc in $accessories) {
    $src = Get-Content -Raw $acc.FullName
    $name = [System.IO.Path]::GetFileNameWithoutExtension($acc.Name)

    if ($src -notmatch "class $name\s*:\s*(Warrior|Mage|Ranger|Summoner)Accessory") {
        throw "$name should extend one of the class accessory bases."
    }

    if ($src -notmatch "UpdateAccessory") {
        throw "$name should do something when equipped (UpdateAccessory)."
    }

    if ($src -notmatch "AddRecipes") {
        throw "$name should be craftable."
    }
}

# --- The point of these: they bend each subclass's SIGNATURE MECHANIC ---------
# Each pairing below is (accessory, the mechanic hook it must drive).

$mechanicHooks = @{
    # Ranger
    "CoolantRig"         = "AccHeatPerShotMult"
    "HeatSinkArray"      = "AccCoolRateMult"
    "RefractoryPlating"  = "AccOverheatShield"
    "HuntersLens"        = "AccFocusRegenMult"
    "FalconEye"          = "AccPerfectDamage"
    "SteadyNerve"        = "AccFocusLossMult"
    "RecoilDamper"       = "AccMomentumDecayMult"
    "HotStreakRig"       = "AccMomentumGainMult"
    "DeadEyeRegulator"   = "AccDeadEyeBonusTicks"
    # Summoner
    "AlphasFang"         = "AccFerocityGainMult"
    "BeastlordsCollar"   = "AccFerocityDecayMult"
    "PrimalTotem"        = "AccFrenzyDamage"
    "CapacitorBank"      = "AccCoreRateMult"
    "FusionCapacitor"    = "AccOverdriveBonusTicks"
    "ReactivePlating"    = "AccOverdriveDefense"
    "CommandersSigil"    = "AccCommandRateMult"
    "LegionStandard"     = "AccLegionScaleBonus"
    "OverclockGovernor"  = "AccOverclockBonusTicks"
    # Warrior
    "BrawlersWraps"      = "AccBonusMaxCombo"
    "ChainBreaker"       = "AccBonusMaxCombo"
    "EndlessRhythm"      = "AccBonusComboDuration"
    "BloodletterBand"    = "AccTrailGainMult"
    "HemophageSigil"     = "AccTrailGainMult"
    "CrimsonChalice"     = "AccTrailGainMult"
    "BulwarkCharm"       = "AccAuraRadius"
    "AegisCore"          = "AccAuraDamage"
    "ResonatingWard"     = "AccAuraPulseMult"
    # Mage
    "AttunementCharm"    = "AccSwitchCooldownCut"
    "ElementalPrism"     = "AccSwitchCooldownCut"
    "PrimalConvergence"  = "AccSurgeBonusTicks"
    "CursedTalisman"     = "BaseCorruption"
    "PactSeal"           = "BaseCorruption"
    "BlightedHeart"      = "BaseCorruption"
    "BonePhylactery"     = "AccReserveMult"
    "SoulConduit"        = "AccManaDrainMult"
    "LichsSigil"         = "AccManaDrainMult"
}

foreach ($acc in $mechanicHooks.Keys) {
    $src = Read-File (Join-Path $accDir "$acc.cs")

    if ($src -notmatch [regex]::Escape($mechanicHooks[$acc])) {
        throw "$acc should bend its subclass mechanic via '$($mechanicHooks[$acc])'."
    }
}

# --- The hooks must exist on the players, and RESET every frame ---------------
# (If they were not reset, an accessory's effect would stack up forever.)

$playerHooks = @{
    "EnergyShooterPlayer"    = @("AccHeatPerShotMult", "AccCoolRateMult", "AccOverheatShield")
    "ArcherPlayer"           = @("AccFocusRegenMult", "AccPerfectDamage", "AccFocusLossMult")
    "GunnerPlayer"           = @("AccMomentumGainMult", "AccMomentumDecayMult", "AccDeadEyeBonusTicks")
    "BeastTamerPlayer"       = @("AccFerocityGainMult", "AccFerocityDecayMult", "AccFrenzyDamage")
    "TechSummonerPlayer"     = @("AccCoreRateMult", "AccOverdriveDefense", "AccOverdriveBonusTicks")
    "AdvancedSummonerPlayer" = @("AccCommandRateMult", "AccLegionScaleBonus", "AccOverclockBonusTicks")
    "FighterPlayer"          = @("AccBonusMaxCombo", "AccBonusComboDuration")
    "CrimsonTrailPlayer"     = @("AccTrailGainMult")
    "GuardianPlayer"         = @("AccAuraDamage", "AccAuraRadius", "AccAuraPulseMult")
    "ElementalistPlayer"     = @("AccSwitchCooldownCut", "AccSurgeBonusTicks")
    "NecromancerPlayer"      = @("AccReserveMult", "AccManaDrainMult")
}

foreach ($p in $playerHooks.Keys) {
    $src = Read-File (Join-Path $playerDir "$p.cs")

    $reset = [regex]::Match(
        $src,
        "public override void ResetEffects\(\)[\s\S]*?\n        \}",
        [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

    if ([string]::IsNullOrEmpty($reset)) {
        throw "$p needs a ResetEffects to clear its accessory hooks each frame."
    }

    foreach ($hook in $playerHooks[$p]) {
        if ($src -notmatch "public (float|int|bool) $hook") {
            throw "$p should expose the accessory hook '$hook'."
        }

        if ($reset -notmatch [regex]::Escape($hook)) {
            throw "$p.ResetEffects must reset '$hook' -- otherwise the accessory stacks forever."
        }
    }
}

# The Cursed Mage's corruption hook already existed; it must still be cleared.
$cursed = Read-File (Join-Path $playerDir "CursedMagePlayer.cs")
if ($cursed -notmatch "BaseCorruption\s*=\s*0") {
    throw "CursedMagePlayer must reset BaseCorruption each frame (curse accessories re-add it)."
}

Write-Host "Accessory source smoke test passed."
