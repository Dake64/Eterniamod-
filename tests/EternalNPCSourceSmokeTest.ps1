$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

$npc = Read-File "Content\NPCs\EternalNPC.cs"
$sub = Read-File "Content\Players\SubclassPlayer.cs"

# --- Still the entry point to the whole mod ----------------------------------

if ($npc -notmatch "GiveEmptySoul") {
    throw "The Eternal must still hand out the Empty Soul -- it is the entry point to everything."
}

# --- A SHOP, stocked for the soul you actually carry -------------------------

if ($npc -notmatch "public override void AddShops\(\)" -or $npc -notmatch "NPCShop") {
    throw "The Eternal should have a shop."
}

if ($npc -notmatch "public override void ModifyActiveShop\(string shopName, Item\[\] items\)") {
    throw "The Eternal's shop should be rebuilt per visit, so it can depend on your Soul."
}

# Each class gets its own stock -- that is the point of a soul-keeper's shop.
foreach ($pair in @(
    @("SoulId.Warrior", "SoulOfSteel"),
    @("SoulId.Mage", "SoulOfEmber"),
    @("SoulId.Ranger", "SoulOfTheHunt"),
    @("SoulId.Summoner", "SoulOfThePack"))) {

    if ($npc -notmatch [regex]::Escape($pair[0])) {
        throw "The shop should stock something for $($pair[0])."
    }
    if ($npc -notmatch $pair[1]) {
        throw "The $($pair[0]) shop should offer the $($pair[1]) accessory."
    }
}

# The way out of a build, once the world turns -- and it costs a fortune.
if ($npc -notmatch "Main\.hardMode" -or $npc -notmatch "SoulReforge") {
    throw "The Eternal should sell the Soul Reforge in Hardmode."
}

# --- "Read my soul": the only place the game reveals where you are heading ----
# Promotion is decided by your highest affinity, and nothing else tells you which
# subclass that will be. Losing this would make it hidden information again.

if ($npc -notmatch "ReadSoul") {
    throw "The Eternal should be able to read where your soul is leaning."
}

foreach ($call in @("DominantAffinityName\(\)", "DominantAffinityValue\(\)", "PredictedSubclass\(\)")) {
    if ($npc -notmatch $call) {
        throw "ReadSoul should report your affinity AND the subclass it is steering you toward ($call)."
    }
}

# Those three must exist on SubclassPlayer, which owns the promotion logic --
# the NPC must not re-derive it and drift out of sync.
foreach ($member in @("public string PredictedSubclass\(\)", "public string DominantAffinityName\(\)", "public int DominantAffinityValue\(\)")) {
    if ($sub -notmatch $member) {
        throw "SubclassPlayer should expose '$member' so the Eternal reads the REAL promotion rules."
    }
}

# The prediction must pretend Hardmode has arrived, or it would always answer
# "you are just a Ranger" before the Wall of Flesh and be useless.
$predict = [regex]::Match(
    $sub,
    "public string PredictedSubclass\(\)[\s\S]*?\n        \}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($predict -notmatch "ResolveSubclass" -or $predict -notmatch "true") {
    throw "PredictedSubclass must resolve as if Hardmode had arrived, or it cannot foretell anything."
}

# --- Reading a PROMOTED soul must not be a dead end ---------------------------
# It used to say what sealed your subclass and then only that a Reforge would undo it,
# which reads as "this is finished". Nothing else in the game tells you the subclass
# mechanic keeps growing.
$eternal = Read-File "Content\NPCs\EternalNPC.cs"

if ($eternal -notmatch "AwakeningCeremony\.MechanicOf") {
    throw "The Eternal should name your mechanic via AwakeningCeremony, not a second copy of that table."
}

# The reading must show which rung you are on AND how many exist -- otherwise the player has
# no way to know the mechanic still grows. It is a panel with pips now, not chat text.
if ($eternal -notmatch "SoulReadingUI\.Show") {
    throw "Reading a promoted soul should open the soul-reading panel, not dump lines into chat."
}

if ($eternal -notmatch "MechanicTier\.Current\(\)" -or
    $eternal -notmatch "MechanicTier\.Perfected") {
    throw "The reading should pass both your current rung and the top of the ladder."
}

$reading = Read-File "Content\UI\SoulReadingUI.cs"

if ($reading -notmatch "DrawTierPips") {
    throw "The panel should draw the rung as pips -- that is what reads at a glance."
}

# Pips only communicate progression if the unearned ones are still drawn.
if ($reading -notmatch "bool earned" -or $reading -notmatch "maxTier") {
    throw "The pips should show unearned rungs too, or they cannot imply what is still ahead."
}

if ($eternal -notmatch "MechanicGrowthLine") {
    throw "The Eternal should hint at the milestones that upgrade the mechanic."
}

$hints = [regex]::Match(
    $eternal,
    'private static string MechanicGrowthLine\([\s\S]+?\n\s{8}\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($hints -notmatch "MechanicTier\.") {
    throw "Growth hints should read the shared ladder rather than re-deriving milestones."
}

# Exactly ONE growth line is printed. Printing the generic promise alongside a subclass's
# specific one made the Eternal announce the same milestone twice in a row.
if ($eternal -match "foreach \(string line in MechanicGrowthLine") {
    throw "The Eternal should print a single growth line, not a list that can repeat a milestone."
}

# The Eternal now promises growth to EVERY subclass, so that promise has to be backed by a
# real implementation -- otherwise it is the invented growth this test exists to prevent.
$tierPlayer = Read-File "Content\Players\MechanicTierPlayer.cs"

if ($tierPlayer -notmatch "public override void PostUpdateEquips") {
    throw "MechanicTierPlayer must apply in PostUpdateEquips, before subclasses read their hooks."
}

if ($tierPlayer -notmatch "MechanicTier\.Steps\(\)" -or
    $tierPlayer -notmatch "steps <= 0") {
    throw "Tier growth must be driven by the shared ladder and be a no-op at tier 1."
}

# Feeding only one or two subclasses would make the Eternal's promise a lie for the rest.
$fed = [regex]::Matches($tierPlayer, "\.Acc[A-Za-z]+")

if ($fed.Count -lt 20) {
    throw ("MechanicTierPlayer should grow the mechanics of every subclass that exposes hooks " +
        "(found only $($fed.Count) Acc* boosts).")
}

# The ceremony is the single source of truth for mechanic names.
$ceremony = Read-File "Content\Systems\AwakeningCeremony.cs"

if ($ceremony -notmatch "public static string MechanicOf") {
    throw "AwakeningCeremony should expose MechanicOf so the Eternal need not duplicate the table."
}

Write-Host "Eternal NPC source smoke test passed."
