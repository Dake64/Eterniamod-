$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$systemPath = Join-Path $repoRoot "Content\Systems\EternalNPCSystem.cs"
$npcPath = Join-Path $repoRoot "Content\NPCs\EternalNPC.cs"
$inventoryPath = Join-Path $repoRoot "Content\Souls\SoulInventory.cs"
$content = Get-Content -Raw $systemPath
$npc = Get-Content -Raw $npcPath
$inventory = Get-Content -Raw $inventoryPath

if ($content -notmatch "Main\.netMode == NetmodeID\.MultiplayerClient") {
    throw "EternalNPCSystem should not spawn onboarding NPCs from multiplayer clients."
}

if ($content -notmatch "TryFindPlayerNeedingSoul") {
    throw "EternalNPCSystem should search for an active player needing a Soul."
}

if ($content -notmatch "Main\.maxPlayers") {
    throw "EternalNPCSystem should support server-side multiplayer player lookup."
}

if ($content -notmatch "SoulInventory\.HasAnySoulItem") {
    throw "EternalNPCSystem should skip onboarding spawn for players who already own a Soul item."
}

if ($npc -notmatch "bool ownsSoulItem =\s*SoulInventory\.HasAnySoulItem\(player\)") {
    throw "EternalNPC chat should use real Soul item ownership, not only the currently active accessory Soul."
}

if ($npc -notmatch "SetChatButtons[\s\S]*SoulInventory\.HasAnySoulItem\(Main\.LocalPlayer\)") {
    throw "EternalNPC should not offer another Empty Soul button when the player already owns any Soul item."
}

if ($inventory -notmatch "EmptySoul" -or
    $inventory -notmatch "WarriorSoul" -or
    $inventory -notmatch "MageSoul" -or
    $inventory -notmatch "RangerSoul" -or
    $inventory -notmatch "SummonerSoul") {
    throw "SoulInventory ownership guard should include Empty, Warrior, Mage, Ranger, and Summoner Souls."
}

Write-Host "EternalNPCSystem source smoke test passed."
