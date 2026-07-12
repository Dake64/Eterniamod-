$ErrorActionPreference = "Stop"

# The Peleador (Fighter) redesign:
#  - Combo is 20 base / 2.5s base, and grants NOTHING on its own; every effect comes
#    from the Combo passive branch (damage/atk-speed/move per point, bigger cap,
#    longer window, generation, conservation, Frenzy at max).
#  - Its punch does full damage point-blank and much less at reach.
#  - A Remate (Skill key) spends the whole Combo on a burst.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$fighter = Get-Content -Raw (Join-Path $c "Players\FighterPlayer.cs")
$skill = Get-Content -Raw (Join-Path $c "Players\FighterSkillPlayer.cs")
$punch = Get-Content -Raw (Join-Path $c "Projectiles\FighterPunchProjectile.cs")
$stats = Get-Content -Raw (Join-Path $c "Players\EterniaStatsPlayer.cs")
$registry = Get-Content -Raw (Join-Path $c "Passives\PassiveRegistry.cs")

# --- Base values from the spec (20 / 2.5s) -----------------------------------
if ($fighter -notmatch "BaseMaxCombo = 20") {
    throw "FighterPlayer base max Combo should be 20 (was 50)."
}
if ($fighter -notmatch "BaseComboDuration = 150") {
    throw "FighterPlayer base Combo window should be 150 ticks (2.5s)."
}

# --- Pre-hardmode: the Combo is a Warrior-wide COUNTER (builds without the
#     subclass), and only a promoted Peleador turns it into stats. -------------
if ($fighter -notmatch "public bool IsActiveWarrior\(") {
    throw "FighterPlayer should expose IsActiveWarrior so the Combo counter works pre-hardmode."
}

# --- Combo is passive-driven: every Combo-branch node is read in FighterPlayer -
foreach ($node in @(
    "Combo Instinct", "Limit Breaker", "Flow State", "Perfect Rhythm",
    "Adrenaline Rush", "Unbroken Chain", "Rapid Blows", "Thousand Cuts",
    "Perpetual Motion")) {
    if ($fighter -notmatch ('HasActivePassive\("' + [regex]::Escape($node) + '"\)')) {
        throw "FighterPlayer should read the Combo passive '$node' (its effect is passive-driven)."
    }
}

# --- Those Combo nodes must NOT still give flat stats in EterniaStatsPlayer ---
foreach ($node in @("Combo Instinct", "Flow State", "Perfect Rhythm",
                    "Rapid Blows", "Unbroken Chain", "Thousand Cuts")) {
    if ($stats -match ('HasActivePassive\(soulPlayer\.ActiveSoul, "' + [regex]::Escape($node) + '"\)')) {
        throw "'$node' should modify the Combo in FighterPlayer, not give a flat stat in EterniaStatsPlayer."
    }
}

# --- Conservation on hurt + generation on crit/point-blank --------------------
if ($fighter -notmatch "OnHurt" -or $fighter -notmatch "ComboGainForHit") {
    throw "FighterPlayer should lose Combo on hurt (unless conserved) and expose ComboGainForHit."
}

# --- Frenzy at max Combo (keystone) ------------------------------------------
if ($fighter -notmatch "AtMaxCombo" -or $registry -notmatch "FRENZY") {
    throw "Max Combo should grant Frenzy (Perpetual Motion keystone)."
}

# --- The Remate finisher (Skill key spends Combo) ----------------------------
if ($skill -notmatch "class FighterSkillPlayer\s*:\s*ModPlayer" -or
    $skill -notmatch "EterniaKeybinds\.SkillKey" -or
    $skill -notmatch "fighter\.Combo = 0" -or
    $skill -notmatch "SimpleStrikeNPC") {
    throw "FighterSkillPlayer should spend the Combo on a Remate burst via the Skill key."
}

# --- Punch: full damage point-blank, much less at reach ----------------------
if ($punch -notmatch "distance <= 60f" -or
    $punch -notmatch "multiplier = 1f;" -or
    $punch -notmatch "multiplier = 0.5f;") {
    throw "FighterPunchProjectile should do full damage point-blank and ~half at the end of reach."
}

Write-Host "Fighter combo source smoke test passed."
