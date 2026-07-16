$ErrorActionPreference = "Stop"

# The Swordsman's edge weapons throw a bleeding slash so they can hit from range
# like most melee weapons. This pins:
#  - every mod bleed sword (IBleedWeapon) fires a beam whose damage is set centrally
#    through BeamDamageFactor (one knob to tune; currently full sword damage, because
#    a halved beam got floored to 1 by Terraria's defense/2 subtraction),
#  - the beam applies bleed on projectile hit (chance for any Warrior, guaranteed +
#    Crimson Trail for the Swordsman), mirroring the direct edge-hit hooks.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$globalItem = Get-Content -Raw (Join-Path $c "Globals\EterniaGlobalItem.cs")
$warriorBleed = Get-Content -Raw (Join-Path $c "Players\WarriorBleedPlayer.cs")
$swordsman = Get-Content -Raw (Join-Path $c "Players\SwordsmanPlayer.cs")

# --- The slash is set centrally for all bleed swords, at reduced damage ---------
if ($globalItem -notmatch "public override void SetDefaults\(Item item\)" -or
    $globalItem -notmatch "item\.ModItem is IBleedWeapon" -or
    $globalItem -notmatch "item\.shoot = ModContent\.ProjectileType<CrimsonSlash>") {
    throw "EterniaGlobalItem should give every IBleedWeapon sword the CrimsonSlash (item.shoot)."
}

if ($globalItem -notmatch "ModifyShootStats" -or
    $globalItem -notmatch "BeamDamageFactor") {
    throw "EterniaGlobalItem should set the thrown beam's damage centrally (BeamDamageFactor)."
}

# --- Bleed applies on the beam (projectile) hit, not just the swing -------------
if ($warriorBleed -notmatch "public override void OnHitNPCWithProj\(") {
    throw "WarriorBleedPlayer should roll the bleed chance on the beam's projectile hit."
}

if ($warriorBleed -notmatch "CanInflictBleed\(Player\.HeldItem\)") {
    throw "The beam bleed should be gated to holding a bleed sword (Player.HeldItem)."
}

if ($swordsman -notmatch "public override void OnHitNPCWithProj\(" -or
    $swordsman -notmatch "ApplyBleed" -or
    $swordsman -notmatch "CrimsonTrailPlayer") {
    throw "SwordsmanPlayer should apply guaranteed bleed + Crimson Trail on the beam hit too."
}

# --- Both proj hooks only act on MELEE-class projectiles ------------------------
foreach ($pair in @(
    @{ Name = "WarriorBleedPlayer"; Content = $warriorBleed },
    @{ Name = "SwordsmanPlayer";    Content = $swordsman })) {
    if ($pair.Content -notmatch "proj\.DamageType\.CountsAsClass\(DamageClass\.Melee\)") {
        throw "$($pair.Name) beam hooks should only act on melee-class projectiles."
    }
}

# --- The custom slash projectile: unique per sword + balance baked in -----------
$slash = Get-Content -Raw (Join-Path $c "Projectiles\Warrior\CrimsonSlash.cs")

if ($slash -notmatch "class CrimsonSlash\s*:\s*ModProjectile" -or
    $slash -notmatch "DamageClass\.Melee" -or
    $slash -notmatch "IBleedWeapon bleed" -or
    $slash -notmatch "bleed\.SlashColor" -or
    $slash -notmatch "bleed\.SlashScale") {
    throw "CrimsonSlash should read the firing sword's SlashColor/SlashScale so each sword is unique."
}

# Balance (short reach + low pierce) lives in the projectile now.
if ($slash -notmatch "timeLeft = \d+" -or
    $slash -notmatch "penetrate = \d+") {
    throw "CrimsonSlash should bake in the short-range/low-pierce balance."
}

# The interface exposes the per-sword slash customization.
$iface = Get-Content -Raw (Join-Path $c "Items\IBleedWeapon.cs")
if ($iface -notmatch "Color SlashColor" -or $iface -notmatch "float SlashScale") {
    throw "IBleedWeapon should expose SlashColor/SlashScale for per-sword slashes."
}

# Every signature sword actually overrides the slash colour (unique look).
$warriorDir = Join-Path $repoRoot "Content\Items\Weapons\Warrior"
$swords = Get-ChildItem $warriorDir -Filter *.cs
$overrides = 0
foreach ($s in $swords) {
    if ((Get-Content -Raw $s.FullName) -match "SlashColor => new Color\(") { $overrides++ }
}
if ($overrides -lt 16) {
    throw "Expected every Warrior bleed sword to override SlashColor (found $overrides)."
}

Write-Host "Sword beam source smoke test passed."
