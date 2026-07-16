$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

# The whole fight lives in the shared base; each version is a thin subclass that only tunes
# numbers and loot.
$base = Read-File "Content\NPCs\Bosses\PrototypeBoss.cs"
$p01 = Read-File "Content\NPCs\Bosses\Prototype01.cs"
$p02 = Read-File "Content\NPCs\Bosses\Prototype02.cs"

# --- It is an actual boss ----------------------------------------------------

if ($base -notmatch "NPC\.boss\s*=\s*true") {
    throw "The Prototype line should be bosses."
}

# --- Three escalating phases keyed off the exposed core ----------------------

if ($base -notmatch "0\.70f" -or $base -notmatch "0\.35f") {
    throw "The Prototype should have three phases at 70% and 35% life."
}

if ($base -notmatch "OnPhaseChange") {
    throw "The Prototype should react when it changes phase (the core overloads)."
}

# --- The weapon modules (the whole point of the fight) -----------------------

foreach ($module in @("SwordDash", "PlasmaVolley", "LanceCharge", "DroneSpawn", "CoreVent")) {
    if ($base -notmatch $module) {
        throw "The Prototype should have its '$module' weapon module."
    }
}

if ($base -notmatch "phase >= 2" -or $base -notmatch "phase >= 3") {
    throw "The Prototype's drones and core vent should be gated to later phases."
}

# Boss-spawned projectiles must be server-authoritative.
if ($base -notmatch "netMode != NetmodeID\.MultiplayerClient") {
    throw "The Prototype should only spawn projectiles server-side."
}

# --- Custom placeholder draw -------------------------------------------------

if ($base -notmatch "PreDraw" -or $base -notmatch "DrawCore") {
    throw "The Prototype should custom-draw its placeholder body and glowing core."
}

# --- Two versions, tuned through the shared base -----------------------------

if ($p01 -notmatch "class Prototype01\s*:\s*PrototypeBoss" -or
    $p02 -notmatch "class Prototype02\s*:\s*PrototypeBoss") {
    throw "Prototype-01 and Prototype-02 should both extend the shared PrototypeBoss."
}

# Prototype-02 is the Hardmode escalation: sturdier + hits harder.
if ($p02 -notmatch "BaseLife" -or $p02 -notmatch "ProjectileDamageScale") {
    throw "Prototype-02 should turn the dials up (life + damage)."
}

foreach ($drop in @("PrototypeCore", "SoulAlloy", "SoulforgedSabre")) {
    if ($p01 -notmatch $drop) {
        throw "Prototype-01 should drop $drop."
    }
}

foreach ($drop in @("RefinedPrototypeCore", "SoulAlloy", "SoulforgedGreatsaber")) {
    if ($p02 -notmatch $drop) {
        throw "Prototype-02 should drop $drop."
    }
}

# --- Projectiles: hostile arsenal + the friendly weapon slash ----------------

foreach ($proj in @("PrototypePlasmaBolt", "PrototypeEnergySlash", "PrototypeDrone", "PrototypeShockwave")) {
    $p = Read-File "Content\Projectiles\Bosses\$proj.cs"
    if ($p -notmatch "Projectile\.hostile\s*=\s*true") {
        throw "$proj should be a hostile boss projectile."
    }
}

$slash = Read-File "Content\Projectiles\Bosses\SoulSlash.cs"
if ($slash -notmatch "Projectile\.friendly\s*=\s*true") {
    throw "SoulSlash (the weapon's shot) should be friendly."
}

# --- Summon items spawn the right boss and are craftable ---------------------

$summon1 = Read-File "Content\Items\Bosses\CorruptedSoulCore.cs"
if ($summon1 -notmatch "SpawnOnPlayer" -or $summon1 -notmatch "NPCType<Prototype01>") {
    throw "CorruptedSoulCore should summon Prototype-01."
}
if ($summon1 -notmatch "AnyNPCs") {
    throw "CorruptedSoulCore should not summon a second Prototype-01 while one is alive."
}

$summon2 = Read-File "Content\Items\Bosses\AwakenedSoulCore.cs"
if ($summon2 -notmatch "SpawnOnPlayer" -or $summon2 -notmatch "NPCType<Prototype02>") {
    throw "AwakenedSoulCore should summon Prototype-02."
}
if ($summon2 -notmatch "Main\.hardMode") {
    throw "AwakenedSoulCore should be a Hardmode summon."
}

# --- Boss Codex integration (lazy build so ModContent types are safe) --------

$codex = Read-File "Content\Progression\BossCodex.cs"

if ($codex -notmatch "ModContent\.NPCType<Prototype01>" -or $codex -notmatch "ModContent\.NPCType<Prototype02>") {
    throw "BossCodex should list both Prototypes as real, trackable entries."
}

if ($codex -notmatch "EnsureBuilt") {
    throw "BossCodex must build lazily -- a static ctor can't resolve modded NPC types."
}

# --- Localization (accept tModLoader's flat OR block form) -------------------

$loc = Read-File "en-US.hjson"

foreach ($npc in @("Prototype01", "Prototype02")) {
    $flat = $loc -match "$npc\.DisplayName:\s*Prototype"
    $block = $loc -match "(?ms)$npc`:\s*\{[^}]*DisplayName:\s*Prototype"
    if (-not ($flat -or $block)) {
        throw "en-US.hjson should localize $npc's name."
    }
}

foreach ($item in @("CorruptedSoulCore", "PrototypeCore", "SoulAlloy", "SoulforgedSabre",
                     "AwakenedSoulCore", "RefinedPrototypeCore", "SoulforgedGreatsaber")) {
    $flat = $loc -match "\b${item}\.DisplayName:"
    $block = $loc -match "(?ms)\b${item}:\s*\{[^}]*DisplayName:"
    if (-not ($flat -or $block)) {
        throw "en-US.hjson should localize $item."
    }
}

Write-Host "Boss Prototype source smoke test passed."
