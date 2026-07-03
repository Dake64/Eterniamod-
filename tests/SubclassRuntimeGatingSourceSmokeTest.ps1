$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$contentRoot = Join-Path $repoRoot "Content"

$elementalist = Get-Content -Raw (Join-Path $contentRoot "Players\ElementalistPlayer.cs")
$virtuoso = Get-Content -Raw (Join-Path $contentRoot "Players\VirtuosoPlayer.cs")
$cursedMage = Get-Content -Raw (Join-Path $contentRoot "Players\CursedMagePlayer.cs")
$archer = Get-Content -Raw (Join-Path $contentRoot "Players\ArcherPlayer.cs")
$energyGunner = Get-Content -Raw (Join-Path $contentRoot "Players\EnergyShooterPlayer.cs")
$berserker = Get-Content -Raw (Join-Path $contentRoot "Players\BerserkerPlayer.cs")
$berserkerSkill = Get-Content -Raw (Join-Path $contentRoot "Players\BerserkerSkillPlayer.cs")
$fighter = Get-Content -Raw (Join-Path $contentRoot "Players\FighterPlayer.cs")
$guardian = Get-Content -Raw (Join-Path $contentRoot "Players\GuardianPlayer.cs")
$gunner = Get-Content -Raw (Join-Path $contentRoot "Players\GunnerPlayer.cs")
$necromancer = Get-Content -Raw (Join-Path $contentRoot "Players\NecromancerPlayer.cs")
$stunner = Get-Content -Raw (Join-Path $contentRoot "Players\StunnerPlayer.cs")
$swordsman = Get-Content -Raw (Join-Path $contentRoot "Players\SwordsmanPlayer.cs")
$yoyoMaster = Get-Content -Raw (Join-Path $contentRoot "Players\YoyoMasterPlayer.cs")
$baseClass = Get-Content -Raw (Join-Path $contentRoot "Players\BaseClassPlayer.cs")
$subclassEffects = Get-Content -Raw (Join-Path $contentRoot "Players\SubclassEffectsPlayer.cs")
$fighterProjectile = Get-Content -Raw (Join-Path $contentRoot "Projectiles\FighterPunchProjectile.cs")
$subclassLockHelper = Get-Content -Raw (Join-Path $contentRoot "Items\Weapons\Promotion\SubclassLockHelper.cs")

if ($elementalist -notmatch "public bool IsActiveElementalist\(") {
    throw "ElementalistPlayer should expose IsActiveElementalist so delayed projectiles can verify subclass ownership."
}

$activeElementalist = [regex]::Match(
    $elementalist,
    'public bool IsActiveElementalist\([\s\S]+?\n\s*}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($activeElementalist -notmatch "GetModPlayer<EterniaPlayer>\(\)" -or
    $activeElementalist -notmatch "HasClassSoul" -or
    $activeElementalist -notmatch "ActiveSoul == SoulId\.Mage") {
    throw "ElementalistPlayer.IsActiveElementalist should require an equipped Mage class Soul, not only a subclass name."
}

foreach ($method in @("GainCharge", "GainAffinity")) {
    $pattern = "public bool $method[\s\S]*?if \(!IsActiveElementalist\(\)\)"

    if ($elementalist -notmatch $pattern) {
        throw "ElementalistPlayer.$method should refuse progression when the player is not Elementalist."
    }
}

foreach ($projectileName in @(
    "FireBoltProjectile.cs",
    "IceBoltProjectile.cs",
    "LightningBoltProjectile.cs")) {
    $projectile = Get-Content -Raw (Join-Path $contentRoot "Projectiles\$projectileName")

    if ($projectile -notmatch "if \(!elementalist\.IsActiveElementalist\(\)\)") {
        throw "$projectileName should stop OnHitNPC subclass effects when the owner is no longer Elementalist."
    }

    if ($projectile -match "elementalist\.(FireAffinity|IceAffinity|LightningAffinity)\+\+") {
        throw "$projectileName should use ElementalistPlayer.GainAffinity instead of mutating affinity fields directly."
    }
}

$elementalStaff = Get-Content -Raw (Join-Path $contentRoot "Items\Weapons\Magic\ElementalApprenticeStaff.cs")
if ($elementalStaff -match "elementalist\.(FireAffinity|IceAffinity|LightningAffinity)\+\+") {
    throw "ElementalApprenticeStaff should use ElementalistPlayer.GainAffinity instead of mutating affinity fields directly."
}

if ($virtuoso -notmatch "public bool IsActiveVirtuoso\(") {
    throw "VirtuosoPlayer should centralize runtime subclass checks."
}

$activeVirtuoso = [regex]::Match(
    $virtuoso,
    'public bool IsActiveVirtuoso\([\s\S]+?\n\s*}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($activeVirtuoso -notmatch "GetModPlayer<EterniaPlayer>\(\)" -or
    $activeVirtuoso -notmatch "HasClassSoul" -or
    $activeVirtuoso -notmatch "ActiveSoul == SoulId\.Ranger") {
    throw "VirtuosoPlayer.IsActiveVirtuoso should require an equipped Ranger class Soul, not only a subclass name."
}

foreach ($method in @("ModifyWeaponDamage", "PostUpdateRunSpeeds")) {
    $pattern = "override .* $method[\s\S]*?if \(!IsActiveVirtuoso\(\)\)"

    if ($virtuoso -notmatch $pattern) {
        throw "VirtuosoPlayer.$method should verify active Virtuoso before applying temporary buffs."
    }
}

if ($cursedMage -notmatch "public bool IsActiveCursedMage\(") {
    throw "CursedMagePlayer should expose IsActiveCursedMage for projectile and resource gating."
}

$activeCursedMage = [regex]::Match(
    $cursedMage,
    'public bool IsActiveCursedMage\([\s\S]+?\n\s*}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($activeCursedMage -notmatch "GetModPlayer<EterniaPlayer>\(\)" -or
    $activeCursedMage -notmatch "HasClassSoul" -or
    $activeCursedMage -notmatch "ActiveSoul == SoulId\.Mage") {
    throw "CursedMagePlayer.IsActiveCursedMage should require an equipped Mage class Soul, not only a subclass name."
}

foreach ($check in @(
    @{
        Name="ArcherPlayer"
        Content=$archer
        Helper="IsActiveArcher"
        Soul="Ranger"
        Methods=@("ResetEffects", "PostUpdate", "ModifyShootStats", "OnHitNPCWithProj", "ModifyWeaponCrit", "UseSpeedMultiplier")
    },
    @{
        Name="EnergyShooterPlayer"
        Content=$energyGunner
        Helper="IsActiveEnergyGunner"
        Soul="Ranger"
        Methods=@("ResetEffects", "PostUpdate", "CanUseItem", "ModifyWeaponDamage", "UseSpeedMultiplier", "OnHitNPCWithProj")
    },
    @{
        Name="BerserkerPlayer"
        Content=$berserker
        Helper="IsActiveBerserker"
        Soul="Warrior"
        Methods=@("ResetEffects", "PostUpdate", "OnHitNPCWithItem", "OnHitNPCWithProj", "OnHurt")
    })) {
    if ($check.Content -notmatch "public bool $($check.Helper)\(") {
        throw "$($check.Name) should expose $($check.Helper) for runtime subclass checks."
    }

    $helperBody = [regex]::Match(
        $check.Content,
        "public bool $($check.Helper)\([\s\S]+?\n\s*}",
        [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

    if ($helperBody -notmatch "GetModPlayer<EterniaPlayer>\(\)" -or
        $helperBody -notmatch "HasClassSoul" -or
        $helperBody -notmatch "ActiveSoul == SoulId\.$($check.Soul)") {
        throw "$($check.Name).$($check.Helper) should require an equipped $($check.Soul) class Soul, not only a subclass name."
    }

    foreach ($method in $check.Methods) {
        $methodBody = [regex]::Match(
            $check.Content,
            "(override .* $method|public .* $method)\([\s\S]+?\n\s*}",
            [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

        if ($methodBody -notmatch "!$($check.Helper)\(\)") {
            throw "$($check.Name).$method should gate behavior with $($check.Helper)()."
        }
    }
}

if ($berserkerSkill -notmatch "GetModPlayer<BerserkerPlayer>\(\)\.IsActiveBerserker\(\)") {
    throw "BerserkerSkillPlayer should use BerserkerPlayer.IsActiveBerserker so the skill also requires an equipped Warrior Soul."
}

foreach ($check in @(
    @{
        Name="FighterPlayer"
        Content=$fighter
        Helper="IsActiveFighter"
        Soul="Warrior"
        Methods=@("ResetEffects", "PostUpdate", "AddCombo", "GetComboMultiplier")
    },
    @{
        Name="GuardianPlayer"
        Content=$guardian
        Helper="IsActiveGuardian"
        Soul="Warrior"
        Methods=@("ResetEffects", "OnHurt")
    },
    @{
        Name="GunnerPlayer"
        Content=$gunner
        Helper="IsActiveGunner"
        Soul="Ranger"
        Methods=@("ResetEffects", "PostUpdate", "ModifyShootStats", "ModifyWeaponCrit", "UseSpeedMultiplier", "OnHitNPCWithProj")
    },
    @{
        Name="NecromancerPlayer"
        Content=$necromancer
        Helper="IsActiveNecromancer"
        Soul="Summoner"
        Methods=@("PostUpdate")
    },
    @{
        Name="StunnerPlayer"
        Content=$stunner
        Helper="IsActiveStunner"
        Soul="Warrior"
        Methods=@("ResetEffects", "PostUpdate", "ModifyHitNPCWithItem", "OnHitNPCWithItem")
    },
    @{
        Name="SwordsmanPlayer"
        Content=$swordsman
        Helper="IsActiveSwordsman"
        Soul="Warrior"
        Methods=@("OnHitNPCWithItem")
    },
    @{
        Name="YoyoMasterPlayer"
        Content=$yoyoMaster
        Helper="IsActiveYoyoMaster"
        Soul="Warrior"
        Methods=@("ResetEffects", "ModifyHitNPCWithProj")
    })) {
    if ($check.Content -notmatch "public bool $($check.Helper)\(") {
        throw "$($check.Name) should expose $($check.Helper) for runtime subclass checks."
    }

    $helperBody = [regex]::Match(
        $check.Content,
        "public bool $($check.Helper)\([\s\S]+?\n\s*}",
        [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

    if ($helperBody -notmatch "GetModPlayer<EterniaPlayer>\(\)" -or
        $helperBody -notmatch "HasClassSoul" -or
        $helperBody -notmatch "ActiveSoul == SoulId\.$($check.Soul)") {
        throw "$($check.Name).$($check.Helper) should require an equipped $($check.Soul) class Soul, not only a subclass name."
    }

    foreach ($method in $check.Methods) {
        $methodBody = [regex]::Match(
            $check.Content,
            "(override .* $method|public .* $method)\([\s\S]+?\n\s*}",
            [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

        if ($methodBody -notmatch "!$($check.Helper)\(\)") {
            throw "$($check.Name).$method should gate behavior with $($check.Helper)()."
        }
    }
}

if ($fighterProjectile -notmatch "GetModPlayer<FighterPlayer>\(\)" -or
    $fighterProjectile -notmatch "fighterPlayer\.IsActiveFighter\(\)") {
    throw "FighterPunchProjectile should gate projectile combo logic with FighterPlayer.IsActiveFighter()."
}

if ($subclassLockHelper -notmatch "GetModPlayer<EterniaPlayer>\(\)" -or
    $subclassLockHelper -notmatch "HasClassSoul" -or
    $subclassLockHelper -notmatch "ClassPromotionRules\.IsPromotionForSoul") {
    throw "SubclassLockHelper.PlayerHasSubclass should require an active class Soul that owns the requested promotion."
}

if ($baseClass -notmatch "private bool IsActiveBaseClass\(SoulId expectedSoul,\s*string expectedSubclass\)") {
    throw "BaseClassPlayer should centralize base class runtime checks through IsActiveBaseClass."
}

foreach ($base in @(
    @{ Soul="Warrior"; Subclass="Warrior" },
    @{ Soul="Mage"; Subclass="Mage" },
    @{ Soul="Ranger"; Subclass="Ranger" },
    @{ Soul="Summoner"; Subclass="Summoner" })) {
    $pattern = "IsActiveBaseClass\(SoulId\.$($base.Soul),\s*`"$($base.Subclass)`"\)"

    if ($baseClass -notmatch $pattern) {
        throw "BaseClassPlayer should gate $($base.Subclass) resources/effects with an equipped $($base.Soul) Soul."
    }
}

if ($subclassEffects -notmatch "private bool IsActiveSubclass\(SoulId expectedSoul,\s*string expectedSubclass\)") {
    throw "SubclassEffectsPlayer should centralize promotion checks through IsActiveSubclass."
}

foreach ($promotion in @(
    @{ Soul="Warrior"; Subclass="Swordsman" },
    @{ Soul="Warrior"; Subclass="Fighter" },
    @{ Soul="Warrior"; Subclass="Guardian" },
    @{ Soul="Warrior"; Subclass="Yoyo Master" },
    @{ Soul="Warrior"; Subclass="Berserker" },
    @{ Soul="Warrior"; Subclass="Stunner" },
    @{ Soul="Ranger"; Subclass="Energy Gunner" },
    @{ Soul="Ranger"; Subclass="Archer" },
    @{ Soul="Ranger"; Subclass="Gunner" },
    @{ Soul="Ranger"; Subclass="Virtuoso" },
    @{ Soul="Mage"; Subclass="Elementalist" },
    @{ Soul="Mage"; Subclass="Cursed Mage" },
    @{ Soul="Mage"; Subclass="Infinity Mage" },
    @{ Soul="Mage"; Subclass="Arcane Bard" },
    @{ Soul="Summoner"; Subclass="Necromancer" },
    @{ Soul="Summoner"; Subclass="Beast Tamer" },
    @{ Soul="Summoner"; Subclass="Advanced Summoner" },
    @{ Soul="Summoner"; Subclass="Tech Summoner" })) {
    $pattern = "IsActiveSubclass\(SoulId\.$($promotion.Soul),\s*`"$($promotion.Subclass)`"\)"

    if ($subclassEffects -notmatch $pattern) {
        throw "SubclassEffectsPlayer should gate $($promotion.Subclass) effects with an equipped $($promotion.Soul) Soul."
    }
}

Write-Host "Subclass runtime gating source smoke test passed."
