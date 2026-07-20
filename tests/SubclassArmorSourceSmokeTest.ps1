$ErrorActionPreference = "Stop"

# Every PLAYABLE subclass should have a hardmode armour set whose bonus deepens its own
# signature mechanic. "Playable" is the v1 allowlist: only three subclasses per base class are
# unlocked (the rest are hidden until a later version -- see PassiveTreeV1Visibility). Building
# armour for a hidden subclass is a trap: it is craftable, but its subclass-gated bonus can
# never trigger, because no one can become that subclass yet.
#
# Pre-hardmode is per BASE CLASS (subclasses do not exist before the Wall of Flesh), so it is
# out of scope here.

$repoRoot = Split-Path -Parent $PSScriptRoot
$armorDir = Join-Path $repoRoot "Content\Items\Armor"

$allArmor =
    (Get-ChildItem -File $armorDir -Filter "*Set.cs" |
        ForEach-Object { Get-Content -Raw $_.FullName }) -join "`n"

# The twelve v1-playable subclasses -> a proof that a set reaches that subclass's mechanic,
# either an Acc* hook the mechanic reads, or a live read of its state gated by IsActive<Sub>.
$playable = @(
    @{ Sub = "Swordsman";         Proof = "CrimsonTrailPlayer" },
    @{ Sub = "Fighter";           Proof = "FighterPlayer" },
    @{ Sub = "Guardian";          Proof = "GuardianPlayer" },
    @{ Sub = "Elementalist";      Proof = "ElementalistPlayer" },
    @{ Sub = "Cursed Mage";       Proof = "CursedMagePlayer" },
    @{ Sub = "Infinity Mage";     Proof = "InfinityMagePlayer" },
    @{ Sub = "Energy Gunner";     Proof = "EnergyShooterPlayer" },
    @{ Sub = "Archer";            Proof = "ArcherPlayer" },
    @{ Sub = "Gunner";            Proof = "GunnerPlayer" },
    @{ Sub = "Beast Tamer";       Proof = "BeastTamerPlayer" },
    @{ Sub = "Advanced Summoner"; Proof = "AdvancedSummonerPlayer" },
    @{ Sub = "Tech Summoner";     Proof = "TechSummonerPlayer" }
)

foreach ($e in $playable) {
    if ($allArmor -notmatch [regex]::Escape($e.Proof)) {
        throw ("No hardmode armour set reaches the playable subclass $($e.Sub) " +
            "(expected a reference to '$($e.Proof)').")
    }
}

# The three sets added this session, each as helm/chest/greaves (or the mage equivalent).
foreach ($set in @(
    @{ Name = "Ironknuckle"; Pieces = @("Helm", "Chest", "Greaves") },
    @{ Name = "Wardplate";   Pieces = @("Helm", "Chest", "Greaves") },
    @{ Name = "Everflow";    Pieces = @("Crown", "Robe", "Leggings") })) {

    foreach ($piece in $set.Pieces) {
        if ($allArmor -notmatch "class $($set.Name)$piece") {
            throw "The $($set.Name) set is missing its $piece piece."
        }
    }
}

# A set that reads live state must GATE on being that subclass, or its bonus leaks to anyone
# wearing it -- which would make it a generic stat block again.
foreach ($gate in @("IsActiveInfinityMage")) {
    if ($allArmor -notmatch "$gate\(\)") {
        throw "A state-reading subclass set must gate its bonus behind $gate()."
    }
}

Write-Host "Subclass armor source smoke test passed."
