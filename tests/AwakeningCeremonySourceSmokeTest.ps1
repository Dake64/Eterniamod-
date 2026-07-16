$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

$ceremony = Read-File "Content\Systems\AwakeningCeremony.cs"
$reward = Read-File "Content\Players\PromotionRewardPlayer.cs"

# --- Triggered by the single-fire promotion event ----------------------------
# PromotionRewardPlayer already fires exactly once per promotion (guarded by its saved
# awardedPromotions list). That is where the Awakening belongs -- co-located with the
# reward grant, so there is no second system racing it in the same frame.

if ($reward -notmatch "AwakeningCeremony\.Begin\(subclass\)") {
    throw "PromotionRewardPlayer should begin the Awakening ceremony on promotion."
}

# The old plain banner + sound must not ALSO fire here, or the moment double-triggers.
if ($reward -match "PromotionBannerUI\.Show" -or $reward -match "SoundEngine\.PlaySound") {
    throw "PromotionRewardPlayer should hand the whole moment to the ceremony, not double-fire a banner/sound."
}

# --- The two FX beats, then hand-off to the banner ---------------------------

if ($ceremony -notmatch "class AwakeningCeremony\s*:\s*ModSystem") {
    throw "AwakeningCeremony should be a ModSystem."
}

foreach ($beat in @("GatherTicks", "BurstTicks")) {
    if ($ceremony -notmatch $beat) {
        throw "The ceremony should have its '$beat' beat."
    }
}

# It must actually BE felt: particles, a shockwave, a camera kick, sound.
foreach ($fx in @("Dust\.NewDustPerfect", "PunchCameraModifier", "SoundEngine\.PlaySound")) {
    if ($ceremony -notmatch $fx) {
        throw "The Awakening should be felt, not just read ($fx missing)."
    }
}

if ($ceremony -notmatch "ModifyInterfaceLayers") {
    throw "The ceremony should draw to the screen (dim + flash)."
}

# The banner is raised by the ceremony at the burst, so it slides in as the flash clears.
if ($ceremony -notmatch "PromotionBannerUI\.Prepare" -or $ceremony -notmatch "PromotionBannerUI\.Fire") {
    throw "The ceremony should prepare and fire the promotion banner at the burst."
}

# --- EVERY subclass must have its own identity -------------------------------
# You are handed an entire new gameplay system at this moment and nothing else in the
# game explains it. A subclass missing from this table would awaken with no idea what
# mechanic it just inherited.

$subclasses = @(
    "Fighter", "Swordsman", "Guardian", "Berserker", "Stunner", "Yoyo Master",
    "Elementalist", "Cursed Mage", "Necromancer", "Infinity Mage", "Arcane Bard",
    "Energy Gunner", "Archer", "Gunner", "Virtuoso",
    "Beast Tamer", "Advanced Summoner", "Tech Summoner")

$identity = [regex]::Match(
    $ceremony,
    "private static \(string mechanic, string creed, Color accent\) Identity\(string name\)[\s\S]*?\n        \}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ([string]::IsNullOrEmpty($identity)) {
    throw "AwakeningCeremony should map each subclass to its mechanic."
}

foreach ($s in $subclasses) {
    if ($identity -notmatch "`"$s`"\s*=>") {
        throw "'$s' has no Awakening identity -- it would be handed a mechanic with no explanation."
    }
}

Write-Host "Awakening ceremony source smoke test passed."
