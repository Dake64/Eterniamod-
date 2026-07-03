$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$swordsmanPath = Join-Path $repoRoot "Content\Players\SwordsmanPlayer.cs"
$bleedPath = Join-Path $repoRoot "Content\NPCs\BleedGlobalNPC.cs"
$bladePath = Join-Path $repoRoot "Content\Items\Weapons\Promotion\BloodletterBlade.cs"

$swordsman = Get-Content -Raw $swordsmanPath
$bleed = Get-Content -Raw $bleedPath
$blade = Get-Content -Raw $bladePath

if ($swordsman -notmatch 'public bool IsActiveSwordsman\(' -or
    $swordsman -notmatch 'ActiveSoul == SoulId\.Warrior' -or
    $swordsman -notmatch 'if \(!IsActiveSwordsman\(\)\)' -or
    $swordsman -notmatch 'return;[\s\S]*BleedGlobalNPC bleedNPC') {
    throw "SwordsmanPlayer should only apply bleed stacks while Swordsman is backed by an active Warrior Soul."
}

if ($swordsman -notmatch 'const int MaxBleedStacks\s*=\s*5' -or
    $swordsman -notmatch 'const int BleedDurationTicks\s*=\s*300') {
    throw "Swordsman bleed stack cap and duration should be named constants."
}

if ($bleed -notmatch 'TryGetBleedOwner' -or
    $bleed -notmatch 'npc\.lastInteraction\s*<\s*0' -or
    $bleed -notmatch 'npc\.lastInteraction\s*>=\s*Main\.maxPlayers') {
    throw "BleedGlobalNPC should validate npc.lastInteraction before indexing Main.player."
}

if ($bleed -notmatch 'GetModPlayer<SwordsmanPlayer>' -or
    $bleed -notmatch 'IsActiveSwordsman\(\)') {
    throw "BleedGlobalNPC should use SwordsmanPlayer.IsActiveSwordsman() so bleed damage requires the matching active Warrior Soul."
}

if ($bleed -match 'GetModPlayer<SubclassPlayer>') {
    throw "BleedGlobalNPC should not read SubclassPlayer.CurrentSubclass directly; that bypasses active Soul gating."
}

if ($blade -notmatch 'ModifyTooltips' -or
    $blade -notmatch 'bleed' -or
    $blade -notmatch 'Swordsman') {
    throw "BloodletterBlade should explain its bleed identity and Swordsman requirement."
}

Write-Host "Swordsman bleed source smoke test passed."
