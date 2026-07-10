$ErrorActionPreference = "Stop"

# The five subclasses that used to be flat "+stat" only now each carry a real
# signature mechanic (a built resource + an active/continuous payoff), in the
# same spirit as the Swordsman's Crimson Trail. This test pins those mechanics
# to their source so they can't silently regress back to plain stat blocks.

$repoRoot = Split-Path -Parent $PSScriptRoot
$players = Join-Path $repoRoot "Content\Players"

function Load($name) {
    $path = Join-Path $players $name
    if (-not (Test-Path $path)) {
        throw "Missing subclass mechanic file: $name"
    }
    return Get-Content -Raw $path
}

# --- Infinity Mage: casting builds Overflow -> free-cast Overload window -------
$inf = Load "InfinityMagePlayer.cs"
if ($inf -notmatch "IsActiveInfinityMage" -or
    $inf -notmatch "Overflow" -or
    $inf -notmatch "OverloadTimer" -or
    $inf -notmatch "manaCost" -or
    $inf -notmatch "SkillKey") {
    throw "InfinityMagePlayer should build Overflow from casting and spend it on a free-cast Overload window."
}

# --- Arcane Bard: magic hits build a decaying Crescendo momentum ---------------
$bard = Load "ArcaneBardPlayer.cs"
if ($bard -notmatch "IsActiveArcaneBard" -or
    $bard -notmatch "Crescendo" -or
    $bard -notmatch "DamageClass\.Magic" -or
    $bard -notmatch "moveSpeed") {
    throw "ArcaneBardPlayer should build and decay Crescendo momentum that scales magic power and speed."
}

# --- Beast Tamer: minion hits build Ferocity -> Primal Roar frenzy -------------
$beast = Load "BeastTamerPlayer.cs"
if ($beast -notmatch "IsActiveBeastTamer" -or
    $beast -notmatch "Ferocity" -or
    $beast -notmatch "Frenzy" -or
    $beast -notmatch "DamageClass\.Summon" -or
    $beast -notmatch "SkillKey") {
    throw "BeastTamerPlayer should build Ferocity from minion hits and spend it on a Primal Roar frenzy."
}

# --- Advanced Summoner: roster-fill synergy + Overclock (+cap) window ----------
$adv = Load "AdvancedSummonerPlayer.cs"
if ($adv -notmatch "IsActiveAdvancedSummoner" -or
    $adv -notmatch "Command" -or
    $adv -notmatch "Overclock" -or
    $adv -notmatch "maxMinions" -or
    $adv -notmatch "SkillKey") {
    throw "AdvancedSummonerPlayer should scale with roster fill and spend Command on an Overclock (+minion cap) window."
}

# --- Tech Summoner: Power Core battery -> Overdrive Protocol (dmg + shield) -----
$tech = Load "TechSummonerPlayer.cs"
if ($tech -notmatch "IsActiveTechSummoner" -or
    $tech -notmatch "PowerCore" -or
    $tech -notmatch "Overdrive" -or
    $tech -notmatch "statDefense" -or
    $tech -notmatch "SkillKey") {
    throw "TechSummonerPlayer should charge a Power Core battery and spend it on an Overdrive Protocol (damage + shield) window."
}

Write-Host "Subclass mechanics source smoke test passed."
