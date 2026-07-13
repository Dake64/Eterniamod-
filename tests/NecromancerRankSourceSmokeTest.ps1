$ErrorActionPreference = "Stop"

# Necromancer Phase 3: the Grimoire evolves with progression. Its RANK (I..IV) gates
# which creatures can be raised, and how many ACTIVE PAGES (distinct undead types) can
# be carried at once (3/5/7/10). Bosses grant lesser echoes via Boss Souls.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$rank = Get-Content -Raw (Join-Path $c "Necromancy\GrimoireRank.cs")
$registry = Get-Content -Raw (Join-Path $c "Necromancy\GrimoireRegistry.cs")
$collection = Get-Content -Raw (Join-Path $c "Players\NecromancerCollectionPlayer.cs")
$grimoire = Get-Content -Raw (Join-Path $c "Items\Weapons\Summoner\GrimoireOfDeath.cs")

# --- Rank tracks progression, pages scale 3/5/7/10 ---------------------------
if ($rank -notmatch "NPC\.downedMoonlord" -or
    $rank -notmatch "NPC\.downedPlantBoss" -or
    $rank -notmatch "Main\.hardMode") {
    throw "GrimoireRank should climb with Wall of Flesh / Plantera / Moon Lord."
}
foreach ($pages in @(3, 5, 7, 10)) {
    if ($rank -notmatch "$pages") {
        throw "MaxActivePages should scale to $pages."
    }
}

# --- Creatures carry a required rank; boss echoes exist -----------------------
if ($registry -notmatch "RequiredRank") {
    throw "GrimoireEntry should gate creatures behind a required rank."
}
foreach ($id in @("GuardianSlime", "EyeSpirit")) {
    if ($registry -notmatch ('Id = "' + $id + '"') -or $registry -notmatch 'Category = "Boss"') {
        throw "The registry should define boss echoes (e.g. $id) in the Boss category."
    }
}

# --- Active pages loadout with eviction --------------------------------------
if ($collection -notmatch "List<string> ActivePages") {
    throw "NecromancerCollectionPlayer should track the active-page loadout."
}
if ($collection -notmatch "public void EnsureActive\(" -or
    $collection -notmatch "MaxActivePages\(\)" -or
    $collection -notmatch "DespawnCreature\(") {
    throw "Raising a creature should occupy an active page, evicting (and crumbling) the oldest when full."
}
if ($collection -notmatch "EligibleEntries" -or
    $collection -notmatch "RequiredRank <= rank") {
    throw "Only creatures within the current Grimoire rank should be raisable."
}
if ($collection -notmatch "NecroActivePages") {
    throw "The active-page loadout should persist per character."
}

# --- The Grimoire uses eligible entries + activates on raise ------------------
if ($grimoire -notmatch "EligibleEntries\(\)" -or $grimoire -notmatch "EnsureActive\(") {
    throw "GrimoireOfDeath should raise only rank-eligible creatures and activate them."
}

Write-Host "Necromancer rank source smoke test passed."
