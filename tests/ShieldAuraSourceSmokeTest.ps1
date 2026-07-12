$ErrorActionPreference = "Stop"

# The Shield weapon category: holding left-click raises the shield and, after a brief
# spin-up (~0.5s), projects a Defensive Aura that pulses damage to nearby enemies while
# the shield is held. Any class can use it; the Guardian (Escudero) gets the most out
# of it (aura scales with Defense). This pins that mechanic to source.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$iface = Get-Content -Raw (Join-Path $c "Items\IShieldWeapon.cs")
$aura = Get-Content -Raw (Join-Path $c "Projectiles\Guardian\DefensiveAuraProjectile.cs")
$guardian = Get-Content -Raw (Join-Path $c "Players\GuardianPlayer.cs")

# --- The interface exposes the aura identity + effect hooks -------------------
foreach ($member in @("AuraPulseInterval", "AuraRadius", "AuraColor",
                      "void OnAuraHit\(", "void OnAuraPulse\(")) {
    if ($iface -notmatch $member) {
        throw "IShieldWeapon should expose $member."
    }
}

# --- The aura is a channelled controller -------------------------------------
if ($aura -notmatch "class DefensiveAuraProjectile\s*:\s*ModProjectile") {
    throw "DefensiveAuraProjectile should be a ModProjectile."
}

# Disappears the instant the shield is lowered (owner stops channelling).
if ($aura -notmatch "owner\.channel" -or $aura -notmatch "Projectile\.Kill\(\)") {
    throw "The aura should end the moment the owner stops channelling the shield."
}

# It is drawn as a dust ring; the reused placeholder texture must NOT be painted
# (otherwise a stray white slash sprite shows behind the player).
if ($aura -notmatch "bool PreDraw\(ref Color lightColor\)\s*=>\s*false") {
    throw "DefensiveAuraProjectile should suppress its placeholder sprite (PreDraw => false)."
}

# Spin-up (~0.5s = 30 ticks) before it deals damage.
if ($aura -notmatch "WarmupTicks\s*=\s*30") {
    throw "The aura should take ~0.5s (30 ticks) to spin up before dealing damage."
}

# Reads the held shield's identity and applies its personality effect.
if ($aura -notmatch "is not IShieldWeapon" -and $aura -notmatch "is IShieldWeapon") {
    throw "The aura should read the held IShieldWeapon."
}
if ($aura -notmatch "OnAuraHit\(" -or $aura -notmatch "OnAuraPulse\(") {
    throw "The aura should apply the shield's OnAuraHit / OnAuraPulse effects."
}

# Deals damage manually, class-neutral (Generic) so any class can use a shield.
if ($aura -notmatch "SimpleStrikeNPC" -or $aura -notmatch "DamageClass\.Generic") {
    throw "The aura should pulse Generic damage to enemies in radius."
}

# Pulses apply knockback (so the aura visibly pushes enemies, not 0 knockback).
if ($aura -notmatch "Projectile\.knockBack") {
    throw "The aura pulse should apply the shield's knockback."
}

# Uses a small radius (hug the enemy). Radius comes from the shield.
if ($aura -notmatch "AuraRadius") {
    throw "The aura radius should come from the shield's AuraRadius."
}

# --- Guardian (Escudero) payoff: aura scales with Defense --------------------
if ($guardian -notmatch "AuraDamageMultiplier" -or
    $guardian -notmatch "AuraRadiusMultiplier") {
    throw "GuardianPlayer should expose the aura scaling helpers."
}

# The damage scaling must be gated on being an active Guardian and use Defense.
$auraDmg = [regex]::Match(
    $guardian,
    "public float AuraDamageMultiplier\(\)[\s\S]+?\n {8}\}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($auraDmg -notmatch "IsActiveGuardian\(\)" -or
    $auraDmg -notmatch "statDefense") {
    throw "AuraDamageMultiplier should scale with Defense only for an active Guardian."
}

# The aura must actually apply the Guardian multipliers.
if ($aura -notmatch "AuraDamageMultiplier\(\)" -or
    $aura -notmatch "AuraRadiusMultiplier\(\)") {
    throw "DefensiveAuraProjectile should apply the Guardian's aura multipliers."
}

# The Escudero's aura damage must include a flat weapon bonus (not only defense).
if ($auraDmg -notmatch "1\.25f") {
    throw "AuraDamageMultiplier should give the Guardian a flat aura-damage bonus (its own weapon)."
}

# --- Guard bonus: any class takes less damage while the aura is up (survive the hug) -
$shieldPlayer = Get-Content -Raw (Join-Path $c "Players\ShieldPlayer.cs")
if ($shieldPlayer -notmatch "class ShieldPlayer\s*:\s*ModPlayer" -or
    $shieldPlayer -notmatch "ownedProjectileCounts" -or
    $shieldPlayer -notmatch "Player\.endurance\s*\+=") {
    throw "ShieldPlayer should grant a guard damage-reduction while the shield aura is up."
}

# --- The Defense passive tree shapes the Escudero's aura ----------------------
# Damage nodes, radius nodes, pulse-speed nodes and a per-pulse effect are all read
# from the Defense tree in GuardianPlayer.
foreach ($node in @("Iron Wall", "Fortress Body", "Aegis",   # aura damage
                    "Shield Training", "Bulwark",             # aura radius
                    "Unbreakable", "Stonewall",               # aura pulse speed
                    "Last Bastion")) {                         # per-pulse effect
    if ($guardian -notmatch ('HasDefensePassive\("' + [regex]::Escape($node) + '"\)')) {
        throw "GuardianPlayer should read the Defense passive '$node' to shape the aura."
    }
}

if ($guardian -notmatch "public float AuraPulseMultiplier\(" -or
    $guardian -notmatch "public void ApplyGuardianAuraPulse\(") {
    throw "GuardianPlayer should expose aura pulse-speed and per-pulse passive hooks."
}

# The aura must actually use the pulse-speed multiplier + per-pulse Guardian effect.
if ($aura -notmatch "AuraPulseMultiplier\(\)" -or
    $aura -notmatch "ApplyGuardianAuraPulse\(") {
    throw "DefensiveAuraProjectile should apply the Guardian's pulse speed and per-pulse effects."
}

Write-Host "Shield aura source smoke test passed."
