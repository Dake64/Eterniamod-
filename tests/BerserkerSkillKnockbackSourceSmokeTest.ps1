$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$skillPath = Join-Path $repoRoot "Content\Players\BerserkerSkillPlayer.cs"
$content = Get-Content -Raw $skillPath

if ($content -notmatch "if \(knockbackDir\.LengthSquared\(\) <= 0f\)") {
    throw "BerserkerSkillPlayer should guard against zero-length knockback vectors before Normalize()."
}

if ($content -notmatch "continue;\s*\}\s*knockbackDir\.Normalize\(\)") {
    throw "BerserkerSkillPlayer should skip zero-length vectors before applying Normalize()."
}

Write-Host "Berserker skill knockback source smoke test passed."
