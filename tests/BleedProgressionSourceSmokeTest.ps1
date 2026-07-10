$ErrorActionPreference = "Stop"

# Progression audit fixes for the Swordsman's Bleed branch:
#  P1 bleed damage AND duration used to flatline at 20 Bleed affinity -- reached
#     after only ~5 of 20 nodes, so 74% of the branch's affinity did nothing for
#     bleed. Both now scale (soft cap 20, hard cap 90 > the ~86 a full branch grants).
#  P2 the minor "path" nodes cost up to 2 for 1 affinity (a dead zone worth ~40x
#     less per point than a notable). They now cost 1 and grant 2 affinity.
#  P3 Exsanguinate (cost 4) gave the same +15% as Execution (cost 2). It now gives more.
#  P4 a full branch grants ~86 affinity, so the mastery cap must not clip it.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$bleedNpc = Get-Content -Raw (Join-Path $c "NPCs\BleedGlobalNPC.cs")
$warriorBleed = Get-Content -Raw (Join-Path $c "Players\WarriorBleedPlayer.cs")
$registry = Get-Content -Raw (Join-Path $c "Passives\PassiveRegistry.cs")
$stats = Get-Content -Raw (Join-Path $c "Players\EterniaStatsPlayer.cs")

# --- P1: bleed DAMAGE keeps scaling past the old affinity-20 flatline -----------
if ($bleedNpc -notmatch "SoftCapAffinity" -or
    $bleedNpc -notmatch "HardCapAffinity" -or
    $bleedNpc -notmatch "GetBleedDamage\(") {
    throw "BleedGlobalNPC should scale bleed damage with a soft/hard affinity cap, not flatline."
}

if ($bleedNpc -match "System\.Math\.Min\(stats\.BleedAffinity, 20\)") {
    throw "BleedGlobalNPC must not clamp bleed damage at 20 Bleed affinity (the old flatline)."
}

# --- P1: bleed DURATION keeps scaling too ---------------------------------------
if ($warriorBleed -notmatch "BleedSoftCapAffinity" -or
    $warriorBleed -notmatch "BleedHardCapAffinity") {
    throw "WarriorBleedPlayer should scale bleed duration with a soft/hard affinity cap."
}

if ($warriorBleed -match "Math\.Min\(affinity, 20\) \* 6") {
    throw "WarriorBleedPlayer must not clamp bleed duration at 20 Bleed affinity (the old flatline)."
}

# --- P2: minor path nodes are cheap and actually grant affinity ------------------
if ($registry -match "int cost = step % 3 == 2 \? 2 : 1") {
    throw "Minor path nodes should not cost 2 points; that made the branch's back half a dead zone."
}

if ($registry -notmatch "1,\s*affinity,\s*2,\s*previous") {
    throw "Minor path nodes should cost 1 and grant 2 affinity."
}

# --- P3: Exsanguinate (cost 4) must beat Execution (cost 2) ----------------------
if ($warriorBleed -notmatch "SourceDamage \+= 0\.25f") {
    throw "Exsanguinate should give more than Execution's +15% (it costs twice as much)."
}

# --- P4: the mastery cap must not clip a fully invested branch (~86 affinity) ----
if ($stats -notmatch "System\.Math\.Min\(affinity, 100\)") {
    throw "AffinityCap should be high enough (100) that a fully invested branch wastes no points."
}

Write-Host "Bleed progression source smoke test passed."
