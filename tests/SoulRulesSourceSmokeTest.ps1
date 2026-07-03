$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$soulRulesPath = Join-Path $repoRoot "Content\Souls\SoulRules.cs"
$content = Get-Content -Raw $soulRulesPath

if ($content -notmatch "CountsAsClass") {
    throw "SoulRules must use DamageClass.CountsAsClass for inherited/custom damage classes."
}

if ($content -notmatch "IsDamageClass") {
    throw "SoulRules should route damage-class compatibility through a named helper."
}

Write-Host "SoulRules source smoke test passed."
