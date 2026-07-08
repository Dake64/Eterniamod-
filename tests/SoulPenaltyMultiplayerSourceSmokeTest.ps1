$ErrorActionPreference = "Stop"

# The Soul penalty (stat debuffs + KillMe for a wrong-class weapon) is applied in
# EterniaPlayer.PostUpdateEquips, which runs for EVERY player on each client. In
# multiplayer it must only affect the local player; otherwise a client could try
# to kill/penalize a remote player. Guard the penalty path with whoAmI/myPlayer.

$repoRoot = Split-Path -Parent $PSScriptRoot
$content = Get-Content -Raw (Join-Path $repoRoot "Content\Players\EterniaPlayer.cs")

if ($content -notmatch "(?ms)public override void PostUpdateEquips\(\)\s*\{[^}]*whoAmI\s*!=\s*Main\.myPlayer") {
    throw "EterniaPlayer.PostUpdateEquips must guard the Soul penalty to the local player (whoAmI == Main.myPlayer)."
}

Write-Host "Soul penalty multiplayer source smoke test passed."
