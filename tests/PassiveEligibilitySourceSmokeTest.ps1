$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$registryPath = Join-Path $repoRoot "Content\Passives\PassiveRegistry.cs"
$servicePath = Join-Path $repoRoot "Content\Progression\ProgressionService.cs"

$registry = Get-Content -Raw $registryPath
$service = Get-Content -Raw $servicePath

if ($registry -notmatch "GetPassivesForSoul\(SoulId soul\)") {
    throw "PassiveRegistry should expose GetPassivesForSoul(SoulId soul)."
}

if ($registry -notmatch "IsPassiveAllowedForSoul\(\s*SoulId soul,\s*PassiveNode passive\)") {
    throw "PassiveRegistry should expose IsPassiveAllowedForSoul(SoulId soul, PassiveNode passive)."
}

if ($registry -notmatch "SoulId\.Warrior => WarriorPassives" -or
    $registry -notmatch "SoulId\.Mage => MagePassives" -or
    $registry -notmatch "SoulId\.Ranger => RangerPassives" -or
    $registry -notmatch "SoulId\.Summoner => SummonerPassives") {
    throw "PassiveRegistry should map all four base class Souls to their passive trees."
}

if ($service -notmatch "PassiveRegistry\.IsPassiveAllowedForSoul\(soul\.ActiveSoul, passive\)") {
    throw "ProgressionService.TryUnlockPassive should reject passives outside the active Soul tree."
}

Write-Host "Passive eligibility source smoke test passed."
