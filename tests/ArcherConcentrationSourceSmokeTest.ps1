$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$playerPath = Join-Path $repoRoot "Content\Players\ArcherPlayer.cs"
$globalPath = Join-Path $repoRoot "Content\Globals\ArcherGlobalProjectile.cs"

foreach ($p in @($playerPath, $globalPath)) {
    if (-not (Test-Path $p)) {
        throw "Missing expected file: $p"
    }
}

$player = Get-Content -Raw $playerPath
$global = Get-Content -Raw $globalPath

# --- Concentration backbone --------------------------------------------------

foreach ($member in @("public float Focus", "const float MaxFocus", "float FocusPercent", "bool PerfectReady", "bool ShotIsPerfect", "bool ShotIsLegendary")) {
    if ($player -notmatch [regex]::Escape($member)) {
        throw "ArcherPlayer should expose '$member' for the Concentration mechanic."
    }
}

# A base Ranger also learns the mechanic; only a promoted Archer gets the full version.
if ($player -notmatch "bool IsRangerLearning\(") {
    throw "ArcherPlayer should let a base Ranger learn Concentration (IsRangerLearning)."
}

# --- Fills when not firing, faster while still; blocked by nearby enemies -----

if ($player -notmatch "ticksSinceFire" -or $player -notmatch "Focus\s*\+=") {
    throw "Concentration should regenerate after holding fire."
}

if ($player -notmatch "velocity\.Length\(\)\s*<\s*0\.5f") {
    throw "Standing still should charge Concentration faster."
}

if ($player -notmatch "EnemyTooClose\(\)") {
    throw "A nearby enemy should block a promoted Archer's Concentration regen."
}

# --- Firing consumes; taking damage reduces ----------------------------------

if ($player -notmatch "Focus\s*-=") {
    throw "Firing a bow should consume Concentration."
}

if ($player -notmatch "OnHurt" -or $player -notmatch "Focus\s*-=\s*30f") {
    throw "Taking damage should reduce Concentration."
}

# --- Tiered bonus (31 / 61 thresholds) ---------------------------------------

if ($player -notmatch "Focus\s*>=\s*61f" -or $player -notmatch "Focus\s*>=\s*31f") {
    throw "Concentration should grant tiered bonuses at 31 and 61."
}

# --- Perfect Shot ------------------------------------------------------------

# Decided up front so the damage/crit hooks empower the shot.
if ($player -notmatch "ShotIsPerfect\s*=\s*IsActiveArcher\(\)\s*&&\s*Focus\s*>=\s*MaxFocus") {
    throw "A Perfect Shot should trigger when a promoted Archer fires at a full bar."
}

if ($player -notmatch "damage\s*\*=\s*1\.35f" -and $player -notmatch "1\.55f") {
    throw "Perfect Shots should deal much more damage (Deadeye scales it)."
}

if ($player -notmatch "crit\s*\+=\s*25f") {
    throw "Perfect Shots should grant +25 crit."
}

# Perfect Shot burns almost the whole bar (back to ~10).
# A Perfect Shot still empties the bar back to ~10 at the base tier; the milestone tiers
# leave progressively more standing so perfects can chain. The `_ =>` arm is the base case.
if ($player -notmatch "_\s*=>\s*10f") {
    throw "A Perfect Shot should leave the bar near 10 at the base tier."
}

if ($player -notmatch "MechanicTier\.") {
    throw "Concentration should grow with the hardmode milestones."
}

# --- Hawkeye ultimate: every 8th Perfect Shot is a Legendary Shot ------------

if ($player -notmatch "perfectShotCount\s*%\s*8" -or $player -notmatch "HasKeystone\(`"Hawkeye`"\)") {
    throw "Hawkeye should turn every 8th Perfect Shot into a Legendary Shot."
}

if ($player -notmatch "damage\s*\*=\s*1\.80f" -or $player -notmatch "crit\s*\+=\s*50f") {
    throw "Legendary Shots should be far stronger than a Perfect Shot."
}

# --- Passive tree shaping (read in ArcherPlayer) -----------------------------

foreach ($node in @("Eagle Eye", "Hunter Instinct", "Storm of Arrows")) {
    if ($player -notmatch "HasPassive\(`"$node`"\)") {
        throw "Bow node '$node' should shape the Concentration mechanic in ArcherPlayer."
    }
}

# --- Global projectile: distance sniper + Perfect/Legendary behaviour --------

if ($global -notmatch "class ArcherGlobalProjectile") {
    throw "ArcherGlobalProjectile should exist."
}

# Distance bonus: 10 blocks -> 0%, 50+ blocks -> +40%.
if ($global -notmatch "Vector2\.Distance" -or $global -notmatch "0\.40f") {
    throw "Arrow damage should scale with distance to the target (up to +40%)."
}

# Sniper (Marksman) amplifies the distance bonus.
if ($global -notmatch "Marksman" -or $global -notmatch "distBonus\s*\*=\s*1\.5f") {
    throw "Sniper (Marksman) should scale the distance bonus higher."
}

# Predator (Volley): extra damage to full-health enemies.
if ($global -notmatch "Volley" -or $global -notmatch "target\.life\s*>=\s*target\.lifeMax") {
    throw "Predator (Volley) should add damage to full-health enemies."
}

# Weak Point (True Flight) + Piercing Arrow (Piercing Shot) on Perfect Shots.
if ($global -notmatch "True Flight" -or $global -notmatch "ArmorPenetration") {
    throw "Weak Point (True Flight) should make Perfect Shots ignore more defense."
}

if ($global -notmatch "Piercing Shot" -or $global -notmatch "penetrate") {
    throw "Piercing Arrow (Piercing Shot) should let Perfect Shots pierce."
}

# Legendary Shots pierce everything.
if ($global -notmatch "penetrate\s*=\s*-1") {
    throw "Legendary Shots should pierce all enemies."
}

Write-Host "Archer Concentration source smoke test passed."
