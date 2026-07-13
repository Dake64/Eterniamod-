$ErrorActionPreference = "Stop"

# The Necromancer redesign (user spec): NO minion slots. Each undead RESERVES a slice of
# max life and DRAINS mana; run dry and the weakest crumble first. This pins the core.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$player = Get-Content -Raw (Join-Path $c "Players\NecromancerPlayer.cs")
$base = Get-Content -Raw (Join-Path $c "Projectiles\Necromancer\BaseNecroMinion.cs")

# --- No slots; reserved life instead -----------------------------------------
if ($player -match "MaxNecroSlots" -or $player -match "UsedNecroSlots" -or $base -match "SlotCost") {
    throw "The Necromancer should no longer use minion slots."
}

# Reserved life lowers effective max life while undead live.
if ($player -notmatch "ReservedLifeFraction" -or
    $player -notmatch "statLifeMax2 -=") {
    throw "Active undead should reserve max life (lower statLifeMax2)."
}
if ($base -notmatch "ReservePercent") {
    throw "BaseNecroMinion should expose ReservePercent."
}

# --- Mana drain crumbles the weakest first -----------------------------------
if ($player -notmatch "ManaDrainPerSecond" -or $player -notmatch "statMana -=") {
    throw "Undead should drain mana each second."
}
$despawn = [regex]::Match(
    $player,
    "private void DespawnWeakest\([\s\S]+?\n {8}\}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value
if ($despawn -notmatch "ReservePercent < lowest" -or $despawn -notmatch "\.Kill\(\)") {
    throw "Running out of mana should crumble the least-important (weakest) undead first."
}

# The base minion must NOT self-destruct on empty mana (the player manages despawns).
if ($base -match "statMana <= 0[\s\S]*Projectile\.Kill\(\)") {
    throw "BaseNecroMinion should fade, not self-kill, on empty mana (player manages despawns)."
}

Write-Host "Necromancer reserved-life source smoke test passed."
