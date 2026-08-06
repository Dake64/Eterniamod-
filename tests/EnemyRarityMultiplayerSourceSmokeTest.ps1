$ErrorActionPreference = "Stop"

# Enemy rarity is random. In multiplayer it MUST be rolled once on the server and
# synced to clients (via SendExtraAI/ReceiveExtraAI); otherwise each client rolls
# its own rarity and the enemies desync (different badge/size per client).

$repoRoot = Split-Path -Parent $PSScriptRoot
$content = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\EterniaGlobalNPC.cs")

if ($content -notmatch "NetmodeID\.MultiplayerClient") {
    throw "EterniaGlobalNPC must not roll rarity on multiplayer clients (server rolls, clients receive)."
}

if ($content -notmatch "public override void SendExtraAI\(") {
    throw "EterniaGlobalNPC must implement SendExtraAI to sync rarity/level to clients."
}

if ($content -notmatch "public override void ReceiveExtraAI\(") {
    throw "EterniaGlobalNPC must implement ReceiveExtraAI to receive synced rarity/level."
}

# The audit found damage/defence scaling worked in singleplayer but was inert online: the
# multipliers were applied on the server but never sent, and the mod skips SetDefaults on
# clients. Both must be transmitted AND re-applied to npc.damage/npc.defense on the client.
$send = [regex]::Match($content,
    'public override void SendExtraAI\([\s\S]+?\n\s{8}\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value
$receive = [regex]::Match($content,
    'public override void ReceiveExtraAI\([\s\S]+?\n\s{8}\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($send -notmatch "damageMultiplier" -or $send -notmatch "defenseMultiplier") {
    throw "SendExtraAI must transmit damage/defence multipliers, or elites are base-stat online."
}
if ($receive -notmatch "npc\.damage =" -or $receive -notmatch "npc\.defense =") {
    throw "ReceiveExtraAI must reapply npc.damage/npc.defense, or the rarity scaling is inert online."
}

Write-Host "Enemy rarity multiplayer source smoke test passed."
