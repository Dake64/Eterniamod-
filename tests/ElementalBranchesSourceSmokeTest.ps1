$ErrorActionPreference = "Stop"

# The Elemental passive branch is split into FIVE element sub-branches (Fire, Ice,
# Lightning, Wind, Earth). Each is its own affinity spoke, but all feed the same
# Elemental affinity toward the Elementalist promotion. Pre-Hardmode they give a magic
# bonus; once promoted, specific nodes supercharge the active element.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$registry = Get-Content -Raw (Join-Path $c "Passives\PassiveRegistry.cs")
$progression = Get-Content -Raw (Join-Path $c "Progression\ProgressionService.cs")
$proj = Get-Content -Raw (Join-Path $c "Globals\ElementalAffinityGlobalProjectile.cs")
$item = Get-Content -Raw (Join-Path $c "Globals\ElementalAffinityGlobalItem.cs")

# --- Five element sub-branches, each with a chain of nodes --------------------
$elements = @{
    "Fire"      = @("Kindling", "Ember Fury", "Pyromancer")
    "Ice"       = @("Frost Touch", "Deep Freeze", "Absolute Zero")
    "Lightning" = @("Static Charge", "Chain Master", "Tempest")
    "Wind"      = @("Zephyr", "Gale Force", "Tempest Winds")
    "Earth"     = @("Stone Skin", "Tremor", "Tectonic")
}

foreach ($pair in $elements.GetEnumerator()) {
    foreach ($node in $pair.Value) {
        $pattern = 'new PassiveNode\(\s*"' + [regex]::Escape($node) + '"[\s\S]*?"' + $pair.Key + '"'
        if ($registry -notmatch $pattern) {
            throw "PassiveRegistry should define '$node' as a '$($pair.Key)' element node."
        }
    }
}

# The old single "Elemental" branch nodes must be gone.
foreach ($old in @("Elemental Control", "Elemental Mastery", "Pyroclasm", "Storm Caller")) {
    if ($registry -match ('"' + [regex]::Escape($old) + '"')) {
        throw "The old generic Elemental node '$old' should be replaced by the element sub-branches."
    }
}

# --- All five elements feed the Elemental affinity (promotion unchanged) ------
foreach ($element in @("Fire", "Ice", "Lightning", "Wind", "Earth")) {
    if ($progression -notmatch ('case "' + $element + '":')) {
        throw "ProgressionService.AddAffinity should route '$element' to the Elemental affinity."
    }
}
if ($progression -notmatch "ElementalAffinity \+= amount") {
    throw "The element affinities should accumulate into ElementalAffinity."
}

# --- Hardmode synergies: nodes supercharge the active element -----------------
foreach ($node in @("Ember Fury", "Pyromancer", "Absolute Zero", "Chain Master",
                    "Tempest", "Gale Force")) {
    if ($proj -notmatch ('HasElementNode\("' + [regex]::Escape($node) + '"\)')) {
        throw "The affinity global should let '$node' supercharge its element."
    }
}
if ($item -notmatch 'HasElementNode\("Tempest Winds"\)') {
    throw "The affinity item global should let 'Tempest Winds' speed Wind projectiles further."
}

# --- Elemental Mastery (Hardmode): faster swaps + swap surge ------------------
$player = Get-Content -Raw (Join-Path $c "Players\ElementalistPlayer.cs")

foreach ($node in @("Elemental Flux", "Momentum Shift", "Grand Attunement")) {
    if ($registry -notmatch ('new PassiveNode\(\s*"' + [regex]::Escape($node) + '"[\s\S]*?"Elemental"')) {
        throw "PassiveRegistry should define the Elemental Mastery node '$node'."
    }
}

# A switch cooldown that the mastery nodes shorten.
if ($player -notmatch "SwitchTimer" -or $player -notmatch "SwitchCooldown\(") {
    throw "ElementalistPlayer should gate element swaps behind a cooldown (SwitchTimer)."
}
if ($player -notmatch 'HasElementNode\("Elemental Flux"\)' -or
    $player -notmatch 'HasElementNode\("Grand Attunement"\)') {
    throw "Elemental Flux / Grand Attunement should shorten the swap cooldown."
}

# A temporary magic surge on swap (Momentum Shift), applied while SurgeTimer runs.
if ($player -notmatch "SurgeTimer" -or
    $player -notmatch 'HasElementNode\("Momentum Shift"\)') {
    throw "Momentum Shift should grant a temporary surge on swap (SurgeTimer)."
}
if ($player -notmatch "SurgeAmount\(\)" -or
    $player -notmatch "GetDamage\(DamageClass\.Magic\)") {
    throw "The swap surge should boost magic damage while active."
}

Write-Host "Elemental branches source smoke test passed."
