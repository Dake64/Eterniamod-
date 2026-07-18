$ErrorActionPreference = "Stop"

# Crimson Trail (Rastro Carmesi) is the Swordsman's EXCLUSIVE resource:
#  - it only exists while an active Swordsman (Warrior Soul + Swordsman subclass),
#  - it is earned in combat (applying bleed / hitting bleeding enemies),
#  - it has NO automatic regeneration,
#  - it is spent only by a Swordsman technique bound to the class Skill key,
#  - it is shown via its own on-screen bar,
#  - its logic is kept separate from the bleed system.

$repoRoot = Split-Path -Parent $PSScriptRoot
$contentRoot = Join-Path $repoRoot "Content"

$resource = Get-Content -Raw (Join-Path $contentRoot "Players\CrimsonTrailPlayer.cs")
$swordsman = Get-Content -Raw (Join-Path $contentRoot "Players\SwordsmanPlayer.cs")
$skill = Get-Content -Raw (Join-Path $contentRoot "Players\SwordsmanSkillPlayer.cs")
$ui = Get-Content -Raw (Join-Path $contentRoot "UI\CrimsonTrailUI.cs")

# --- Resource: Swordsman-only, no auto-regen, persisted ---------------------
if ($resource -notmatch "public int CrimsonTrail" -or
    $resource -notmatch "MaxCrimsonTrail") {
    throw "CrimsonTrailPlayer should expose the CrimsonTrail resource with a cap."
}

if ($resource -notmatch "IsActiveSwordsman\(\)") {
    throw "CrimsonTrail should only exist for an active Swordsman."
}

if ($resource -notmatch "CrimsonTrail = 0") {
    throw "CrimsonTrail should reset to 0 when the player is not an active Swordsman."
}

# The clear MUST live in PostUpdate, not ResetEffects: ResetEffects runs before the class Soul
# re-activates each frame, so gating the wipe on IsActiveSwordsman() there wiped the resource the
# instant it was earned (playtest bug 2026-07-16).
if ($resource -notmatch "public override void PostUpdate") {
    throw "CrimsonTrail should be cleared in PostUpdate (late), not ResetEffects."
}

# It must never regenerate on its own -- the only gain is Add(), called from combat hooks.
if ($resource -match "CrimsonTrail \+=" -and $resource -notmatch "public void Add") {
    throw "CrimsonTrail must not regenerate automatically; the only gain is Add()."
}

if ($resource -notmatch "public void Add\(" -or
    $resource -notmatch "public bool TrySpend\(") {
    throw "CrimsonTrailPlayer should expose Add(...) and TrySpend(...)."
}

if ($resource -notmatch 'tag\["CrimsonTrail"\]' -or
    $resource -notmatch "SaveData" -or
    $resource -notmatch "LoadData") {
    throw "CrimsonTrailPlayer should persist the resource via SaveData/LoadData."
}

# --- Earned in combat via the Swordsman on-hit hook -------------------------
if ($swordsman -notmatch "GetModPlayer<CrimsonTrailPlayer>\(\)" -or
    $swordsman -notmatch "\.Add\(") {
    throw "SwordsmanPlayer should grant Crimson Trail on bleed hits."
}

# --- Spent only by a Skill-key technique ------------------------------------
if ($skill -notmatch "ProcessTriggers") {
    throw "SwordsmanSkillPlayer should handle the technique in ProcessTriggers."
}

if ($skill -notmatch "IsActiveSwordsman\(\)") {
    throw "SwordsmanSkillPlayer should require an active Swordsman."
}

if ($skill -notmatch "EterniaKeybinds\.SkillKey") {
    throw "SwordsmanSkillPlayer should bind the technique to the class Skill key."
}

if ($skill -notmatch "TrySpend\(") {
    throw "The technique should consume Crimson Trail via TrySpend."
}

if ($skill -notmatch "SetCooldown\(") {
    throw "The technique should use the shared skill cooldown."
}

if ($skill -notmatch "SimpleStrikeNPC" -or
    $skill -notmatch "EXECUTE") {
    throw "The technique should deal the reborn EXECUTE burst to bleeding enemies."
}

# --- The key must never feel dead: loud feedback on EVERY failed press ---------
# Playtest 2026-07-16: the only feedback was tiny combat text, so the player read a
# non-firing press as "the key does nothing".
if ($skill -notmatch "EN ENFRIAMIENTO" -or
    $skill -notmatch "RASTRO " -or
    $skill -notmatch "NADIE SANGRA") {
    throw "Every failed technique press should announce WHY it did not fire."
}

if ($skill -notmatch "SoundEngine\.PlaySound\(SoundID\.MenuClose") {
    throw "A denied technique press should play an audible sound, not just draw text."
}

# CombatText's dramatic flag is what makes the announcement big enough to notice.
if ($skill -notmatch "CombatText\.NewText\(Player\.Hitbox, color, text, true\)") {
    throw "Technique announcements should use dramatic (large) combat text."
}

# The target check MUST come before TrySpend: a mistimed press with nothing bleeding
# used to burn the full 50 Trail for zero effect.
$targetCheck = $skill.IndexOf("CountBleedingInRange() == 0")
$spend = $skill.IndexOf("TrySpend(")

if ($targetCheck -lt 0 -or $spend -lt 0 -or $targetCheck -gt $spend) {
    throw "The technique should verify a bleeding target BEFORE spending Crimson Trail."
}

# --- Dedicated on-screen bar ------------------------------------------------
if ($ui -notmatch "class CrimsonTrailUI\s*:\s*ModSystem" -or
    $ui -notmatch "IsActiveSwordsman\(\)" -or
    $ui -notmatch "DrawFloatingResourceBar") {
    throw "CrimsonTrailUI should draw a Swordsman-only resource bar."
}

Write-Host "Crimson Trail source smoke test passed."
