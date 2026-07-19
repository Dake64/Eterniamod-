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
    "private static \(string mechanic, string creed, string how, Color accent\) Identity\(string name\)[\s\S]*?\n        \}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ([string]::IsNullOrEmpty($identity)) {
    throw "AwakeningCeremony should map each subclass to its mechanic."
}

foreach ($s in $subclasses) {
    if ($identity -notmatch "`"$s`"\s*=>") {
        throw "'$s' has no Awakening identity -- it would be handed a mechanic with no explanation."
    }
}


# --- The ceremony must TEACH, not just name ------------------------------------
# Playtest concern: "como el jugador se dara cuenta de la mecanica de la subclase".
# Naming the mechanic ("CRIMSON TRAIL") and a creed is flavour; without a concrete
# how-to the player is handed a system nothing in the game explains.
$banner = Read-File "Content\UI\PromotionBannerUI.cs"

# Every arm of the table is (mechanic, creed, how, colour): count the entries and make
# sure none was left with only flavour text.
$arms = [regex]::Matches($identity, '"[^"]+" =>')

if ($arms.Count -lt 17) {
    throw "Identity should cover every subclass (found $($arms.Count), expected 17+)."
}

$colours = [regex]::Matches($identity, 'new Color\(')

if ($arms.Count -ne $colours.Count) {
    throw "Every subclass arm should carry its own accent colour."
}

# Three quoted strings per arm (mechanic + creed + how). Two would mean a missing lesson.
$quoted = [regex]::Matches($identity, '"[^"]{4,}"')

if ($quoted.Count -lt ($arms.Count * 3)) {
    throw ("Each subclass needs a how-it-works line as well as a mechanic name and creed " +
        "(found $($quoted.Count) strings for $($arms.Count) subclasses).")
}

if ($ceremony -notmatch "\{KEY\}" -or $ceremony -notmatch "SkillKeyLabel") {
    throw "Instructions should name the player's REAL skill key, not a hardcoded letter."
}

if ($ceremony -notmatch "Main\.NewText") {
    throw "The lesson should also go to chat: the banner fades and can be missed entirely."
}

if ($banner -notmatch "howLine") {
    throw "PromotionBannerUI should render the how-it-works line."
}

Write-Host "Awakening ceremony source smoke test passed."
