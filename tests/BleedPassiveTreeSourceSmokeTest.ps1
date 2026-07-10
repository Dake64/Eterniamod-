$ErrorActionPreference = "Stop"

# The Bleed branch is deepened with bleed-POWER nodes (not more proc chance), so
# investing in bleed makes the wound hit harder/last longer without re-inflating
# the application rate the player asked us to keep in check. The affinity-based
# contributions are capped so a deep tree cannot run the numbers away.

$repoRoot = Split-Path -Parent $PSScriptRoot
$registry = Get-Content -Raw (Join-Path $repoRoot "Content\Passives\PassiveRegistry.cs")
$warriorBleed = Get-Content -Raw (Join-Path $repoRoot "Content\Players\WarriorBleedPlayer.cs")
$bleedNPC = Get-Content -Raw (Join-Path $repoRoot "Content\NPCs\BleedGlobalNPC.cs")

$newNodes = @("Rupture", "Hemoplague", "Exsanguinate", "Bloodthirst")

foreach ($node in $newNodes) {
    $pattern = 'new PassiveNode\(\s*"' + [regex]::Escape($node) + '"[\s\S]*?"Bleed"'
    if ($registry -notmatch $pattern) {
        throw "PassiveRegistry should define the Bleed node '$node'."
    }
}

# Rupture boosts the DoT (in BleedGlobalNPC); the rest are wired in WarriorBleedPlayer.
if ($bleedNPC -notmatch '"Rupture"') {
    throw "Rupture should increase the bleed DoT in BleedGlobalNPC."
}

foreach ($node in @("Hemoplague", "Exsanguinate", "Bloodthirst")) {
    if ($warriorBleed -notmatch [regex]::Escape($node)) {
        throw "WarriorBleedPlayer should apply the '$node' bleed passive effect."
    }
}

# Affinity contributions are capped so deepening the tree does not run away.
if ($warriorBleed -notmatch "Math\.Min\(affinity") {
    throw "WarriorBleedPlayer should cap the Bleed affinity contribution (Math.Min)."
}

if ($bleedNPC -notmatch "Math\.Min\(") {
    throw "BleedGlobalNPC should cap the Bleed affinity contribution to the DoT (Math.Min)."
}

Write-Host "Bleed passive tree source smoke test passed."
