$ErrorActionPreference = "Stop"

# The Cursed Mage runs in two phases:
#  Pre-Hardmode (any Mage): only Cursed Energy, with a FIXED regen. No Corruption/Burst.
#  Hardmode (promoted Cursed Mage): Corruption (0-200) drives energy regen and boosts
#    magic damage/speed, at the cost of defense/max-life/damage-taken and a Collapse at
#    the top. Cursed Burst spends ALL Corruption for a scaling explosion + resets it +
#    refunds energy.

$repoRoot = Split-Path -Parent $PSScriptRoot
$player = Get-Content -Raw (Join-Path $repoRoot "Content\Players\CursedMagePlayer.cs")

# --- Two gates: Mage-wide energy vs promoted Cursed Mage ----------------------
if ($player -notmatch "public bool IsActiveMage\(" -or
    $player -notmatch "public bool IsActiveCursedMage\(") {
    throw "CursedMagePlayer should expose IsActiveMage (energy) and IsActiveCursedMage (corruption/burst)."
}

# Corruption is gated to the promoted Cursed Mage (pre-hardmode it is 0).
if ($player -notmatch "TotalCorruption\s*=>[\s\S]*IsActiveCursedMage\(\)") {
    throw "TotalCorruption must be 0 unless the player is a promoted Cursed Mage."
}

# Pre-hardmode returns early before any corruption/burst handling.
if ($player -notmatch "if \(!cursedMage\)[\s\S]*TemporaryCorruption = 0;[\s\S]*return;") {
    throw "Pre-hardmode should skip all corruption/burst handling."
}

# --- Corruption reward + risk ------------------------------------------------
$effects = [regex]::Match(
    $player,
    "private void HandleCorruptionEffects\([\s\S]+?\n {8}\}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value
if ($effects -notmatch "GetDamage\(DamageClass\.Magic\)" -or
    $effects -notmatch "GetAttackSpeed\(DamageClass\.Magic\)") {
    throw "Corruption should reward magic damage and cast speed."
}
if ($effects -notmatch "statDefense" -or
    $effects -notmatch "statLifeMax2" -or
    $effects -notmatch "endurance") {
    throw "Corruption should cost defense, max life and damage taken (risk)."
}
if ($effects -notmatch "Player\.Hurt\(") {
    throw "Extreme corruption should cause a Cursed Collapse (damage over time)."
}

# --- Cursed Burst: spend ALL corruption for a scaling explosion --------------
$burst = [regex]::Match(
    $player,
    "public void ActivateBurst\([\s\S]+?\n {8}\}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value
if ($burst -notmatch "corruption \* 4" -and $burst -notmatch "corruption \*") {
    throw "The Cursed Burst explosion should scale with the corruption spent."
}
if ($burst -notmatch "SimpleStrikeNPC") {
    throw "The Cursed Burst should deal AoE damage."
}
if ($burst -notmatch "TemporaryCorruption = 0" -or $burst -notmatch "GainEnergy\(") {
    throw "The Cursed Burst should reset corruption and refund energy."
}

# --- The Curse passive tree shapes the mechanic ------------------------------
# Nodes keep their flat magic stat in EterniaStatsPlayer, and ALSO reshape the
# corruption mechanic here (read via HasCurse).
if ($player -notmatch "public bool HasCurse\(") {
    throw "CursedMagePlayer should expose HasCurse to let the Curse tree shape the mechanic."
}
foreach ($node in @("Dark Ritual",     # +energy regen
                    "Cursed Blood",     # more damage per corruption
                    "Doom Bringer",     # more cast speed per corruption
                    "Withering Curse",  # less defense loss
                    "Soul Rot",         # less max-life loss
                    "Blight",           # bigger burst
                    "Malediction")) {   # more refund + later collapse
    if ($player -notmatch ('HasCurse\("' + [regex]::Escape($node) + '"\)')) {
        throw "The Curse tree node '$node' should shape the corruption mechanic in CursedMagePlayer."
    }
}

Write-Host "Cursed Mage mechanic source smoke test passed."
