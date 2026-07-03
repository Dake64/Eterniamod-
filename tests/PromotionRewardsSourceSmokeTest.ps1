$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$rewardPath = Join-Path $repoRoot "Content\Players\PromotionRewardPlayer.cs"
$promotionWeaponDir = Join-Path $repoRoot "Content\Items\Weapons\Promotion"
$lockHelperPath = Join-Path $promotionWeaponDir "SubclassLockHelper.cs"
$lockedItemPath = Join-Path $promotionWeaponDir "SubclassLockedItem.cs"
$gauntletPath = Join-Path $repoRoot "Content\Items\Weapons\Fighter\TrainingGauntlet.cs"
$shieldPath = Join-Path $repoRoot "Content\Items\Weapons\Guardian\TrainingShield.cs"
$elementalStaffPath = Join-Path $repoRoot "Content\Items\Weapons\Magic\ElementalApprenticeStaff.cs"
$cursedTomePath = Join-Path $repoRoot "Content\Items\Weapons\Magic\CursedApprenticeTome.cs"
$necroBookPath = Join-Path $repoRoot "Content\Items\Weapons\Summoner\BeginnerNecromancyBook.cs"

if (!(Test-Path $rewardPath)) {
    throw "Missing required source file: $rewardPath"
}

$reward = Get-Content -Raw $rewardPath
$lockHelper = Get-Content -Raw $lockHelperPath
$lockedItem = Get-Content -Raw $lockedItemPath
$gauntlet = Get-Content -Raw $gauntletPath
$shield = Get-Content -Raw $shieldPath
$elementalStaff = Get-Content -Raw $elementalStaffPath
$cursedTome = Get-Content -Raw $cursedTomePath
$necroBook = Get-Content -Raw $necroBookPath

$expectedRewards = @{
    "Swordsman" = "BloodletterBlade"
    "Fighter" = "TrainingGauntlet"
    "Guardian" = "TrainingShield"
    "Yoyo Master" = "PracticeYoyo"
    "Berserker" = "RageCleaver"
    "Stunner" = "ImpactMace"
    "Energy Gunner" = "EnergySidearm"
    "Archer" = "Longbow"
    "Gunner" = "TrainingPistol"
    "Virtuoso" = "ResonantCrossbow"
    "Elementalist" = "ElementalApprenticeStaff"
    "Cursed Mage" = "CursedApprenticeTome"
    "Infinity Mage" = "InfinityTome"
    "Arcane Bard" = "ArcaneFocus"
    "Beast Tamer" = "BeastWhip"
    "Advanced Summoner" = "FusionWhip"
    "Tech Summoner" = "TechWhip"
    "Necromancer" = "BeginnerNecromancyBook"
}

foreach ($entry in $expectedRewards.GetEnumerator()) {
    if ($reward -notmatch [regex]::Escape("`"$($entry.Key)`"")) {
        throw "PromotionRewardPlayer should define a reward for '$($entry.Key)'."
    }

    if ($reward -notmatch [regex]::Escape($entry.Value)) {
        throw "PromotionRewardPlayer should grant '$($entry.Value)' for '$($entry.Key)'."
    }
}

if ($reward -notmatch "AwardedPromotions") {
    throw "Promotion rewards should be persisted so they are granted once."
}

if ($reward -notmatch "ClassPromotionRules\.GetBaseClassName") {
    throw "PromotionRewardPlayer should avoid granting rewards for base classes."
}

if ($reward -notmatch "ClassPromotionRules\.IsPromotionForSoul") {
    throw "PromotionRewardPlayer should only grant rewards for promotions owned by the active Soul."
}

if ($lockHelper -notmatch "Requires promotion:") {
    throw "SubclassLockHelper should provide a clear required-promotion tooltip."
}

if ($lockedItem -notmatch "ModifyTooltips" -or
    $lockedItem -notmatch "SubclassLockHelper\.AddTooltip") {
    throw "SubclassLockedItem should add the required-promotion tooltip."
}

if ($shield -notmatch "Item\.damage\s*=\s*9") {
    throw "TrainingShield should have non-zero damage so Guardian can test it."
}

if ($gauntlet -notmatch "SubclassLockHelper\.AddTooltip" -or
    $gauntlet -notmatch '"Fighter"') {
    throw "TrainingGauntlet should show a Fighter required-promotion tooltip."
}

if ($shield -notmatch "SubclassLockHelper\.AddTooltip" -or
    $shield -notmatch '"Guardian"') {
    throw "TrainingShield should be restricted to Guardian."
}

if ($elementalStaff -notmatch "SubclassLockHelper\.AddTooltip" -or
    $elementalStaff -notmatch '"Elementalist"') {
    throw "ElementalApprenticeStaff should be restricted to Elementalist."
}

if ($cursedTome -notmatch "SubclassLockHelper\.AddTooltip" -or
    $cursedTome -notmatch '"Cursed Mage"') {
    throw "CursedApprenticeTome should show a Cursed Mage required-promotion tooltip."
}

if ($necroBook -notmatch "SubclassLockHelper\.AddTooltip" -or
    $necroBook -notmatch '"Necromancer"') {
    throw "BeginnerNecromancyBook should show a Necromancer required-promotion tooltip."
}

foreach ($entry in $expectedRewards.GetEnumerator()) {
    $itemPath = Join-Path $promotionWeaponDir "$($entry.Value).cs"

    if (!(Test-Path $itemPath)) {
        continue
    }

    $item = Get-Content -Raw $itemPath

    if ($item -notmatch [regex]::Escape("RequiredSubclass => `"$($entry.Key)`"")) {
        throw "$($entry.Value) should be locked to '$($entry.Key)'."
    }

    if ($item -notmatch "DamageClass\.(Melee|MeleeNoSpeed|Ranged|Magic|SummonMeleeSpeed)") {
        throw "$($entry.Value) should define a concrete allowed DamageClass."
    }
}

foreach ($ammo in @("ItemID.WoodenArrow", "ItemID.MusketBall")) {
    if ($reward -notmatch [regex]::Escape($ammo)) {
        throw "PromotionRewardPlayer should grant ammo '$ammo' for ammo-based rewards."
    }
}

Write-Host "Promotion rewards source smoke test passed."
