$ErrorActionPreference = "Stop"

# The Elementalist's signature: an Elemental Affinity the player switches between five
# elements (Fire/Ice/Lightning/Wind/Earth). While promoted, the ACTIVE element reshapes
# practically every magic weapon. This pins that mechanic to source.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$player = Get-Content -Raw (Join-Path $c "Players\ElementalistPlayer.cs")
$proj = Get-Content -Raw (Join-Path $c "Globals\ElementalAffinityGlobalProjectile.cs")
$item = Get-Content -Raw (Join-Path $c "Globals\ElementalAffinityGlobalItem.cs")

# --- Five elements ------------------------------------------------------------
foreach ($element in @("Fire", "Ice", "Lightning", "Wind", "Earth")) {
    if ($player -notmatch ('"' + $element + '"')) {
        throw "ElementalistPlayer should expose the '$element' element."
    }
}
foreach ($affinity in @("WindAffinity", "EarthAffinity")) {
    if ($player -notmatch $affinity) {
        throw "ElementalistPlayer should track $affinity (5-element line)."
    }
}

# Wind/Earth must persist like the other elements.
if ($player -notmatch 'tag\["WindAffinity"\]' -or $player -notmatch 'tag\["EarthAffinity"\]') {
    throw "ElementalistPlayer should save the Wind/Earth affinities."
}

# --- Passive stat effects of the active element -------------------------------
$equips = [regex]::Match(
    $player,
    "public override void PostUpdateEquips\([\s\S]+?\n {8}\}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value
if ($equips -notmatch "IsActiveElementalist\(\)") {
    throw "The affinity stat effects must be gated on being a promoted Elementalist."
}
if ($equips -notmatch "GetDamage\(DamageClass\.Magic\)" -or   # Fire
    $equips -notmatch "manaCost" -or                          # Wind
    $equips -notmatch "statDefense") {                        # Earth
    throw "The active element should give its passive stat effect (fire damage / wind mana / earth defense)."
}

# --- The active affinity reshapes ALL magic weapons (global hooks) ------------
if ($proj -notmatch "class ElementalAffinityGlobalProjectile\s*:\s*GlobalProjectile") {
    throw "There should be a GlobalProjectile applying the active affinity to magic projectiles."
}
if ($proj -notmatch "CountsAsClass\(DamageClass\.Magic\)" -or
    $proj -notmatch "IsActiveElementalist\(\)") {
    throw "The affinity global should only affect an active Elementalist's magic projectiles."
}
# On-hit behaviour per element: burn, frost, chain lightning, earth burst.
if ($proj -notmatch "BuffID\.OnFire" -or
    $proj -notmatch "BuffID\.Frostburn" -or
    $proj -notmatch "BuffID\.Electrified" -or
    $proj -notmatch "ChainLightning" -or
    $proj -notmatch "EarthBurst") {
    throw "The affinity global should apply per-element on-hit effects."
}
# Fire's bonus-vs-burning and Wind's pierce.
if ($proj -notmatch "ModifyHitNPC" -or $proj -notmatch "penetrate") {
    throw "The affinity global should add fire bonus damage and wind pierce."
}

# Lightning/Wind faster projectiles (item side).
if ($item -notmatch "class ElementalAffinityGlobalItem\s*:\s*GlobalItem" -or
    $item -notmatch "ModifyShootStats" -or
    $item -notmatch "velocity") {
    throw "ElementalAffinityGlobalItem should speed up Lightning/Wind magic projectiles."
}

Write-Host "Elemental affinity source smoke test passed."
