$ErrorActionPreference = "Stop"

# The Swordsman blades used to share one placeholder texture ("MOD") and throw one near-blank
# slash sprite. Every blade now ships its OWN sprite, and the slash projectile draws a shape
# chosen by the weapon's style. This test pins that the art exists and stays wired, so nobody
# silently falls back to the placeholder again.

$repoRoot = Split-Path -Parent $PSScriptRoot
$warrior = Join-Path $repoRoot "Content\Items\Weapons\Warrior"
$promo   = Join-Path $repoRoot "Content\Items\Weapons\Promotion"
$proj    = Join-Path $repoRoot "Content\Projectiles\Warrior"

$warriorBlades = @('TrainingBlade','SerratedIronBlade','SilverlightRapier','HuntersWarblade',
'CorruptorsRipper','DreadReaver','Thornrender','BonewardenSabre','ReveniteCleaver','MoltenGutripper',
'QuicksilverFang','SanguineCleaver','HallowedBloodletter','NullsteelReaver','ChlorophyteHemoblade',
'TitansGutcleaver','CrimsonRequiem','Exsanguinator')

# --- Every Warrior blade has its own PNG and no placeholder Texture override -----
foreach ($n in $warriorBlades) {
    $png = Join-Path $warrior ($n + ".png")
    if (-not (Test-Path $png)) {
        throw "$n is missing its own sprite ($n.png)."
    }
    $src = Get-Content -Raw (Join-Path $warrior ($n + ".cs"))
    if ($src -match 'override string Texture') {
        throw "$n still overrides Texture (should default to its own $n.png)."
    }
    if ($src -match 'TrainingGauntlet' -or $src -match 'Souls/WarriorSoul') {
        throw "$n still points at a placeholder texture."
    }
}

# --- The signature Bloodletter Blade points its TexturePath at its own sprite -----
if (-not (Test-Path (Join-Path $promo "BloodletterBlade.png"))) {
    throw "BloodletterBlade is missing its own sprite."
}
$blSrc = Get-Content -Raw (Join-Path $promo "BloodletterBlade.cs")
if ($blSrc -notmatch 'Promotion/BloodletterBlade') {
    throw "BloodletterBlade should point TexturePath at its own sprite."
}
if ($blSrc -match 'TrainingGauntlet') {
    throw "BloodletterBlade still points at the placeholder."
}

# --- The four slash shapes exist -----------------------------------------------
foreach ($slash in @('CrimsonSlash','CrimsonSlash_Heavy','CrimsonSlash_Pierce','CrimsonSlash_Return')) {
    if (-not (Test-Path (Join-Path $proj ($slash + ".png")))) {
        throw "Missing slash texture $slash.png -- the projectile can't vary its shape."
    }
}

# --- The slash projectile draws a shape chosen by style ------------------------
$slashSrc = Get-Content -Raw (Join-Path $proj "CrimsonSlash.cs")
if ($slashSrc -notmatch 'PreDraw') {
    throw "CrimsonSlash should draw itself (PreDraw) to pick a per-style shape."
}
foreach ($tex in @('_Heavy','_Pierce','_Return')) {
    if ($slashSrc -notmatch [regex]::Escape($tex)) {
        throw "CrimsonSlash draw does not select the $tex shape."
    }
}

Write-Host "Sword sprite source smoke test passed."
