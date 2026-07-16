$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

$boss = Read-File "Content\NPCs\Bosses\Prototype01.cs"

# --- It is an actual boss ----------------------------------------------------

if ($boss -notmatch "NPC\.boss\s*=\s*true") {
    throw "Prototype-01 should be a boss."
}

# --- Three escalating phases keyed off the exposed core ----------------------

if ($boss -notmatch "0\.70f" -or $boss -notmatch "0\.35f") {
    throw "Prototype-01 should have three phases at 70% and 35% life."
}

if ($boss -notmatch "OnPhaseChange") {
    throw "Prototype-01 should react when it changes phase (the core overloads)."
}

# --- The weapon modules (the whole point of the fight) -----------------------

foreach ($module in @("SwordDash", "PlasmaVolley", "LanceCharge", "DroneSpawn", "CoreVent")) {
    if ($boss -notmatch $module) {
        throw "Prototype-01 should have its '$module' weapon module."
    }
}

# Later phases must actually gate the extra modules.
if ($boss -notmatch "phase >= 2" -or $boss -notmatch "phase >= 3") {
    throw "Prototype-01's drones and core vent should be gated to later phases."
}

# --- Custom placeholder draw + loot -----------------------------------------

if ($boss -notmatch "PreDraw" -or $boss -notmatch "DrawCore") {
    throw "Prototype-01 should custom-draw its placeholder body and glowing core."
}

foreach ($drop in @("PrototypeCore", "SoulAlloy", "SoulforgedSabre")) {
    if ($boss -notmatch $drop) {
        throw "Prototype-01 should drop $drop."
    }
}

# --- Projectiles: hostile arsenal + the friendly weapon slash ----------------

$hostile = @("PrototypePlasmaBolt", "PrototypeEnergySlash", "PrototypeDrone", "PrototypeShockwave")

foreach ($proj in $hostile) {
    $p = Read-File "Content\Projectiles\Bosses\$proj.cs"

    if ($p -notmatch "Projectile\.hostile\s*=\s*true") {
        throw "$proj should be a hostile boss projectile."
    }
}

$slash = Read-File "Content\Projectiles\Bosses\SoulSlash.cs"

if ($slash -notmatch "Projectile\.friendly\s*=\s*true") {
    throw "SoulSlash (the weapon's shot) should be friendly."
}

# --- Summon item spawns the boss and is craftable ----------------------------

$summon = Read-File "Content\Items\Bosses\CorruptedSoulCore.cs"

if ($summon -notmatch "SpawnOnPlayer" -or $summon -notmatch "NPCType<Prototype01>") {
    throw "CorruptedSoulCore should summon Prototype-01."
}

if ($summon -notmatch "AnyNPCs<" -and $summon -notmatch "AnyNPCs\(") {
    throw "CorruptedSoulCore should not summon a second Prototype-01 while one is alive."
}

if ($summon -notmatch "CreateRecipe") {
    throw "CorruptedSoulCore should be craftable from salvaged tech."
}

# --- Boss Codex integration (lazy build so ModContent types are safe) --------

$codex = Read-File "Content\Progression\BossCodex.cs"

if ($codex -notmatch "ModContent\.NPCType<Prototype01>") {
    throw "BossCodex should list Prototype-01 as a real, trackable entry."
}

if ($codex -notmatch "EnsureBuilt") {
    throw "BossCodex must build lazily -- a static ctor can't resolve modded NPC types."
}

# --- Localization ------------------------------------------------------------

$loc = Read-File "en-US.hjson"

# tModLoader OWNS this file and rewrites it on load -- a single-field entry becomes the flat
# "Key.DisplayName: value" form, a multi-field one stays a "Key: { ... }" block. Accept either.
$protoFlat = $loc -match "Prototype01\.DisplayName:\s*Prototype-01"
$protoBlock = $loc -match "(?ms)Prototype01:\s*\{[^}]*DisplayName:\s*Prototype-01"

if (-not ($protoFlat -or $protoBlock)) {
    throw "en-US.hjson should localize Prototype-01's name."
}

foreach ($item in @("CorruptedSoulCore", "PrototypeCore", "SoulAlloy", "SoulforgedSabre")) {
    $flat = $loc -match "\b${item}\.DisplayName:"
    $block = $loc -match "(?ms)\b${item}:\s*\{[^}]*DisplayName:"

    if (-not ($flat -or $block)) {
        throw "en-US.hjson should localize $item."
    }
}

Write-Host "Boss Prototype-01 source smoke test passed."
