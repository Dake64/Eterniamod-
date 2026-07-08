$ErrorActionPreference = "Stop"

# XP is awarded in EterniaGlobalNPC.OnKill, which runs on the SERVER in
# multiplayer. The server must send the earned XP to the killing player's client
# (via a ModPacket) and the mod must handle that packet client-side; otherwise XP
# never reaches the player who killed the enemy.

$repoRoot = Split-Path -Parent $PSScriptRoot
$mod = Get-Content -Raw (Join-Path $repoRoot "ETERNIA.cs")
$npc = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\EterniaGlobalNPC.cs")

if ($mod -notmatch "public override void HandlePacket\(") {
    throw "ETERNIA mod should override HandlePacket to receive networked messages."
}

if ($mod -notmatch "AddExperience") {
    throw "ETERNIA HandlePacket should handle the AddExperience message."
}

if ($npc -notmatch "NetmodeID\.Server") {
    throw "EterniaGlobalNPC.OnKill should route XP through the server in multiplayer."
}

if ($npc -notmatch "GetPacket\(\)") {
    throw "EterniaGlobalNPC.OnKill should send a ModPacket carrying the earned XP."
}

Write-Host "Experience multiplayer source smoke test passed."
