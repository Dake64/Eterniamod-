$ErrorActionPreference = "Stop"

# The Swordsman blades used to throw ONE identical projectile: eight of them shared the plain
# Straight style, and every blade used the same slash sprite and the same blood dust. Each blade
# now drives its OWN motion, rhythm and particles via SlashStyle.
#
# The load-bearing assertion here is UNIQUENESS: no two weapons may share a style. That is the
# invariant that stops the arsenal collapsing back into "same weapon, different colour", and it is
# the thing a future edit is most likely to break by copy-pasting a weapon file.

$repoRoot = Split-Path -Parent $PSScriptRoot
$weaponRoot = Join-Path $repoRoot "Content\Items\Weapons"

$iface = Get-Content -Raw (Join-Path $repoRoot "Content\Items\IBleedWeapon.cs")
$slash = Get-Content -Raw (Join-Path $repoRoot "Content\Projectiles\Warrior\CrimsonSlash.cs")
$global = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\EterniaGlobalItem.cs")

# --- The style system exists, one value per weapon ----------------------------
if ($iface -notmatch "enum SlashStyle") {
    throw "IBleedWeapon should define the SlashStyle enum that drives per-blade behaviour."
}

$expected = @('Clean','Ripping','Lance','Lunge','Serpent','Growing','Seeding','Quick','Braking',
              'Ember','Echo','Cross','Wide','Shatter','Return','Homing','Quake','Mark','Phantom')

foreach ($style in $expected) {
    if ($iface -notmatch "\b$style\b") {
        throw "SlashStyle is missing the '$style' behaviour."
    }
}

if ($iface -notmatch "SlashStyle Style =>") {
    throw "IBleedWeapon should expose a Style property so each sword picks its behaviour."
}

# --- EVERY bleed weapon declares a style, and NO TWO SHARE ONE ----------------
$styleOf = @{}
Get-ChildItem -Recurse -File $weaponRoot -Filter "*.cs" | ForEach-Object {
    $src = Get-Content -Raw $_.FullName
    if ($src -notmatch "IBleedWeapon") { return }

    $m = [regex]::Match($src, 'SlashStyle Style => SlashStyle\.(\w+)')
    if (-not $m.Success) {
        throw "$($_.BaseName) implements IBleedWeapon but declares no SlashStyle."
    }
    $styleOf[$_.BaseName] = $m.Groups[1].Value
}

if ($styleOf.Count -lt 19) {
    throw "Expected at least 19 bleed weapons with a style; found $($styleOf.Count)."
}

$dupes = $styleOf.GetEnumerator() | Group-Object -Property Value | Where-Object { $_.Count -gt 1 }
if ($dupes) {
    $detail = ($dupes | ForEach-Object {
        "$($_.Name) used by " + (($_.Group | ForEach-Object { $_.Key }) -join ', ')
    }) -join ' | '
    throw "Two or more blades share a slash style -- they would feel like the same weapon: $detail"
}

# --- The projectile actually ACTS on the distinctive styles -------------------
# Behaviour, not decoration: these are the motions that make the blades read differently.
foreach ($behaviour in @(
    'SlashStyle\.Serpent',    # weaves as it travels
    'SlashStyle\.Growing',    # swells as it advances
    'SlashStyle\.Braking',    # bogs down
    'SlashStyle\.Lunge',      # pounces on bleeding prey
    'SlashStyle\.Seeding',    # plants thorns
    'SlashStyle\.Ember',      # sheds burning slag
    'SlashStyle\.Shatter',    # bursts into shards
    'SlashStyle\.Return',     # boomerang
    'SlashStyle\.Homing')) {  # hunts
    if ($slash -notmatch $behaviour) {
        throw "CrimsonSlash does not handle $behaviour -- that behaviour would be inert."
    }
}

if ($slash -notmatch "SteerTowardBleeding" -or $slash -notmatch "ReturnAI") {
    throw "Homing/Return styles must have real steering/return logic, not just a label."
}

# Seeding and Ember must spawn their own entities, not just emit dust.
if ($slash -notmatch "ThornSeed") { throw "Seeding must actually plant ThornSeed projectiles." }
if ($slash -notmatch "EmberPatch") { throw "Ember must actually drop EmberPatch projectiles." }

# Shards must be flagged so they never shatter recursively.
if ($slash -notmatch "IsShard" -or $slash -notmatch "ai1: 1f") {
    throw "Shatter must flag its shards (ai[1]=1) so they don't recursively shatter."
}

# --- Particles differ per style ----------------------------------------------
# If every style emitted DustID.Blood the blades would still feel identical.
$dusts = [regex]::Matches($slash, 'DustID\.(\w+)') | ForEach-Object { $_.Groups[1].Value } |
         Sort-Object -Unique
if ($dusts.Count -lt 8) {
    throw "Only $($dusts.Count) distinct dust types across all styles; particles are still same-y."
}

# --- Multi-projectile attack PATTERNS ----------------------------------------
# Three blades attack as a pattern rather than a single slash.
if ($global -notmatch "override bool Shoot") {
    throw "EterniaGlobalItem should override Shoot for the multi-projectile attack patterns."
}
foreach ($pattern in @('SlashStyle\.Ripping', 'SlashStyle\.Cross', 'SlashStyle\.Echo')) {
    if ($global -notmatch $pattern) {
        throw "The Shoot hook does not build the $pattern attack pattern."
    }
}
if ($global -notmatch "ai1: 2f") {
    throw "The Echo pattern must tag its ghost slash (ai[1]=2) so it draws faint and never echoes."
}

# --- Held-blade glow ----------------------------------------------------------
# Higher-tier blades light the player in their own colour. Plain steel must NOT glow:
# if every blade glows the glow stops signalling anything.
if ($global -notmatch "override void HoldItem") {
    throw "EterniaGlobalItem should light the player while a bleed katana is held."
}
if ($global -notmatch "Lighting\.AddLight") {
    throw "The held-blade glow must actually emit light."
}
if ($global -notmatch "SlashColor\.ToVector3") {
    throw "The glow should use the blade's own SlashColor, not one shared colour."
}
if ($global -notmatch "ItemRarityID\.White" -or $global -notmatch "return 0f") {
    throw "Plain-steel rarities must return zero glow so not every blade lights up."
}

Write-Host "Sword slash style source smoke test passed ($($styleOf.Count) blades, all distinct)."
