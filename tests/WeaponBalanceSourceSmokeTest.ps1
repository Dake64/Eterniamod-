$ErrorActionPreference = "Stop"

# Weapon balance invariants.
#
# The four base-class starter weapons are craftable from the start and read as
# White (basic) tier.
#
# Every subclass PROMOTION reward is handed out at the SAME progression point --
# and promotion only happens in HARDMODE (ClassPromotionRules.ResolveSubclass
# returns the base class while !Main.hardMode). They used to be Blue with 8-20
# damage, so promoting was a DOWNGRADE: the Swordsman's reward did 16 while the
# pre-hardmode craftable Molten Gutripper does 27. They must now read as hardmode
# rewards: LightRed, promotion-only (no recipe), and stronger than the best
# pre-hardmode craftable OF THEIR OWN DAMAGE CLASS (whips and summon staves have
# intentionally low direct damage, so comparing them to a broadsword is wrong).

$repoRoot = Split-Path -Parent $PSScriptRoot
$w = Join-Path $repoRoot "Content\Items\Weapons"

$baseStarters = @(
    "Warrior\TrainingBlade.cs",
    "Magic\ApprenticeWand.cs",
    "Ranger\TrainingBow.cs",
    "Summoner\TrainingWhip.cs"
)

# Best damage craftable before hardmode, per damage class.
$preHardmodeBest = @{
    "Melee"  = 27   # Molten Gutripper
    "Ranged" = 9    # Training Bow
    "Magic"  = 10   # Apprentice Wand
    "Summon" = 8    # Training Whip
}

# Even the lowest-damage archetypes must still land in hardmode range.
$hardmodeFloor = 24

$promotionRewards = @{
    "Promotion\BloodletterBlade.cs"   = "Melee"
    "Fighter\TrainingGauntlet.cs"     = "Melee"
    "Guardian\TrainingShield.cs"      = "Melee"
    "Promotion\PracticeYoyo.cs"       = "Melee"
    "Promotion\RageCleaver.cs"        = "Melee"
    "Promotion\ImpactMace.cs"         = "Melee"
    "Promotion\EnergySidearm.cs"      = "Ranged"
    "Promotion\Longbow.cs"            = "Ranged"
    "Promotion\TrainingPistol.cs"     = "Ranged"
    "Promotion\ResonantCrossbow.cs"   = "Ranged"
    "Magic\ElementalApprenticeStaff.cs" = "Magic"
    "Magic\CursedApprenticeTome.cs"   = "Magic"
    "Promotion\InfinityTome.cs"       = "Magic"
    "Promotion\ArcaneFocus.cs"        = "Magic"
    "Promotion\BeastWhip.cs"          = "Summon"
    "Promotion\FusionWhip.cs"         = "Summon"
    "Promotion\TechWhip.cs"           = "Summon"
    "Summoner\BeginnerNecromancyBook.cs" = "Summon"
}

foreach ($rel in $baseStarters) {
    $content = Get-Content -Raw (Join-Path $w $rel)
    if ($content -notmatch "ItemRarityID\.White") {
        throw "Base-class starter '$rel' should be White (basic starter tier)."
    }
}

foreach ($entry in $promotionRewards.GetEnumerator()) {
    $rel = $entry.Key
    $damageClass = $entry.Value
    $content = Get-Content -Raw (Join-Path $w $rel)

    if ($content -notmatch "ItemRarityID\.LightRed") {
        throw "Promotion reward '$rel' should be LightRed (it is a hardmode subclass reward)."
    }

    if ($content -match "ItemRarityID\.(White|Blue)") {
        throw "Promotion reward '$rel' should not carry a pre-hardmode rarity."
    }

    # Promotion is the only way to get these; they are hardmode-tier.
    if ($content -match "public override void AddRecipes\(") {
        throw "Promotion reward '$rel' must not be craftable."
    }

    $match = [regex]::Match($content, "Item\.damage\s*=\s*(\d+);")
    if (-not $match.Success) {
        throw "Promotion reward '$rel' should declare Item.damage."
    }

    $damage = [int]$match.Groups[1].Value
    $beat = $preHardmodeBest[$damageClass]

    if ($damage -le $beat) {
        throw "Promotion reward '$rel' does $damage $damageClass damage, which does not beat the best pre-hardmode $damageClass craftable ($beat). Promoting must not be a downgrade."
    }

    if ($damage -lt $hardmodeFloor) {
        throw "Promotion reward '$rel' does $damage damage, below the hardmode floor ($hardmodeFloor)."
    }
}

Write-Host "Weapon balance source smoke test passed."
