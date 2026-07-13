$ErrorActionPreference = "Stop"

# Necromancer Phase 2: the Grimoire of Death + Soul collection. You dominate a creature
# by killing enough of its source enemies, obtaining its Soul (a chanced drop past the
# threshold), and registering that Soul in the Grimoire. Unlocked creatures are then
# raised from the Grimoire.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$registry = Get-Content -Raw (Join-Path $c "Necromancy\GrimoireRegistry.cs")
$collection = Get-Content -Raw (Join-Path $c "Players\NecromancerCollectionPlayer.cs")
$condition = Get-Content -Raw (Join-Path $c "Necromancy\SoulDropCondition.cs")
$killNpc = Get-Content -Raw (Join-Path $c "Globals\NecromancerKillGlobalNPC.cs")
$soul = Get-Content -Raw (Join-Path $c "Items\Souls\EnemySoul.cs")
$grimoire = Get-Content -Raw (Join-Path $c "Items\Weapons\Summoner\GrimoireOfDeath.cs")

# --- Registry: creatures with a source, a threshold, a soul and a minion ------
foreach ($id in @("Skeleton", "Zombie", "DemonEye")) {
    if ($registry -notmatch ('Id = "' + $id + '"')) {
        throw "GrimoireRegistry should define the '$id' creature."
    }
}
if ($registry -notmatch "KillThreshold" -or
    $registry -notmatch "SoulType" -or
    $registry -notmatch "MinionType" -or
    $registry -notmatch "DefaultUnlocked = true") {
    throw "GrimoireEntry should carry a kill threshold, soul, minion, and a default-unlocked starter."
}

# --- Collection: kills + unlocked, persistent --------------------------------
if ($collection -notmatch "Dictionary<int, int> Kills" -or
    $collection -notmatch "List<string> Unlocked") {
    throw "NecromancerCollectionPlayer should track kills and unlocked creatures."
}
if ($collection -notmatch "SaveData" -or $collection -notmatch "LoadData") {
    throw "The Grimoire collection should persist per character."
}
if ($collection -notmatch "public bool Unlock\(" -or
    $collection -notmatch "KillsForEntry") {
    throw "NecromancerCollectionPlayer should expose Unlock + KillsForEntry."
}

# --- Soul drop only after the kill threshold, and only if not yet dominated ---
if ($condition -notmatch "IItemDropRuleCondition" -or
    $condition -notmatch "IsUnlocked" -or
    $condition -notmatch "KillsForEntry\(entry\) >= entry\.KillThreshold") {
    throw "SoulDropCondition should gate the soul on kills >= threshold and not-yet-unlocked."
}
if ($killNpc -notmatch "OnKill" -or $killNpc -notmatch "AddKill" -or
    $killNpc -notmatch "ItemDropRule\.ByCondition") {
    throw "NecromancerKillGlobalNPC should count kills and add the conditional soul drop."
}

# --- Registering a Soul unlocks the creature and consumes the soul -----------
if ($soul -notmatch "IsActiveNecromancer\(\)" -or
    $soul -notmatch "\.Unlock\(CreatureId\)" -or
    $soul -notmatch "consumable = true") {
    throw "EnemySoul should let a Necromancer unlock its creature and be consumed."
}

# --- The Grimoire raises the selected creature (left) / cycles (right) --------
if ($grimoire -notmatch "AltFunctionUse" -or
    $grimoire -notmatch "SelectedIndex" -or
    $grimoire -notmatch "MinionType\(\)" -or
    $grimoire -notmatch "ReservedLifeFraction < 0\.9f") {
    throw "GrimoireOfDeath should raise the selected creature (life-gated) and cycle selection on right click."
}

Write-Host "Necromancer grimoire source smoke test passed."
