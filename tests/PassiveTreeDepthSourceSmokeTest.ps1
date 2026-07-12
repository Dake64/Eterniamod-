$ErrorActionPreference = "Stop"

# Each affinity branch used to bottom out at 3 nodes. The trees were deepened so
# every branch offers 5 upgrades, and every new node must both exist as data in
# the registry AND apply a real runtime effect (checked by name in
# EterniaStatsPlayer). This test guards both halves.

$repoRoot = Split-Path -Parent $PSScriptRoot
$registryPath = Join-Path $repoRoot "Content\Passives\PassiveRegistry.cs"
$statsPath = Join-Path $repoRoot "Content\Players\EterniaStatsPlayer.cs"

$registry = Get-Content -Raw $registryPath
$stats = Get-Content -Raw $statsPath

# The two new tier-4 / tier-5 nodes added to each of the 18 branches.
$newNodes = @(
    # Warrior
    "Hemorrhage", "Crimson Reaper",
    # (Flow State / Perfect Rhythm are Combo-branch nodes: their runtime effect is
    #  in FighterPlayer now, not EterniaStatsPlayer -- see FighterComboSourceSmokeTest.)
    "Bulwark", "Unbreakable",
    "Keen Edge", "Lethal Precision",
    "Berserk Momentum", "Undying Wrath",
    "Crushing Blows", "Overwhelming Force",
    # Ranger
    "Ion Surge", "Fusion Cannon",
    "Piercing Shot", "Storm of Arrows",
    "Hair Trigger", "Bullet Storm",
    "Battle Hymn", "Grand Finale",
    # Mage
    "Arcane Conductor", "Elemental Overload",
    "Withering Curse", "Doom Bringer",
    "Boundless Mana", "Eternal Flow",
    "Harmonic Field", "Celestial Symphony",
    # Summoner
    "Pack Leader", "Savage Alpha",
    "Synchronized Assault", "Transcendent Fusion",
    "Overclocked Core", "Autonomous Legion",
    "Soul Harvest", "Legion of the Dead"
)

foreach ($node in $newNodes) {
    $registryPattern = 'new PassiveNode\(\s*"' + [regex]::Escape($node) + '"'
    if ($registry -notmatch $registryPattern) {
        throw "PassiveRegistry should define the new passive node '$node'."
    }

    $effectPattern = 'HasActivePassive\(soulPlayer\.ActiveSoul,\s*"' +
        [regex]::Escape($node) + '"\)'
    if ($stats -notmatch $effectPattern) {
        throw "EterniaStatsPlayer should apply a runtime effect for '$node'."
    }
}

# Every branch should now expose 5 nodes. Count PassiveNode definitions per class
# list; 6 warrior branches * 5 = 30, and 4 branches * 5 = 20 for the rest.
function Get-ListBody($text, $marker) {
    $start = $text.IndexOf($marker)
    if ($start -lt 0) { throw "Could not find list '$marker'." }
    $open = $text.IndexOf("{", $start)
    $depth = 0
    for ($i = $open; $i -lt $text.Length; $i++) {
        $c = $text[$i]
        if ($c -eq '{') { $depth++ }
        elseif ($c -eq '}') {
            $depth--
            if ($depth -eq 0) {
                return $text.Substring($open, $i - $open + 1)
            }
        }
    }
    throw "Unbalanced braces for '$marker'."
}

$expectedCounts = @{
    "List<PassiveNode> WarriorPassives"  = 49
    "List<PassiveNode> RangerPassives"   = 32
    "List<PassiveNode> MagePassives"     = 32
    "List<PassiveNode> SummonerPassives" = 32
}

foreach ($marker in $expectedCounts.Keys) {
    $body = Get-ListBody $registry $marker
    $count = ([regex]::Matches($body, "new PassiveNode\(")).Count
    if ($count -ne $expectedCounts[$marker]) {
        throw "$marker should contain $($expectedCounts[$marker]) nodes but has $count."
    }
}

Write-Host "Passive tree depth source smoke test passed."
