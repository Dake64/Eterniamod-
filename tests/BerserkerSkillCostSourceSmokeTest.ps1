$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$skillPath = Join-Path $repoRoot "Content\Players\BerserkerSkillPlayer.cs"
$content = Get-Content -Raw $skillPath

if ($content -notmatch "private const int SkillRageCost\s*=\s*100") {
    throw "BerserkerSkillPlayer should define SkillRageCost = 100."
}

if ($content -notmatch "ragePlayer\.Rage\s*<\s*SkillRageCost") {
    throw "Berserker skill should require SkillRageCost instead of a hard-coded rage check."
}

if ($content -match "Rage\s*-=\s*0") {
    throw "Berserker skill should not subtract zero rage."
}

if ($content -notmatch "ragePlayer\.Rage\s*-=\s*SkillRageCost") {
    throw "Berserker skill should consume SkillRageCost rage when used."
}

if ($content -notmatch "if \(ragePlayer\.Rage < 0\)") {
    throw "Berserker skill should clamp rage after spending."
}

Write-Host "Berserker skill cost source smoke test passed."
