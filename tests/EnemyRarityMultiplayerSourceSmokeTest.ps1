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

Write-Host "Enemy rarity multiplayer source smoke test passed."
