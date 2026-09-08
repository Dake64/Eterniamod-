$ErrorActionPreference = "Stop"

# The Swordsman blades used to throw ONE identical projectile (CrimsonSlash), differing only in
# colour and size. Each blade now drives a genuinely different slash BEHAVIOUR via SlashStyle,
# read by the one shared projectile. This test pins that the variety exists and can't silently
# collapse back to "same missile, different colour".

$repoRoot = Split-Path -Parent $PSScriptRoot
$weaponRoot = Join-Path $repoRoot "Content\Items\Weapons"

$iface = Get-Content -Raw (Join-Path $repoRoot "Content\Items\IBleedWeapon.cs")
$slash = Get-Content -Raw (Join-Path $repoRoot "Content\Projectiles\Warrior\CrimsonSlash.cs")

# --- The style system exists -------------------------------------------------
if ($iface -notmatch "enum SlashStyle") {
    throw "IBleedWeapon should define the SlashStyle enum that drives per-blade behaviour."
}

foreach ($style in @("Straight", "Pierce", "Wide", "Homing", "HomingAggressive", "Return", "Split")) {
    if ($iface -notmatch "\b$style\b") {
        throw "SlashStyle is missing the '$style' behaviour."
    }
}

if ($iface -notmatch "SlashStyle Style =>") {
    throw "IBleedWeapon should expose a Style property so each sword picks its behaviour."
}

# --- The projectile actually ACTS on the style -------------------------------
# Behaviour, not decoration: it must branch on the style and implement the distinct motions.
if ($slash -notmatch "bleed\.Style") {
    throw "CrimsonSlash should read the firing weapon's Style."
}

foreach ($behaviour in @(
    "SlashStyle\.Pierce",           # penetrate the line
    "SlashStyle\.Wide",             # slow wide crescent
    "SlashStyle\.Homing",           # steer toward a foe
    "SlashStyle\.Return",           # boomerang
    "SlashStyle\.Split")) {         # burst into fragments
    if ($slash -notmatch $behaviour) {
        throw "CrimsonSlash does not handle $behaviour -- that behaviour would be inert."
    }
}

# Homing must actually seek a target; a boomerang must actually come back.
if ($slash -notmatch "SteerTowardBleeding" -or $slash -notmatch "ReturnAI") {
    throw "Homing/Return styles must have real steering/return logic, not just a label."
}

# A Split fragment must be tagged so it can't split forever.
if ($slash -notmatch "IsFragment" -or $slash -notmatch "ai1: 1f") {
    throw "Split must flag its fragments (ai[1]=1) so they don't recursively split."
}

# --- Enough blades are genuinely non-Straight --------------------------------
# If only one or two swords diverged, the arsenal would still feel same-y.
$nonStraight = 0
Get-ChildItem -Recurse -File $weaponRoot -Filter "*.cs" | ForEach-Object {
    $src = Get-Content -Raw $_.FullName
    if ($src -match "SlashStyle\.(Pierce|Wide|Homing|HomingAggressive|Return|Split)") {
        $nonStraight++
    }
}

if ($nonStraight -lt 8) {
    throw "Only $nonStraight blades diverge from the plain slash; the arsenal is still too same-y."
}

Write-Host "Sword slash style source smoke test passed."
