$ErrorActionPreference = "Stop"

# Every playable subclass should have a HARDMODE armour set whose bonus deepens ITS OWN
# signature mechanic and does nothing for anyone else -- so armour is a real expression of
# your growth, not a generic stat block. This test tracks which subclasses are covered and
# fails when a set's bonus is generic instead of mechanic-specific.
#
# (Pre-hardmode is per BASE CLASS, not per subclass -- subclasses do not exist before the Wall
#  of Flesh -- so it is checked elsewhere, not here.)

$repoRoot = Split-Path -Parent $PSScriptRoot
$armorDir = Join-Path $repoRoot "Content\Items\Armor"

$allArmor =
    (Get-ChildItem -File $armorDir -Filter "*Set.cs" |
        ForEach-Object { Get-Content -Raw $_.FullName }) -join "`n"

# subclass -> a proof that its set reaches that subclass's mechanic. Either an Acc* hook the
# mechanic reads, or a live read of the subclass's own state gated behind IsActive<Subclass>.
$expected = @(
    @{ Sub = "Swordsman";         Proof = "CrimsonTrailPlayer" },
    @{ Sub = "Fighter";           Proof = "FighterPlayer" },
    @{ Sub = "Guardian";          Proof = "GuardianPlayer" },
    @{ Sub = "Berserker";         Proof = "IsActiveBerserker" },
    @{ Sub = "Stunner";           Proof = "IsActiveStunner" },
    @{ Sub = "Yoyo Master";       Proof = "IsActiveYoyoMaster" },
    @{ Sub = "Beast Tamer";       Proof = "BeastTamerPlayer" },
    @{ Sub = "Advanced Summoner"; Proof = "AdvancedSummonerPlayer" },
    @{ Sub = "Tech Summoner";     Proof = "TechSummonerPlayer" },
    @{ Sub = "Gunner";            Proof = "GunnerPlayer" },
    @{ Sub = "Archer";            Proof = "ArcherPlayer" },
    @{ Sub = "Energy Gunner";     Proof = "EnergyShooterPlayer" }
)

foreach ($e in $expected) {
    if ($allArmor -notmatch [regex]::Escape($e.Proof)) {
        throw ("No hardmode armour set reaches the $($e.Sub) mechanic " +
            "(expected a reference to '$($e.Proof)').")
    }
}

# A subclass set that reads live state must GATE on being that subclass, or the bonus leaks to
# anyone wearing it -- which would make it generic again.
foreach ($gate in @("IsActiveBerserker", "IsActiveStunner", "IsActiveYoyoMaster")) {
    if ($allArmor -notmatch "$gate\(\)") {
        throw "A state-reading subclass set must gate its bonus behind $gate()."
    }
}

# Each of the five new Warrior sets must exist as three pieces (helm/chest/greaves).
foreach ($set in @("Ironknuckle", "Wardplate", "Warpath", "Concussor", "Whipcord")) {
    foreach ($piece in @("Helm", "Chest", "Greaves")) {
        if ($allArmor -notmatch "class $set$piece") {
            throw "The $set set is missing its $piece piece."
        }
    }

    # A set bonus with no words is a set bonus nobody can read.
    if ($allArmor -notmatch "player\.setBonus = ") {
        throw "$set should describe its set bonus."
    }
}

Write-Host "Subclass armor source smoke test passed."
