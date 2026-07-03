$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$statsPath = Join-Path $repoRoot "Content\Players\EterniaStatsPlayer.cs"
$registryPath = Join-Path $repoRoot "Content\Passives\PassiveRegistry.cs"

$stats = Get-Content -Raw $statsPath
$registry = Get-Content -Raw $registryPath

if ($registry -notmatch "IsPassiveAllowedForSoul\(\s*SoulId soul,\s*string passiveName\)") {
    throw "PassiveRegistry should expose IsPassiveAllowedForSoul(SoulId soul, string passiveName) for passive effect checks."
}

if ($stats -notmatch "HasActivePassive\(\s*SoulId activeSoul,\s*string passiveName\)") {
    throw "EterniaStatsPlayer should expose HasActivePassive(SoulId activeSoul, string passiveName)."
}

if ($stats -notmatch "PassiveRegistry\.IsPassiveAllowedForSoul\(activeSoul, passiveName\)") {
    throw "HasActivePassive should verify the passive belongs to the active Soul tree."
}

if ($stats -match 'if\s*\(\s*HasPassive\("') {
    throw "Passive effects should use HasActivePassive(...) instead of HasPassive(...) directly."
}

Write-Host "Passive effect scope source smoke test passed."
