$ErrorActionPreference = "Stop"

# The shield line (pre-hardmode + hardmode). Every shield shares the base mechanic
# (channel a Defensive Aura via DefensiveAuraProjectile, Generic damage, no swing, ANY
# class can use it) and differs in stats + a personality effect. Aura damage rises tier
# to tier. Some shields are drop/chest rewards (obtention 2nd pass) instead of craftable.

$repoRoot = Split-Path -Parent $PSScriptRoot
$g = Join-Path $repoRoot "Content\Items\Weapons\Guardian"

# In tier order. Effect = the personality this shield must apply; DropOnly = obtained
# by drop/chest instead of a recipe.
$preHardmode = @(
    @{ Name = "WoodenShield";  Cap = 20; Effect = $null;    DropOnly = $false }, # start
    @{ Name = "IronShield";    Cap = 26; Effect = $null;    DropOnly = $false }, # ore
    @{ Name = "CorruptShield"; Cap = 32; Effect = "Ichor";  DropOnly = $true  }, # evil boss
    @{ Name = "GlacialShield"; Cap = 36; Effect = "Slow";   DropOnly = $false }, # ice
    @{ Name = "EmberShield";   Cap = 42; Effect = "OnFire"; DropOnly = $false }, # Underworld
    @{ Name = "HolyShield";    Cap = 50; Effect = "HEAL";   DropOnly = $false }  # final pre-WoF
)

$hardmode = @(
    @{ Name = "MythrilBulwark";    Cap = 66;  Effect = "Frostburn";     DropOnly = $false }, # HM ore
    @{ Name = "HallowedBulwark";   Cap = 82;  Effect = "CursedInferno"; DropOnly = $false }, # biome
    @{ Name = "NightfallBarrier";  Cap = 96;  Effect = "Ichor";         DropOnly = $false }, # event
    @{ Name = "PlaguebringerWall"; Cap = 114; Effect = "Venom";         DropOnly = $false }, # boss
    @{ Name = "LuminiteBarrier";   Cap = 140; Effect = "Ichor";         DropOnly = $false }, # Luminite
    @{ Name = "EternalAegis";      Cap = 170; Effect = "ShadowFlame";   DropOnly = $false }  # capstone
)

function Test-ShieldLine($line) {
    $previousDamage = 0

    foreach ($entry in $line) {
        $path = Join-Path $g "$($entry.Name).cs"

        if (!(Test-Path $path)) {
            throw "Missing shield '$($entry.Name)'."
        }

        $file = Get-Content -Raw $path

        # --- Shared base mechanic ---------------------------------------------
        if ($file -notmatch ":\s*ModItem,\s*IShieldWeapon") {
            throw "$($entry.Name) should be a shield (ModItem, IShieldWeapon)."
        }
        if ($file -notmatch "Item\.channel\s*=\s*true") {
            throw "$($entry.Name) should be channelled (hold to keep the aura up)."
        }
        if ($file -notmatch "DefensiveAuraProjectile") {
            throw "$($entry.Name) should project the DefensiveAuraProjectile."
        }
        if ($file -notmatch "DamageClass\.Generic") {
            throw "$($entry.Name) aura should be Generic (any class can use a shield)."
        }
        if ($file -notmatch "Item\.noMelee\s*=\s*true") {
            throw "$($entry.Name) should not swing (noMelee)."
        }
        if ($file -notmatch "ownedProjectileCounts\[type\]\s*==\s*0") {
            throw "$($entry.Name) should spawn only ONE persistent aura, not stack them."
        }

        # Any class can use it -> NOT subclass-locked.
        if ($file -match "SubclassLockHelper" -or $file -match "SubclassLockedItem") {
            throw "$($entry.Name) must NOT be subclass-locked (any class can use shields)."
        }

        # Craftable, UNLESS it is a drop/chest reward (obtention 2nd pass).
        if ($entry.DropOnly) {
            if ($file -match "CreateRecipe\(\)") {
                throw "$($entry.Name) is a drop/chest reward and must NOT be craftable."
            }
        }
        elseif ($file -notmatch "CreateRecipe\(\)") {
            throw "$($entry.Name) should be craftable."
        }

        # --- Personality effect -----------------------------------------------
        if ($null -ne $entry.Effect) {
            if ($entry.Effect -eq "HEAL") {
                if ($file -notmatch "void OnAuraPulse\(" -or $file -notmatch "statLife") {
                    throw "$($entry.Name) should lightly regen allies via OnAuraPulse."
                }
            }
            else {
                if ($file -notmatch "void OnAuraHit\(") {
                    throw "$($entry.Name) should apply its effect via OnAuraHit."
                }
                if ($file -notmatch ("BuffID\." + $entry.Effect)) {
                    throw "$($entry.Name) should inflict $($entry.Effect)."
                }
            }
        }

        # --- Aura damage progression ------------------------------------------
        $damage = [int][regex]::Match($file, "Item\.damage\s*=\s*(\d+);").Groups[1].Value
        if ($damage -le $previousDamage) {
            throw "$($entry.Name) aura does $damage, not more than the previous tier ($previousDamage)."
        }
        if ($damage -gt $entry.Cap) {
            throw "$($entry.Name) aura does $damage, above its tier cap ($($entry.Cap))."
        }
        $previousDamage = $damage
    }

    return $previousDamage
}

$topPre = Test-ShieldLine $preHardmode
$topHard = Test-ShieldLine $hardmode

# Hardmode shields must out-scale the whole pre-hardmode line.
$firstHard = [int][regex]::Match(
    (Get-Content -Raw (Join-Path $g "$($hardmode[0].Name).cs")),
    "Item\.damage\s*=\s*(\d+);").Groups[1].Value
if ($firstHard -le $topPre) {
    throw "The first Hardmode shield should out-damage the last pre-hardmode shield ($topPre)."
}

Write-Host "Shield arsenal source smoke test passed."
