$ErrorActionPreference = "Stop"

# Two Swordsman blades earned a TRULY bespoke mechanic -- a whole projectile of their own, not
# just a slash style: Titan's Gutcleaver's ground shockwave and the Bonewarden Sabre's bone spike.
# This test pins that those projectiles exist, behave like real Swordsman melee hits, and are
# wired up (and multiplayer-safe) from the on-hit identity table.

$repoRoot = Split-Path -Parent $PSScriptRoot
$projRoot = Join-Path $repoRoot "Content\Projectiles\Warrior"

$shock = Get-Content -Raw (Join-Path $projRoot "TitanShockwave.cs")
$spike = Get-Content -Raw (Join-Path $projRoot "BoneSpike.cs")
$ident = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\SwordIdentityGlobalItem.cs")

# --- The bespoke projectiles exist and are real melee hits ---------------------
foreach ($pair in @(
    @{ name = "TitanShockwave"; src = $shock },
    @{ name = "BoneSpike";      src = $spike })) {

    if ($pair.src -notmatch "class $($pair.name)\s*:\s*ModProjectile") {
        throw "$($pair.name) should be a ModProjectile with its own behaviour."
    }
    # Melee so it inherits the Swordsman bleed / Crimson Trail pipeline.
    if ($pair.src -notmatch "DamageClass\.Melee") {
        throw "$($pair.name) must be Melee so it inherits bleed/Trail like a slash."
    }
    # Drawn with dust only until real sprite art exists.
    if ($pair.src -notmatch "PreDraw.*=>\s*false") {
        throw "$($pair.name) is dust-drawn; PreDraw should return false."
    }
}

# The shockwave must actually roll (grow/travel) and the spike must actually hold still.
if ($shock -notmatch "penetrate = -1") {
    throw "TitanShockwave should roll THROUGH the whole line (penetrate -1)."
}
if ($spike -notmatch "velocity = Vector2\.Zero") {
    throw "BoneSpike should erupt in place and hold, not fly."
}

# --- Wired up from the on-hit identity table -----------------------------------
if ($ident -notmatch "case TitansGutcleaver:" -or $ident -notmatch "TitanShockwave") {
    throw "SwordIdentityGlobalItem should spawn the Titan shockwave for Titan's Gutcleaver."
}
if ($ident -notmatch "case BonewardenSabre:" -or $ident -notmatch "BoneSpike") {
    throw "SwordIdentityGlobalItem should spawn a bone spike for the Bonewarden Sabre."
}

# --- Multiplayer-safe: only the owner spawns; the quake doesn't stack per enemy -
if (($ident | Select-String -Pattern "Main\.myPlayer" -AllMatches).Matches.Count -lt 2) {
    throw "Both bespoke spawns must be guarded by Main.myPlayer so only the owner spawns them."
}
# The shockwave dedupes per swing by checking for a freshly-spawned wave already out.
if ($ident -notmatch "timeLeft > 24") {
    throw "The Titan shockwave must dedupe per swing (one quake, not one per enemy clipped)."
}

Write-Host "Sword bespoke projectile source smoke test passed."
