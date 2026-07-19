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

# Trail is banked ONLY from blood already running: the strike that opens a wound earns
# nothing. Both entry points (direct strike and the thrown slash) must bail on !wasBleeding
# BEFORE granting, or the subclass goes back to paying most for hit-and-move-on crowd farming.
$grants = [regex]::Matches($swordsman, 'if \(!wasBleeding\)')

if ($grants.Count -lt 2) {
    throw "Both the direct hit and the slash should refuse Trail when the target was not already bleeding."
}

foreach ($hook in @("OnHitNPCWithItem", "OnHitNPCWithProj")) {
    $body = [regex]::Match(
        $swordsman,
        "public override void $hook\([\s\S]+?\n\s{8}\}",
        [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

    $bail = $body.IndexOf("if (!wasBleeding)")
    $grant = $body.IndexOf(".Add(")

    if ($bail -lt 0 -or $grant -lt 0 -or $bail -gt $grant) {
        throw "$hook should bail on a non-bleeding target before granting Crimson Trail."
    }

    # These hooks fire once PER ENEMY STRUCK, so a wide swing through a crowd banked once per
    # enemy it clipped -- +30 from a single swing, which is what made hordes fill instantly.
    $window = $body.IndexOf("CanBankTrail()")

    if ($window -lt 0 -or $window -gt $grant) {
        throw "$hook should pass the per-swing window before granting, or a cleave banks per enemy."
    }
}

if ($swordsman -notmatch "MaxTrailGainsPerWindow" -or
    $swordsman -notmatch "TrailWindowFrames") {
    throw "The per-swing cap should stay tunable through named constants."
}

# The old first-blood bonus paid DOUBLE for opening a wound -- the exact opposite of the rule.
if ($swordsman -match "FirstBloodGain") {
    throw "The first-blood bonus contradicts banking from existing bleed; it should stay removed."
}

# --- Standing bleed income ------------------------------------------------------
# Blood already drawn keeps paying, so the resource flows from the field bleeding rather
# than only from swinging at it.
$income = [regex]::Match(
    $swordsman,
    'public override void PostUpdate\(\)[\s\S]+?\n\s{8}\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($income -notmatch "TrailPerBleedingEnemy") {
    throw "Bleeding enemies should feed the Trail over time, not only on hit."
}

if ($income -notmatch "BleedOwner != Player\.whoAmI") {
    throw "Only your own bleeds should pay you; another Warrior's bleed is their income."
}

# The Hemorrhage tier bleeds a whole 28-tile zone. Uncapped, an event would refill the bar
# faster than the technique's cooldown and turn it into a held button.
if ($income -notmatch "BleedingEnemyCap\(\)") {
    throw "Bleed income must be capped, or mass bleeding trivialises the technique's cost."
}

if ($swordsman -notmatch "MaxBleedingEnemiesCounted = 3") {
    throw "The bleed income cap should stay low; at 8 the technique refilled itself in ~6s."
}

# --- The tree must actually feed the subclass's own resource --------------------
# Every node above these was generic melee/bleed power: the signature mechanic had no
# passive support at all in its own branch.
foreach ($node in @("Blood Tithe", "Open Veins")) {
    if ($swordsman -notmatch [regex]::Escape($node)) {
        throw "The Bleed branch node '$node' should have a runtime effect on the Trail."
    }
}

if ($skill -notmatch "Merciless" -or $skill -notmatch "public int EffectiveCost") {
    throw "The execution's cost should be dynamic so Merciless and the keystone can move it."
}

# The keystone used to cost attack speed, which starved the very resource it sits on.
if ($skill -notmatch "Hemorrhagic Frenzy") {
    throw "The Bleed keystone should pay its price at the finisher, not with attack speed."
}

$ui = Get-Content -Raw (Join-Path $contentRoot "UI\CrimsonTrailUI.cs")

if ($ui -notmatch "EffectiveCost\(\)") {
    throw "The gauge's fire line must track the real cost, or it points at a stale number."
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
if ($skill -notmatch "ON COOLDOWN" -or
    $skill -notmatch "CRIMSON \{" -or
    $skill -notmatch "NOTHING BLEEDING") {
    throw "Every failed technique press should announce WHY it did not fire."
}

# The mod ships in English; player-facing strings must not drift into another language just
# because the author and I discuss it in Spanish. Matched as "any non-ASCII character" rather
# than by listing accented letters, so this file stays pure ASCII and cannot be corrupted by
# the UTF-8/ANSI mismatch in Windows PowerShell 5.1.
if ($skill -match '[^\x00-\x7F]') {
    throw "SwordsmanSkillPlayer contains non-ASCII text; player-facing strings should be English."
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
$targetCheck = $skill.IndexOf("CountTargetsInRange(tier, radius) == 0")
$spend = $skill.IndexOf("TrySpend(")

if ($targetCheck -lt 0 -or $spend -lt 0 -or $targetCheck -gt $spend) {
    throw "The technique should verify a valid target BEFORE spending Crimson Trail."
}

# --- Hardmode escalation ladder ------------------------------------------------
# The Swordsman only exists in hardmode (subclasses resolve off Main.hardMode), so the
# power fantasy is staged across hardmode milestones, NOT pre-hardmode -> hardmode.
if ($skill -notmatch "TierFinisher" -or
    $skill -notmatch "TierHemorrhage" -or
    $skill -notmatch "TierAnnihilation") {
    throw "Crimson Execution should define its three hardmode escalation tiers."
}

# The gates themselves now live in the shared MechanicTier ladder, so the execution, the
# central Acc* growth and the Eternal's advice can never disagree about the player's progress.
if ($skill -notmatch "MechanicTier\.Current\(\)") {
    throw "Execution tiers should come from the shared MechanicTier ladder, not a private copy."
}

$tierLadder = Get-Content -Raw (Join-Path $contentRoot "Progression\MechanicTier.cs")

if ($tierLadder -notmatch "NPC\.downedPlantBoss" -or
    $tierLadder -notmatch "NPC\.downedMoonlord") {
    throw "The tiers should be gated on hardmode milestones (Plantera, Moon Lord)."
}

# Compare against code only -- the comments legitimately explain the Main.hardMode gate.
$skillCode = ($skill -split "`n" | Where-Object { $_ -notmatch '^\s*//' }) -join "`n"

if ($skillCode -match "Main\.hardMode") {
    throw "Do not gate a tier on Main.hardMode: the Swordsman already requires hardmode to exist."
}

# The radius must GROW with the tier, otherwise the endgame never feels different.
$radii = [regex]::Matches($skill, "Radius(?:Finisher|Hemorrhage|Annihilation) = (\d+)f") |
    ForEach-Object { [int]$_.Groups[1].Value }

if ($radii.Count -ne 3) {
    throw "Each tier should declare its own execution radius."
}

if (-not ($radii[0] -lt $radii[1] -and $radii[1] -lt $radii[2])) {
    throw "Execution radius should grow with each tier (finisher < hemorrhage < annihilation)."
}

# Hemorrhage: the execution must draw its own blood instead of only finishing existing bleeds.
if ($skill -notmatch "tier >= TierHemorrhage" -or
    $skill -notmatch "bleed\.ApplyBleed\(npc\)") {
    throw "From the Hemorrhage tier the execution should apply bleed to the whole zone itself."
}

# --- The guardrail that keeps the mod playable ---------------------------------
# Annihilation instant-kills. If bosses were not exempt, one key press would delete every
# boss fight in the mod, Prototype-01 included.
if ($skill -notmatch "IsBossLike") {
    throw "Annihilation must recognise bosses so it can exempt them."
}

$bossLike = [regex]::Match(
    $skill,
    'static bool IsBossLike\([\s\S]+?\n\s*\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($bossLike -notmatch "npc\.boss" -or
    $bossLike -notmatch "ShouldBeCountedAsBoss") {
    throw "IsBossLike should cover both npc.boss and NPCID.Sets.ShouldBeCountedAsBoss."
}

$wiped = [regex]::Match(
    $skill,
    'bool wiped =[\s\S]+?;',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($wiped -notmatch "!IsBossLike\(npc\)") {
    throw "The instant kill MUST exempt bosses -- otherwise the skill deletes every boss fight."
}

if ($wiped -notmatch "tier >= TierAnnihilation") {
    throw "The instant kill should only exist at the endgame Annihilation tier."
}

# Soul Ascension is the payoff dial on the endgame tier.
if ($skill -notmatch "SoulAscensionPlayer" -or
    $skill -notmatch "AnnihilationThreshold") {
    throw "The annihilation threshold should scale with Soul Ascension."
}

# --- Dedicated on-screen bar ------------------------------------------------
if ($ui -notmatch "class CrimsonTrailUI\s*:\s*ModSystem" -or
    $ui -notmatch "IsActiveSwordsman\(\)" -or
    $ui -notmatch "DrawFloatingResourceBar") {
    throw "CrimsonTrailUI should draw a Swordsman-only resource bar."
}

Write-Host "Crimson Trail source smoke test passed."
