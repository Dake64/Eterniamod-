$ErrorActionPreference = "Stop"

# The live, in-game-resolving localization file is the ROOT en-US.hjson
# (tModLoader-managed; the mod resolves keys under Mods.Eternia.*). The
# Localization/en-US_Mods.ETERNIA.hjson file is dead (wrong-case Mods.ETERNIA.*)
# and is retired. This test guards the LIVE file against the localization
# regressions we hit before:
#   - self-referential Description/Tooltip values (value equal to its own key),
#   - auto-generated double-spaced labels (e.g. "Toggle Soul U I"),
#   - the no-Soul penalty debuff losing its intended English name.

$repoRoot = Split-Path -Parent $PSScriptRoot
$livePath = Join-Path $repoRoot "en-US.hjson"

if (!(Test-Path $livePath)) {
    throw "Live localization file missing: $livePath"
}

$content = Get-Content -Raw $livePath

# 1. No self-referential Description/Tooltip (value equals a Mods.* key path).
if ($content -match "(?m)(Description|Tooltip)\s*:\s*Mods\.[A-Za-z0-9_.]+\s*$") {
    throw "Live localization contains a self-referential Description/Tooltip pointing at its own key."
}

# 2. No auto-generated double-spaced labels.
if ($content -match "  U I|Toggle  |Class  Skill|Change  Note|Elemental  Ultimate|Cursed  Burst") {
    throw "Live localization contains auto-generated double-spaced labels."
}

# 3. The no-Soul penalty debuff keeps its intended English name.
if ($content -notmatch "(?ms)\bSoulLessDebuff:\s*\{[^}]*DisplayName:\s*Lost Soul") {
    throw "SoulLessDebuff should display as 'Lost Soul'."
}

Write-Host "Localization integrity source smoke test passed."
