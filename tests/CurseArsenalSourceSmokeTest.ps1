$ErrorActionPreference = "Stop"

# The Cursed Mage arsenal: 5 pre-hardmode + 7 hardmode weapons. Each is a CurseWeapon
# that runs on Cursed Energy (not mana): CanUseItem gates on energy, Shoot consumes it
# and (in hardmode) adds Corruption. Damage rises tier to tier. None are subclass-locked.

$repoRoot = Split-Path -Parent $PSScriptRoot
$m = Join-Path $repoRoot "Content\Items\Weapons\Magic"

# In progression order. Corruption = weapon adds Corruption per cast (hardmode).
$line = @(
    @{ Name = "InitiatesGrimoire";     Dmg = 14;  Corruption = $false },
    @{ Name = "VoidRod";               Dmg = 20;  Corruption = $false },
    @{ Name = "ForbiddenTome";         Dmg = 24;  Corruption = $false },
    @{ Name = "EclipseStaff";          Dmg = 10;  Corruption = $false },  # rapid; low per-hit
    @{ Name = "AbyssalCodex";          Dmg = 40;  Corruption = $false },
    @{ Name = "GrimoireOfSin";         Dmg = 60;  Corruption = $true  },
    @{ Name = "CursedStaff";           Dmg = 68;  Corruption = $true  },
    @{ Name = "TomeOfTheAbyss";        Dmg = 78;  Corruption = $true  },
    @{ Name = "DoomOrb";               Dmg = 50;  Corruption = $false },  # power is the orb
    @{ Name = "BookOfCollapse";        Dmg = 95;  Corruption = $true  },
    @{ Name = "ScepterOfTheEnd";       Dmg = 115; Corruption = $true  },
    @{ Name = "NecronomiconOfEternia"; Dmg = 140; Corruption = $true  }
)

foreach ($entry in $line) {
    $path = Join-Path $m "$($entry.Name).cs"
    if (!(Test-Path $path)) {
        throw "Missing curse weapon '$($entry.Name)'."
    }

    $c = Get-Content -Raw $path

    if ($c -notmatch ":\s*CurseWeapon") {
        throw "$($entry.Name) should extend CurseWeapon."
    }
    if ($c -notmatch "public override int EnergyCost =>") {
        throw "$($entry.Name) should declare an EnergyCost."
    }
    if ($c -notmatch "CreateRecipe\(\)") {
        throw "$($entry.Name) should be craftable."
    }
    if ($c -match "SubclassLockHelper" -or $c -match "SubclassLockedItem") {
        throw "$($entry.Name) must NOT be subclass-locked (any Mage can use curse weapons)."
    }

    $dmg = [int][regex]::Match($c, "SetCurseDefaults\((\d+)").Groups[1].Value
    if ($dmg -ne $entry.Dmg) {
        throw "$($entry.Name) should do $($entry.Dmg) damage but does $dmg."
    }

    if ($entry.Corruption -and $c -notmatch "CorruptionPerCast =>") {
        throw "$($entry.Name) is a hardmode curse weapon and should generate Corruption."
    }
}

# --- Base class enforces the two-resource contract ---------------------------
$base = Get-Content -Raw (Join-Path $m "CurseWeapon.cs")
if ($base -notmatch "ConsumeEnergy\(EnergyCost\)") {
    throw "CurseWeapon should consume Cursed Energy on cast."
}
if ($base -notmatch "IsActiveCursedMage\(\)[\s\S]*AddTemporaryCorruption\(CorruptionPerCast\)") {
    throw "CurseWeapon should add Corruption per cast only for a promoted Cursed Mage."
}
if ($base -notmatch "CanUseItem" -or $base -notmatch "CursedEnergy >= EnergyCost") {
    throw "CurseWeapon.CanUseItem should gate on Cursed Energy."
}
if ($base -notmatch "ModifyWeaponDamage" -or $base -notmatch "CorruptionDamageMultiplier") {
    throw "CurseWeapon should let damage scale with Corruption."
}

# --- Corruption scaling weapons ----------------------------------------------
$staff = Get-Content -Raw (Join-Path $m "CursedStaff.cs")
if ($staff -notmatch "corruption > 50") {
    throw "Cursed Staff should scale its damage past 50 corruption."
}
$collapse = Get-Content -Raw (Join-Path $m "BookOfCollapse.cs")
if ($collapse -notmatch "corruption > 150") {
    throw "Book of Collapse should spike past 150 corruption."
}
$necro = Get-Content -Raw (Join-Path $m "NecronomiconOfEternia.cs")
if ($necro -notmatch "BurstMultiplier => " -or $necro -notmatch "corruption \* 0\.004") {
    throw "The Necronomicon should scale with corruption and amplify Cursed Burst."
}

Write-Host "Curse arsenal source smoke test passed."
