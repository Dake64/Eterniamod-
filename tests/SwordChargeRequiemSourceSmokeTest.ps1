$ErrorActionPreference = "Stop"

# The last two Swordsman blades get mechanics that go beyond a slash: the Sanguine Cleaver's
# hold-to-charge guillotine (alt-fire) and the Crimson Requiem's mark-and-detonate. This test
# pins those exist, are wired, keep the normal left-click swing, and stay multiplayer-safe.

$repoRoot = Split-Path -Parent $PSScriptRoot
$projRoot = Join-Path $repoRoot "Content\Projectiles\Warrior"

$charge   = Get-Content -Raw (Join-Path $projRoot "SanguineCharge.cs")
$guillo   = Get-Content -Raw (Join-Path $projRoot "SanguineGuillotine.cs")
$deton    = Get-Content -Raw (Join-Path $projRoot "RequiemDetonation.cs")
$cleaver  = Get-Content -Raw (Join-Path $repoRoot "Content\Items\Weapons\Warrior\SanguineCleaver.cs")
$bleedNpc = Get-Content -Raw (Join-Path $repoRoot "Content\NPCs\BleedGlobalNPC.cs")
$ident    = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\SwordIdentityGlobalItem.cs")

# --- Sanguine Cleaver: hold-to-charge guillotine -------------------------------
if ($cleaver -notmatch "AltFunctionUse") {
    throw "SanguineCleaver should enable AltFunctionUse (right click charges)."
}
if ($cleaver -notmatch "Item\.channel = true") {
    throw "SanguineCleaver alt-fire must channel so player.channel stays true while held."
}
if ($cleaver -notmatch "SanguineCharge") {
    throw "SanguineCleaver should spawn the SanguineCharge held projectile on alt-fire."
}
# The left click must be untouched: Shoot returns true so the normal CrimsonSlash still fires.
if ($cleaver -notmatch "return true;") {
    throw "SanguineCleaver left click must still fire the normal slash (Shoot returns true)."
}

if ($charge -notmatch "class SanguineCharge\s*:\s*ModProjectile") {
    throw "SanguineCharge should be a ModProjectile."
}
# Charges while channelled, releases into the guillotine.
if ($charge -notmatch "owner\.channel") {
    throw "SanguineCharge must build/release off the owner's channel state."
}
if ($charge -notmatch "SanguineGuillotine") {
    throw "SanguineCharge must release a SanguineGuillotine payload."
}
# Owner-authoritative + a self-clean safety so a remote copy can't stick.
if ($charge -notmatch "Projectile\.owner != Main\.myPlayer") {
    throw "SanguineCharge must be owner-authoritative (only the owner drives release)."
}
if ($guillo -notmatch "class SanguineGuillotine\s*:\s*ModProjectile" -or $guillo -notmatch "DamageClass\.Melee") {
    throw "SanguineGuillotine should be a melee ModProjectile (inherits bleed/Trail)."
}
if ($guillo -notmatch "ai\[0\]") {
    throw "SanguineGuillotine should scale to the charge fraction (ai[0])."
}

# --- Crimson Requiem: mark and detonate ----------------------------------------
if ($bleedNpc -notmatch "RequiemMarks" -or $bleedNpc -notmatch "RequiemMarkTimer") {
    throw "BleedGlobalNPC should hold Requiem mark stacks and a fade timer."
}
# Marks must actually fade (decay), or the requiem wouldn't need to be 'sung'.
if ($bleedNpc -notmatch "RequiemMarks = 0") {
    throw "Requiem marks must fade when the timer runs out."
}
if ($ident -notmatch "case CrimsonRequiem:" -or $ident -notmatch "RequiemThreshold") {
    throw "SwordIdentityGlobalItem should mark for the requiem and detonate at a threshold."
}
if ($ident -notmatch "RequiemDetonation") {
    throw "The requiem must spawn RequiemDetonation when the marks reach the threshold."
}
if ($deton -notmatch "class RequiemDetonation\s*:\s*ModProjectile" -or $deton -notmatch "localNPCHitCooldown = -1") {
    throw "RequiemDetonation should be an AoE pulse that hits each foe once."
}

# --- Multiplayer safety: owner-only spawns -------------------------------------
# Titan guards with '!=' (early return); Bonewarden and Requiem with '=='. Accept both forms.
if (($ident | Select-String -Pattern "Main\.myPlayer [!=]= player\.whoAmI" -AllMatches).Matches.Count -lt 3) {
    throw "All bespoke on-hit spawns (Titan, Bonewarden, Requiem) must be owner-guarded."
}

Write-Host "Sword charge/requiem source smoke test passed."
