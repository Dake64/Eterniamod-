$ErrorActionPreference = "Stop"

# The Peleador fist arsenal (pre-hardmode + hardmode).
#
# Shared base mechanics (ALL fist weapons):
#   - throw the FighterPunchProjectile (builds the Combo counter + distance mechanic),
#   - Melee, fast (low useTime), very low knockback,
#   - craftable, and NOT subclass-locked (any Warrior uses fists).
#
# Weapons evolve ONLY in damage / speed / secondary effect. Crucially, a weapon NEVER
# touches the Combo -- that is the subclass's job. Secondary effects (poison, fire,
# defense-down, lifesteal) are applied through IFistWeapon.OnPunchHit.

$repoRoot = Split-Path -Parent $PSScriptRoot
$f = Join-Path $repoRoot "Content\Items\Weapons\Fighter"

# Progression, in tier order. Cap = vanilla same-tier reference the fast fist stays under.
$preHardmode = @(
    @{ Name = "PaddedFists";           Cap = 11; Effect = $false; DropOnly = $false }, # start
    @{ Name = "IronKnuckles";          Cap = 15; Effect = $false; DropOnly = $false }, # ore
    @{ Name = "ProspectorsGauntlets";  Cap = 18; Effect = $false; DropOnly = $true  }, # chest/miner
    @{ Name = "ThornfistGauntlets";    Cap = 22; Effect = $true;  DropOnly = $false }, # biome (jungle)
    @{ Name = "MoltenKnuckles";        Cap = 26; Effect = $true;  DropOnly = $false }, # Underworld
    @{ Name = "BloodfeastGauntlets";   Cap = 32; Effect = $true;  DropOnly = $true  }  # evil boss
)

$hardmode = @(
    @{ Name = "MythrilBrawlers";       Cap = 52;  Effect = $true; DropOnly = $false }, # Hardmode ore
    @{ Name = "HallowedKnuckles";      Cap = 64;  Effect = $true; DropOnly = $false }, # biome (Hallow)
    @{ Name = "NightfallGauntlets";    Cap = 76;  Effect = $true; DropOnly = $false }, # event (eclipse)
    @{ Name = "PlaguebringerFists";    Cap = 92;  Effect = $true; DropOnly = $false }, # boss (Plantera)
    @{ Name = "LuminiteKnuckles";      Cap = 112; Effect = $true; DropOnly = $false }, # Luminite
    @{ Name = "EternalFury";           Cap = 132; Effect = $true; DropOnly = $false }  # Eternia capstone
)

function Test-FistLine($line) {
    $previousDamage = 0

    foreach ($entry in $line) {
        $path = Join-Path $f "$($entry.Name).cs"

        if (!(Test-Path $path)) {
            throw "Missing fist weapon '$($entry.Name)'."
        }

        $c = Get-Content -Raw $path

        # --- Shared base mechanics -------------------------------------------
        if ($c -notmatch "FighterPunchProjectile") {
            throw "$($entry.Name) should throw the FighterPunchProjectile (Combo + distance)."
        }
        if ($c -notmatch "DamageClass\.Melee") {
            throw "$($entry.Name) should be a Melee weapon."
        }
        # Craftable, UNLESS it is a drop/chest reward (obtention 2nd pass).
        if ($entry.DropOnly) {
            if ($c -match "CreateRecipe\(\)") {
                throw "$($entry.Name) is a drop/chest reward and must NOT be craftable."
            }
        }
        elseif ($c -notmatch "CreateRecipe\(\)") {
            throw "$($entry.Name) should be craftable."
        }
        if ($c -match "SubclassLockHelper" -or $c -match "SubclassLockedItem") {
            throw "$($entry.Name) must NOT be subclass-locked (any Warrior uses fists)."
        }

        $useTime = [int][regex]::Match($c, "Item\.useTime\s*=\s*(\d+);").Groups[1].Value
        if ($useTime -gt 15) {
            throw "$($entry.Name) useTime=$useTime is too slow; fists are fast (<= 15)."
        }

        $kb = [double][regex]::Match($c, "Item\.knockBack\s*=\s*([\d.]+)f;").Groups[1].Value
        if ($kb -gt 3.0) {
            throw "$($entry.Name) knockBack=$kb is too high; fists have very low knockback (<= 3)."
        }

        # --- Weapons NEVER touch the Combo (comments may mention it; CODE must not
        #     read/modify it) ---------------------------------------------------
        if ($c -match "AddCombo" -or $c -match "FighterPlayer" -or $c -match "\.Combo\b") {
            throw "$($entry.Name) must not touch the Combo system in code (that is the subclass's job)."
        }

        # --- Secondary effect (optional) via IFistWeapon.OnPunchHit ----------
        if ($entry.Effect) {
            if ($c -notmatch "IFistWeapon") {
                throw "$($entry.Name) has a secondary effect and should implement IFistWeapon."
            }
            if ($c -notmatch "void OnPunchHit\(") {
                throw "$($entry.Name) should apply its effect through OnPunchHit."
            }
            if ($c -notmatch "AddBuff" -and $c -notmatch "statLife") {
                throw "$($entry.Name).OnPunchHit should apply a buff or lifesteal."
            }
        }

        # --- Damage progression ---------------------------------------------
        $damage = [int][regex]::Match($c, "Item\.damage\s*=\s*(\d+);").Groups[1].Value
        if ($damage -le $previousDamage) {
            throw "$($entry.Name) does $damage damage, not more than the previous tier ($previousDamage)."
        }
        if ($damage -gt $entry.Cap) {
            throw "$($entry.Name) does $damage damage, above its same-tier cap ($($entry.Cap))."
        }
        $previousDamage = $damage
    }

    return $previousDamage
}

$topPre = Test-FistLine $preHardmode
$topHard = Test-FistLine $hardmode

# Hardmode weapons must out-scale the whole pre-hardmode line.
if ($hardmode[0].Name -and ([int][regex]::Match(
        (Get-Content -Raw (Join-Path $f "$($hardmode[0].Name).cs")),
        "Item\.damage\s*=\s*(\d+);").Groups[1].Value) -le $topPre) {
    throw "The first Hardmode fist should out-damage the last pre-hardmode fist ($topPre)."
}

# --- The punch wires the weapon's secondary effect ---------------------------
$punch = Get-Content -Raw (Join-Path $repoRoot "Content\Projectiles\FighterPunchProjectile.cs")
if ($punch -notmatch "is IFistWeapon" -or $punch -notmatch "OnPunchHit\(") {
    throw "FighterPunchProjectile should invoke the held IFistWeapon's OnPunchHit."
}

# --- The interface exposes ONLY the secondary on-hit effect, not the Combo ----
$iface = Get-Content -Raw (Join-Path $repoRoot "Content\Items\IFistWeapon.cs")
if ($iface -notmatch "void OnPunchHit\(") {
    throw "IFistWeapon should declare the OnPunchHit secondary-effect hook."
}
if ($iface -match "int\s+Combo" -or $iface -match "AddCombo") {
    throw "IFistWeapon must not expose the Combo as an interface member."
}

# The promotion-reward gauntlet stays Fighter-locked (it is a hardmode reward).
$gauntlet = Get-Content -Raw (Join-Path $f "TrainingGauntlet.cs")
if ($gauntlet -notmatch "SubclassLockHelper") {
    throw "TrainingGauntlet (Peleador promotion reward) should stay Fighter-locked."
}

Write-Host "Fist arsenal source smoke test passed."
