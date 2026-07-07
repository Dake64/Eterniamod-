$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$inventoryPath = Join-Path $repoRoot "Content\Souls\SoulInventory.cs"
$npcPath = Join-Path $repoRoot "Content\NPCs\EternalNPC.cs"
$systemPath = Join-Path $repoRoot "Content\Systems\EternalNPCSystem.cs"
$playerPath = Join-Path $repoRoot "Content\Players\EterniaPlayer.cs"

if (!(Test-Path $inventoryPath)) {
    throw "Missing shared Soul inventory helper: $inventoryPath"
}

$inventory = Get-Content -Raw $inventoryPath
$npc = Get-Content -Raw $npcPath
$system = Get-Content -Raw $systemPath
$player = Get-Content -Raw $playerPath

if ($inventory -notmatch "public static bool HasAnySoulItem\(Player player\)") {
    throw "SoulInventory should expose HasAnySoulItem(Player player)."
}

if ($inventory -notmatch "public static bool IsSoulItemType\(int type\)") {
    throw "SoulInventory should expose IsSoulItemType(int type)."
}

if ($inventory -notmatch "public static bool HasAnyClassSoulItem\(Player player\)") {
    throw "SoulInventory should expose HasAnyClassSoulItem(Player player)."
}

if ($inventory -notmatch "public static bool IsClassSoulItemType\(int type\)") {
    throw "SoulInventory should expose IsClassSoulItemType(int type)."
}

if ($npc -notmatch "SoulInventory\.HasAnySoulItem\(player\)") {
    throw "EternalNPC should use SoulInventory.HasAnySoulItem(player)."
}

if ($system -notmatch "SoulInventory\.HasAnySoulItem\((player|candidate)\)") {
    throw "EternalNPCSystem should use SoulInventory.HasAnySoulItem(...) for player onboarding guards."
}

if ($npc -match "private static bool HasAnySoulItem" -or
    $system -match "private static bool HasAnySoulItem" -or
    $npc -match "private static bool HasSoulItem" -or
    $system -match "private static bool HasSoulItem" -or
    $player -match "private static bool HasClassSoulItem") {
    throw "Soul item inventory scans should not be duplicated in NPC onboarding classes."
}

Write-Host "SoulInventory source smoke test passed."
