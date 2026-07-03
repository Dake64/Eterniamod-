$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiPath = Join-Path $repoRoot "Content\UI\PassiveUI.cs"
$content = Get-Content -Raw $uiPath

if ($content -notmatch "PassiveRegistry\.GetPassivesForSoul\(\s*soulPlayer\.ActiveSoul\)") {
    throw "PassiveUI should resolve the active tree through PassiveRegistry.GetPassivesForSoul(soulPlayer.ActiveSoul)."
}

if ($content -match "currentTree\s*=\s*PassiveRegistry\.WarriorPassives" -or
    $content -match "currentTree\s*=\s*PassiveRegistry\.MagePassives" -or
    $content -match "currentTree\s*=\s*PassiveRegistry\.RangerPassives" -or
    $content -match "currentTree\s*=\s*PassiveRegistry\.SummonerPassives") {
    throw "PassiveUI should not duplicate Soul-to-passive-tree selection."
}

Write-Host "PassiveUI source smoke test passed."
